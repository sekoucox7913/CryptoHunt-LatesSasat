using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using MiniJSON;

using GoShared;
using Mapbox.Utils;
using Mapbox.VectorTile;
using Mapbox.VectorTile.ExtensionMethods;
using Mapbox.VectorTile.Geometry;
using UnityEngine.Profiling;

namespace GoMap
{
	[ExecuteInEditMode]
	public abstract class GOPBFTile : GOTile
	{

		public VectorTile vt;


		public abstract GOFeature ParseFeatureData (VectorTileFeature feature, IDictionary properties, GOLayer layer, float  featureIndex, Int64 layerIndex);
		public abstract string GetLayersStrings (GOLayer layer);

		public override IEnumerator BuildTile(IDictionary mapData, GOLayer layer, bool delayedLoad)
		{ 
			yield return null;
		}

		public override IEnumerator ParseTileData(object m, Coordinates tilecenter, int zoom, GOLayer[] layers, bool delayedLoad, List <string> layerNames)
		{

			if (mapData == null) {
				Debug.LogWarning ("Map Data is null!");
				#if !UNITY_WEBPLAYER
				FileHandler.Remove (gameObject.name);
				#endif
				yield break;
			}

			var decompressed = Compression.Decompress((byte[])mapData);
			vt = new VectorTile(decompressed,false);

			foreach (GOLayer layer in layers) {

				string[] lyrs = GetLayersStrings(layer).Split(',');

				foreach (string l in lyrs) {
					VectorTileLayer lyr = vt.GetLayer(l);
					if (lyr != null)
						yield return (StartCoroutine(BuildLayer(lyr,layer,delayedLoad)) );
				}
			}
		}

		public IEnumerator BuildLayer(VectorTileLayer layerData, GOLayer layer, bool delayedLoad)
		{

			Profiler.BeginSample("[GoMap] [BuildLayer] game object");
			GameObject parent = null;
			if (transform.Find (layer.name) == null) {
				parent = new GameObject ();
				parent.name = layer.name;
				parent.transform.parent = this.transform;
				parent.SetActive (!layer.startInactive);
			} else {
				parent = transform.Find (layer.name).gameObject;
			}
			Profiler.EndSample ();

			int  featureCount = layerData.FeatureCount();

			if (featureCount == 0)
				yield break;

			List<GOFeature> stack = new List<GOFeature> ();

			//Caching variables..
			VectorTileFeature feature;
			List<List<LatLng>> geomWgs84; //= new List<List<LatLng>>();
			GOFeature goFeature;
			Dictionary<string,object> properties = null;
			List<KeyValue> attributes = null;
			List <Vector3> convertedGeometry = null;


			for (int i = 0; i < featureCount; i++) {

				feature = layerData.GetFeature(i);
				properties = feature.GetProperties ();
				geomWgs84 = feature.GeometryAsWgs84((ulong)map.zoomLevel, (ulong)tileCoordinates.x, (ulong)tileCoordinates.y,0);
				attributes = GOFeature.PropertiesToAttributes (properties);
				if (geomWgs84.Count > 0)
					convertedGeometry = GOFeature.CoordsToVerts (geomWgs84[0] , false);

				//get the feature (here is the actual protobuf conversion)
				goFeature = ParseFeatureData (feature, properties,layer, -1,-1);

				//8-11mb
				if (layer.useOnly.Length > 0 && !layer.useOnly.Contains (goFeature.kind)) {
					continue;
				}
				if (layer.avoid.Length > 0 && layer.avoid.Contains (goFeature.kind)) {
					continue;
				}

				if (layer.layerType == GOLayer.GOLayerType.Roads) {
					GORoadFeature grf = (GORoadFeature)goFeature;
					if ((grf.isBridge && !layer.useBridges) || (grf.isTunnel && !layer.useTunnels) || (grf.isLink && !layer.useBridges)) {
						continue;
					}
				}

				GOFeatureType gotype = feature.GOFeatureType(geomWgs84);

				if (gotype == GOFeatureType.Undefined || feature.GeometryType == GeomType.POINT) {
					continue;
				}
				if (feature.GeometryType == GeomType.POLYGON && layer.layerType == GOLayer.GOLayerType.Roads)
					continue;

				Int64 index = vt.LayerNames().IndexOf(layerData.Name)+1;

				GOFeature gf;
				Profiler.BeginSample("[GoMap] [BuildLayer] IF");


				if (gotype == GOFeatureType.MultiLine || (gotype == GOFeatureType.Polygon && !layer.isPolygon)) {

					Profiler.BeginSample("[GoMap] [BuildLayer] multi line");
					foreach (IList geometry in geomWgs84) {

						float indexMulti = ((float)geomWgs84.IndexOf((List<LatLng>)geometry)/geomWgs84.Count);
						gf = ParseFeatureData (feature, properties,layer, Convert.ToInt64(i) + indexMulti, index);
						gf.geometry = geometry;
						gf.layer = layer;
						gf.parent = parent;
						gf.properties = properties;
						gf.ConvertGeometries ();
						gf.attributes = attributes;
						gf.goFeatureType = GOFeatureType.MultiLine;
						stack.Add(gf);
					}
					Profiler.EndSample ();
				} 

				else if (gotype == GOFeatureType.Line) {

					Profiler.BeginSample("[GoMap] [BuildLayer] line");

					gf = ParseFeatureData (feature,properties, layer, Convert.ToInt64(i), index);
					gf.geometry = geomWgs84 [0];
					gf.layer = layer;
					gf.parent = parent;
					gf.properties = properties;
					gf.convertedGeometry = convertedGeometry;
					gf.attributes = attributes;
					gf.index = (Int64)i + vt.LayerNames().IndexOf(layerData.Name);
					gf.goFeatureType = GOFeatureType.Line;
					if (geomWgs84.Count == 0)
						continue;

					stack.Add(gf);

					Profiler.EndSample ();

				} 

				else if (gotype == GOFeatureType.Polygon) {
					
					Profiler.BeginSample("[GoMap] [BuildLayer] polygon");

					gf = ParseFeatureData (feature, properties, layer, Convert.ToInt64(i), index);
					gf.geometry = geomWgs84 [0];
					gf.layer = layer;
					gf.parent = parent;
					gf.properties = properties;
					gf.convertedGeometry = convertedGeometry;
					gf.attributes = attributes;
					gf.index = (Int64)i + vt.LayerNames().IndexOf(layerData.Name);
					gf.goFeatureType = GOFeatureType.Polygon;

					stack.Add(gf);	

					Profiler.EndSample ();

				}

				else if (gotype == GOFeatureType.MultiPolygon) {

					Profiler.BeginSample("[GoMap] [BuildLayer] multi polygon");

//					GameObject multi = new GameObject ("MultiPolygon");
//					multi.transform.parent = parent.transform;

					IList subject = null;
					IList clips = new List<List<LatLng>>();

					for (int j = 0; j<geomWgs84.Count; j++) { //Clip ascending

						IList p = geomWgs84 [j];
						if (GOFeature.IsGeoPolygonClockwise (p)) {
							subject = p;
						} 
						else {
							//Add clip
							clips.Add (p);
						}
						//Last one
						if (j == geomWgs84.Count - 1 || (j<geomWgs84.Count-1 && GOFeature.IsGeoPolygonClockwise (geomWgs84 [j + 1]) && subject != null)) {

							gf = ParseFeatureData (feature, properties, layer, Convert.ToInt64(i), index);
							gf.geometry = subject;
							gf.clips = clips;
							gf.layer = layer;
							gf.parent = parent;
							gf.properties = properties;
							gf.ConvertGeometries ();
							gf.attributes = attributes;
							gf.index = (Int64)i + vt.LayerNames().IndexOf(layerData.Name);
							gf.goFeatureType = GOFeatureType.MultiPolygon;

							stack.Add (gf);

							subject = null;
							clips = new List<List<LatLng>>();
						}
					}
					Profiler.EndSample ();

				}
				Profiler.EndSample ();

			}
				
			Profiler.BeginSample("[GoMap] [BuildLayer] merge roads");
			IList iStack = (IList)stack;
			if (layer.layerType == GOLayer.GOLayerType.Roads) {
				iStack = GORoadFeature.MergeRoads (iStack);
//				Debug.Log ("Roads: "+stack.Count+" Merged: "+iStack.Count);
			}
			Profiler.EndSample ();

			int n = 25;
			for (int i = 0; i < iStack.Count; i+=n) {

				for (int k = 0; k<n; k++) {
					if (i + k >= iStack.Count) {
						yield return null;
						break;
					}

					GOFeature r = (GOFeature)iStack [i + k];
					IEnumerator routine = r.BuildFeature (this, delayedLoad);
					if (routine != null) {
						if (Application.isPlaying)
							StartCoroutine (routine);
						else
							GORoutine.start (routine, this);
					}
				}
				yield return null;
			}

			Resources.UnloadUnusedAssets ();

			yield return null;
		}
	}
}
