using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoShared;
using System;
using Mapbox.VectorTile.Geometry;
using UnityEngine.Profiling;


namespace GoMap {

	[System.Serializable]
	public class GOFeature {

		public string name;
		public GOFeatureKind kind = GOFeatureKind.baseKind;
		public GOPOIKind poiKind;

		public string detail;
		public float sort;
		public float y;
		public float height;
		public float index;

		public GOFeatureType goFeatureType;


		/*[HideInInspector]*/ public IList geometry;
		public Coordinates poiCoordinates;

		[HideInInspector] public IList clips;

		public List <Vector3> convertedGeometry;
		public Vector3 poiGeometry;


		public IDictionary properties;
		public List<KeyValue> attributes;

		[HideInInspector] public GameObject parent;
		[HideInInspector] public GOLayer layer;
		[HideInInspector] public GOPOILayer poiLayer;
	
		[HideInInspector] public GOMesh preloadedMeshData;
		[HideInInspector] public GORenderingOptions renderingOptions;
		[HideInInspector] public GOPOIRendering poiRendering;

		bool defaultRendering = true;


		//Terrain elevation costants
		public static int BuildingElevationOffset = 10;
		public static int RoadsBreakEvery = 15;

		public GOFeature () {

		}

		public GOFeature (GOFeature f) {

			name = f.name;
			index = f.index;
			goFeatureType = f.goFeatureType;
			properties = f.properties;
			attributes = f.attributes;
			layer = f.layer;


			//After editing the feature in tile subclasses.

			//		public string kind;
			kind = f.kind;
			renderingOptions = f.renderingOptions;
			detail = f.detail;
			sort = f.sort;
			y = f.y;
			height = f.height;
			index = f.index;

		}

		#region BUILDERS

		public void ConvertGeometries () {

			bool noise = layer.layerType == GOLayer.GOLayerType.Buildings;
			convertedGeometry = CoordsToVerts (geometry,noise);

		}

		public void ConvertPOIGeometries () {

			LatLng c = (LatLng)geometry [0];
			poiCoordinates = new Coordinates (c.Lat, c.Lng, 0);
			poiGeometry = poiCoordinates.convertCoordinateToVector ();

		}

		public void ConvertAttributes () {

			List <KeyValue> list = new List <KeyValue>();

			foreach (string key in properties.Keys) {
				KeyValue keyValue = new KeyValue ();
				keyValue.key = key;
				if (properties[key] != null) {
					keyValue.value = properties[key].ToString();
				}
				list.Add (keyValue);
			}

			attributes = list;
		}


		public virtual IEnumerator BuildFeature (GOTile tile, bool delayedLoad) {
			
			if (goFeatureType == GOFeatureType.Undefined) {
				Debug.Log ("type is null");
				return null;
			}
			try {
				if (goFeatureType == GOFeatureType.Line || goFeatureType == GOFeatureType.MultiLine) {
					return CreateLine (tile,delayedLoad);
				} 
				else if (goFeatureType == GOFeatureType.Polygon || goFeatureType == GOFeatureType.MultiPolygon){
					return CreatePolygon (tile,delayedLoad);
				}
				else if (goFeatureType == GOFeatureType.Point){
					return CreatePoi (tile,delayedLoad);
				} 
				else return null;
			}
			catch (Exception ex)
			{
				Debug.Log ("[GOFeature] Catched exception: " + ex);
				return null;
			}
		}

		public virtual IEnumerator CreateLine (GOTile tile, bool delayedLoad)
		{

			GORenderingOptions renderingOptions = GetRenderingOptions();

			if (renderingOptions.lineWidth == 0) {
				yield break;
			}

			GameObject line = new GameObject (name != null? name:kind.ToString());
			line.transform.parent = parent.transform;

			//Layer mask
			if (layer.useLayerMask == true) {
				tile.AddObjectToLayerMask (layer, line);				
			} 

			GOFeatureMeshBuilder builder = new GOFeatureMeshBuilder (this);

			if (preloadedMeshData != null)
				builder.BuildLineFromPreloaded (line, this, tile.map);
			else if (tile.map.mapType == GOMap.GOMapType.MapzenJson)
				builder.BuildLine(line, layer ,renderingOptions, tile.map);

			GOFeatureBehaviour fb = line.AddComponent<GOFeatureBehaviour> ();
			fb.goFeature = this;

			if (layer.layerType == GOLayer.GOLayerType.Roads && name != null && name.Length > 0 && renderingOptions.useStreetNames) {
				GOStreetName streetName = new GameObject ().AddComponent<GOStreetName> ();
				streetName.gameObject.name = "";
				streetName.transform.SetParent (line.transform);
				yield return tile.StartCoroutine(streetName.Build (name,tile.map.textShader,tile.map.streetnameColor));
			}

			if (layer.OnFeatureLoad != null) {
				layer.OnFeatureLoad.Invoke(builder.mesh,layer,kind, builder.center);
			}

			if (delayedLoad)
				yield return null;
		}

		public virtual IEnumerator CreatePolygon (GOTile tile, bool delayedLoad)
		{

			Profiler.BeginSample ("[GOFeature] CreatePolygon ALLOC");
			GOFeatureMeshBuilder builder = new GOFeatureMeshBuilder(this);
			Profiler.EndSample ();

			Profiler.BeginSample ("[GOFeature] CreatePolygon Material");
			//Materials
			Material material = tile.GetMaterial(renderingOptions,builder.center);
			Material roofMat = renderingOptions.roofMaterial;

			if (sort != 0) {
				if (material)
					material.renderQueue = -(int)sort;
				if (roofMat)
					roofMat.renderQueue = -(int)sort;
			}
			Profiler.EndSample ();

			Profiler.BeginSample ("[GOFeature] CreatePolygon Center");
			//Group buildings by center coordinates
			if (layer.layerType == GOLayer.GOLayerType.Buildings && defaultRendering) {
				GameObject centerContainer = tile.findNearestCenter(builder.center,parent,material);
				parent = centerContainer;
				material = centerContainer.GetComponent<GOMatHolder> ().material;
			}
			Profiler.EndSample();

			if (!layer.useRealHeight) {
				height = renderingOptions.polygonHeight;
			}

			int offset = 0;
			float trueHeight = height;
			#if GOLINK
			if (GOMap.GOLink) {
				trueHeight += BuildingElevationOffset;
				//[GOLINK] GOTerrain link (This requires GOTerrain! https://www.assetstore.unity3d.com/#!/content/84198) 
				if (tile.map.goTerrain != null) {
					offset = BuildingElevationOffset;
					if (y < offset)
						y = tile.map.goTerrain.FindAltitudeForVector(builder.center)-offset;
				}
			}
			#endif

			Profiler.BeginSample ("[GOFeature] CreatePolygon MESH");
			GameObject polygon = null;
			if (preloadedMeshData != null)
				polygon = builder.BuildPolygonFromPreloaded(this);
			else if (tile.map.mapType == GOMap.GOMapType.MapzenJson) //ONLY FOR JSON 
				polygon = builder.BuildPolygon(layer,trueHeight+offset);
			
			Profiler.EndSample ();

			if (polygon == null)
				yield break;

			polygon.name = name;
			polygon.transform.parent = parent.transform;

			//Layer mask
			if (layer.useLayerMask == true) {
				tile.AddObjectToLayerMask (layer, polygon);	
			} 

			if (renderingOptions.tag.Length > 0) {
				polygon.tag = renderingOptions.tag;
			}

			if (layer.useRealHeight && roofMat != null) {

				Profiler.BeginSample ("[GOFeature] CreatePolygon ROOF");

				GameObject roof;
				if (preloadedMeshData != null && preloadedMeshData.secondaryMesh != null)
					roof = builder.CreateRoofFromPreloaded (preloadedMeshData.secondaryMesh);
				else  roof = builder.CreateRoof();

				roof.name = "roof";
				roof.transform.parent = polygon.transform;
				roof.GetComponent<MeshRenderer> ().material = roofMat;
				roof.transform.position = new Vector3 (roof.transform.position.x,trueHeight+0.11f,roof.transform.position.z);
				roof.tag = polygon.tag;
				roof.layer = polygon.layer;

				Profiler.EndSample ();
			}

			Profiler.BeginSample ("[GOFeature] TRANSFORM");
			Vector3 pos = polygon.transform.position;
			pos.y = y;
			if (layer.layerType == GOLayer.GOLayerType.Buildings)
				y += GOFeatureMeshBuilder.Noise ();

			polygon.transform.position = pos;
			polygon.transform.localPosition = pos;

			GOFeatureBehaviour fb = polygon.AddComponent<GOFeatureBehaviour> ();
			fb.goFeature = this;

			builder.meshRenderer.material = material;

			if (layer.OnFeatureLoad != null) {
				layer.OnFeatureLoad.Invoke(builder.mesh,layer,kind, builder.center);
			}
			Profiler.EndSample ();

			preloadedMeshData = null;

			if (delayedLoad)
				yield return null;

		}

		IEnumerator CreatePoi(GOTile tile, bool delayedLoad) {

			if (poiRendering == null) {
				yield break;
			}

			GameObject poi = GameObject.Instantiate (poiRendering.prefab);

			#if GOLINK
			poiGeometry.y = GoTerrain.GOTerrain.RaycastAltitudeForVector(poiGeometry);
			#endif

			poi.transform.localPosition = poiGeometry;

			poi.transform.parent = parent.transform;
			poi.name = name;

			//Layer mask
			if (poiLayer.useLayerMask == true) {
				tile.AddObjectToLayerMask (layer, poi);				
			} 

			GOFeatureBehaviour fb = poi.AddComponent<GOFeatureBehaviour> ();
			fb.goFeature = this;

			if (poiRendering.OnPoiLoad != null) {
				poiRendering.OnPoiLoad.Invoke(this);
			}

			if (delayedLoad)
				yield return null;

		}

		#endregion

		#region UTILS

		public void setRenderingOptions () {
			
			renderingOptions = layer.defaultRendering;
			foreach (GORenderingOptions r in layer.renderingOptions) {
				if (r.kind == kind) {
					defaultRendering = false;
					renderingOptions = r;
					break;
				}
			}
		}

		public GORenderingOptions GetRenderingOptions () {
			GORenderingOptions renderingOptions = layer.defaultRendering;
			foreach (GORenderingOptions r in layer.renderingOptions) {
				if (r.kind == kind) {
					defaultRendering = false;
					renderingOptions = r;
					break;
				}
			}
			return renderingOptions;
		}

		public static List <KeyValue> PropertiesToAttributes (IDictionary props) {

			List <KeyValue> list = new List <KeyValue>();
			KeyValue keyValue;

			foreach (string key in props.Keys) {

				keyValue = new KeyValue ();

				keyValue.key = key;
				if (props[key] != null) {
					keyValue.value = props[key].ToString();
				}
				list.Add (keyValue);
			}

			return list;
		}

		public static List<Vector3> CoordsToVerts (IList geometry, bool withNoise) {

			var convertedGeometry = new List<Vector3>();
			for (int i = 0; i < geometry.Count; i++) {
				if (geometry.GetType () == typeof(List<LatLng>)) { //Mapbox 
					LatLng c = (LatLng)geometry [i];
					Coordinates coords = new Coordinates (c.Lat, c.Lng, 0);
					Vector3 p = coords.convertCoordinateToVector ();

					if (withNoise && i != 0 && i != geometry.Count-1){
						float noise = GOFeatureMeshBuilder.Noise ();
						p.x += noise;
						p.z += noise;
					}
		
					convertedGeometry.Add (p);
				} else { //Mapzen
					IList c = (IList)geometry[i];
					Coordinates coords = new Coordinates ((double)c[1], (double)c[0],0);
					convertedGeometry.Add(coords.convertCoordinateToVector());
				}
			}
			return convertedGeometry;
		}

//		public static Vector3 PoiCoordsToVert (IList geometry) {
//
//			if (geometry.GetType () == typeof(List<LatLng>)) { //Mapbox 
//				LatLng c = (LatLng)geometry [0];
//				Coordinates coords = new Coordinates (c.Lat, c.Lng, 0);
//				return coords.convertCoordinateToVector ();
//			} 
//
//			return Vector3.zero;
//		}

		public static bool IsGeoPolygonClockwise(IList coords)
		{
			return (IsClockwise(GOFeature.CoordsToVerts(coords,false)));

		}

		public static bool IsClockwise(IList<Vector3> vertices)
		{
			double sum = 0.0;
			for (int i = 0; i < vertices.Count; i++) {
				Vector3 v1 = vertices[i];
				Vector3 v2 = vertices[(i + 1) % vertices.Count];
				sum += (v2.x - v1.x) * (v2.z + v1.z);
			}
			return sum > 0.0;
		}
			

		#endregion
	}

	[System.Serializable]
	public class KeyValue
	{
		public string key;
		public string value;
	}
}