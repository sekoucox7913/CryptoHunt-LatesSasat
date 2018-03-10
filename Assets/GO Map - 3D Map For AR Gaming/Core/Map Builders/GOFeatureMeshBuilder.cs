using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;
using UnityEngine.Profiling;
using GoShared;

#if GOLINK
using GoTerrain;
#endif

namespace GoMap
{

    public class GOFeatureMeshBuilder
    {
		GOFeature feature;
		public Mesh mesh;
		public Mesh mesh2D;
		public Vector3 center;
		static System.Random random = new System.Random ();
		public MeshRenderer meshRenderer;

		MeshJob job;
		public GameObject gameObject;

		public GOFeatureMeshBuilder (GOFeature f) {

			feature = f;
			if (feature.goFeatureType == GOFeatureType.Polygon || feature.goFeatureType == GOFeatureType.MultiPolygon)
				center = feature.convertedGeometry.Aggregate((acc, cur) => acc + cur) / feature.convertedGeometry.Count;

		}

		#region Builders

		public void BuildLine(GameObject line, GOLayer layer, GORenderingOptions renderingOptions , GOMap map)
        {
			if (feature.convertedGeometry.Count == 2 && feature.convertedGeometry[0].Equals(feature.convertedGeometry[1])) {
				return;
			}

			#if GOLINK
			feature.convertedGeometry = GOFeatureMeshBuilder.BreakLine(feature.convertedGeometry,map.goTerrain);
			#endif

			if (renderingOptions.tag.Length > 0) {
				line.tag = renderingOptions.tag;
			}

			if (renderingOptions.material)
				renderingOptions.material.renderQueue = -(int)feature.sort;
			if (renderingOptions.outlineMaterial)
				renderingOptions.outlineMaterial.renderQueue = -(int)feature.sort;
			
			GOLineMesh lineMesh = new GOLineMesh (feature.convertedGeometry);
			lineMesh.width = renderingOptions.lineWidth;
			lineMesh.load (line);
			mesh = lineMesh.mesh;
			line.GetComponent<Renderer>().material = renderingOptions.material;

			Vector3 position = line.transform.position;
			position.y = feature.y;

			#if GOLINK
			if (renderingOptions.polygonHeight > 0) {
				int offset = GOFeature.BuildingElevationOffset;
				line.GetComponent<MeshFilter> ().sharedMesh = SimpleExtruder.Extrude (line.GetComponent<MeshFilter> ().sharedMesh, line, renderingOptions.polygonHeight + offset);
				position.y -= offset;
			}
			#else

			#endif

			line.transform.position = position;

			if (renderingOptions.outlineMaterial != null) {
				GameObject outline = CreateRoadOutline (line,renderingOptions.outlineMaterial, renderingOptions.lineWidth + layer.defaultRendering.outlineWidth);
				if (layer.useColliders)
					outline.AddComponent<MeshCollider> ().sharedMesh = outline.GetComponent<MeshFilter> ().sharedMesh;

				outline.layer = line.layer;
				outline.tag = line.tag;
				
			} else if (layer.useColliders) {
//				Mesh m = gameObject.GetComponent<MeshFilter> ().sharedMesh;
				line.AddComponent<MeshCollider> ();
			}
        }

		public GameObject BuildPolygon(GOLayer layer, float height)
		{

			if (feature.convertedGeometry.Count == 2 && feature.convertedGeometry[0].Equals(feature.convertedGeometry[1])) {
				return null;
			}
			List<Vector3> clean = feature.convertedGeometry.Distinct().ToList();

			if (clean == null || clean.Count <= 2)
				return null;

			GameObject polygon = new GameObject();

			Profiler.BeginSample("[GoMap] Start poly2mesh");
			Poly2Mesh.Polygon poly = new Poly2Mesh.Polygon();
			poly.outside = feature.convertedGeometry;
			if (feature.clips != null ) {
				foreach (IList clipVerts in feature.clips) {
					poly.holes.Add(GOFeature.CoordsToVerts(clipVerts,true));
				}
			}
			Profiler.EndSample ();

			MeshFilter filter = polygon.AddComponent<MeshFilter>();
			meshRenderer = polygon.AddComponent<MeshRenderer>();

			Profiler.BeginSample("[GoMap] Create polygon mesh");
			try {
				mesh = Poly2Mesh.CreateMesh (poly);
			} catch {
				
			}
			Profiler.EndSample ();


			if (mesh) {

				Profiler.BeginSample("[GoMap] Set polygon UV");
				Vector2[] uvs = new Vector2[mesh.vertices.Length];
				Vector3[] vertices = mesh.vertices;
				for (int i=0; i < uvs.Length; i++) {
					uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
				}
				mesh.uv = uvs;
				Profiler.EndSample ();

				Profiler.BeginSample("[GoMap] instantiate mesh 2D");
				mesh2D = Mesh.Instantiate(mesh);
				Profiler.EndSample ();

				Profiler.BeginSample("[GoMap] polygon extrusion");
				if (height > 0) {
					mesh = SimpleExtruder.SliceExtrude (mesh, polygon, height, 4f,4f,10f);
//					mesh = SimpleExtruder.Extrude (mesh, polygon, height);
				}
				Profiler.EndSample();

			}


			filter.sharedMesh = mesh;

			if (layer.useColliders && mesh != null && feature.convertedGeometry.Count() > 2)
				polygon.AddComponent<MeshCollider>().sharedMesh = mesh;


			return polygon;

		}

		#endregion

		#region LINE UTILS

		GameObject CreateRoadOutline (GameObject line, Material material, float width) {

			GameObject outline = new GameObject ("outline");
			outline.transform.parent = line.transform;

			material.renderQueue = -((int)feature.sort-1);

			GOLineMesh lineMesh = new GOLineMesh (feature.convertedGeometry);
			lineMesh.width = width;
			lineMesh.load (outline);

			Vector3 position = outline.transform.position;
			position.y = -0.039f;
			outline.transform.localPosition = position;

			outline.GetComponent<Renderer>().material = material;

			return outline;
		}



		//[GOLINK] GOTerrain link (This requires GOTerrain! https://www.assetstore.unity3d.com/#!/content/84198) 
		#if GOLINK
		public static List<Vector3> BreakLine (List<Vector3> verts, GoTerrain.GOTerrain terrain) {

			List<Vector3> brokenVerts = new List <Vector3> ();
			for (int i = 0; i<verts.Count-1; i++) {
				
				float d = Vector3.Distance (verts [i], verts [i + 1]);
				if (d > GOFeature.RoadsBreakEvery) {
					for (int j = 0; j < d; j += GOFeature.RoadsBreakEvery) {
						Vector3 P = LerpByDistance (verts [i], verts [i + 1], j);
						P.y = terrain.FindAltitudeForVector (P);
						brokenVerts.Add (P);
					}
				} else {
					Vector3 P = verts [i];
					P.y = terrain.FindAltitudeForVector (P);
					brokenVerts.Add(P);
				}

			}
			Vector3 Pn = verts [verts.Count - 1];
			Pn.y = terrain.FindAltitudeForVector (Pn);
			brokenVerts.Add (Pn);
			return brokenVerts;
		}

		public static Vector3 LerpByDistance(Vector3 A, Vector3 B, float x)
		{
			Vector3 P = x * Vector3.Normalize(B - A) + A;
			return P;
		}
		#endif

		#endregion

		#region POLYGON UTILS

		public GameObject CreateRoof (){

			GameObject roof = new GameObject();
			MeshFilter filter = roof.AddComponent<MeshFilter>();
			roof.AddComponent(typeof(MeshRenderer));
			filter.mesh = mesh2D;
			return roof;
		}

		public static string VectorListToString (List<Vector3> list) {

			list =  new HashSet<Vector3>(list).ToList();
			string s = "";
			foreach (Vector3 v in list) {
				s += v.ToString() + " ";
			}
			return s;

		}

		#endregion

		#region PreloadedData 

		public static GOMesh PreloadFeatureData (GOFeature feature) {

			try {
				switch (feature.goFeatureType) {
				case GOFeatureType.Polygon:
					return PreloadPolygon (feature);
				case GOFeatureType.MultiPolygon:
					return PreloadPolygon (feature);
				case GOFeatureType.Line:
					return PreloadLine (feature);
				case GOFeatureType.MultiLine:
					return PreloadLine (feature);
					default:
					return null;
				}
			} catch (Exception ex) {
				Debug.LogWarning ("[GOMAP] error catched in feature: "+feature.name+", "+feature.kind + ", "+feature.convertedGeometry + ", " +ex);
				return null;
			}
		}

		public static GOMesh PreloadPolygon (GOFeature feature) {

			if (feature.convertedGeometry == null)
				return null;
			
			if (feature.convertedGeometry.Count == 2 && feature.convertedGeometry[0].Equals(feature.convertedGeometry[1])) {
				return null;
			}

			List<Vector3> clean = feature.convertedGeometry.Distinct().ToList();

			if (clean == null || clean.Count <= 2)
				return null;


			Poly2Mesh.Polygon poly = new Poly2Mesh.Polygon();
			poly.outside = feature.convertedGeometry;
			if (feature.clips != null ) {
				foreach (List<Vector3> clipVerts in feature.clips) {
					poly.holes.Add(clipVerts);
				}
			}

			GOMesh preMesh = null;

//			try {
				preMesh = Poly2Mesh.CreateMeshInBackground (poly);
//			} catch {
//				Debug.LogWarning ("Catched polygon");
//			}

			if (preMesh != null) {

				Vector2[] uvs = new Vector2[preMesh.vertices.Length];
				Vector3[] vertices = preMesh.vertices;
				for (int i=0; i < uvs.Length; i++) {
					uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
				}
				preMesh.uv = uvs;

				if (feature.height > 0) {
					preMesh.secondaryMesh = new GOMesh (preMesh);

					float h = feature.height;

					if (GOMap.GOLink)
						h += GOFeature.BuildingElevationOffset;

					h += Noise ();

					preMesh = SimpleExtruder.SliceExtrudePremesh (preMesh,h, 4f,4f,10f);
				}

			}


			return preMesh;
		}

		public static GOMesh PreloadLine (GOFeature feature) {


			if (feature.convertedGeometry.Count == 2 && feature.convertedGeometry[0].Equals(feature.convertedGeometry[1])) {
				return null;
			}

			GOMesh preMesh = new GOMesh ();

			GOLineMesh lineMesh = new GOLineMesh (feature.convertedGeometry);

			lineMesh.width = feature.renderingOptions.lineWidth;
			preMesh = lineMesh.CreatePremesh();

			if (feature.height > 0) {
				float h = feature.height;

				if (GOMap.GOLink)
					h += GOFeature.BuildingElevationOffset;
				
				preMesh = SimpleExtruder.SliceExtrudePremesh (preMesh, h + Noise(), 4f,4f,10f);
			}

			if (feature.renderingOptions.outlineWidth > 0) {
				lineMesh.width = feature.renderingOptions.lineWidth + feature.layer.defaultRendering.outlineWidth;
				preMesh.secondaryMesh = lineMesh.CreatePremesh();
			}

			return preMesh;
		}

		public static float Noise() {
			double r = random.NextDouble ();
			return ((float)r/10f);
		}

			
		#endregion

		#region New Builders

		public void BuildLineFromPreloaded(GameObject line, GOFeature feature, GOMap map)
		{

			if (feature.preloadedMeshData == null)
				return;

			GORenderingOptions renderingOptions = feature.renderingOptions;

			if (renderingOptions.tag.Length > 0) {
				line.tag = renderingOptions.tag;
			}

			if (renderingOptions.material)
				renderingOptions.material.renderQueue = -(int)feature.sort;
			if (renderingOptions.outlineMaterial)
				renderingOptions.outlineMaterial.renderQueue = -(int)feature.sort;


			MeshFilter filter = line.GetComponent<MeshFilter> ();
			if (filter == null) {
				filter = (MeshFilter)line.AddComponent(typeof(MeshFilter));
			}

			MeshRenderer renderer = line.GetComponent<MeshRenderer> ();
			if (renderer == null) {
				renderer = (MeshRenderer)line.AddComponent(typeof(MeshRenderer));
			}

			filter.sharedMesh = feature.preloadedMeshData.ToMesh();
			line.GetComponent<Renderer>().material = renderingOptions.material;

			Vector3 position = line.transform.position;
			position.y = feature.y;

			position.y += Noise ();

			if (GOMap.GOLink && renderingOptions.polygonHeight > 0) {
				position.y -= GOFeature.BuildingElevationOffset;
			}

			line.transform.position = position;

			if (renderingOptions.outlineMaterial != null && feature.preloadedMeshData != null && feature.preloadedMeshData.secondaryMesh != null) {
				GameObject outline = RoadOutlineFromPreloaded (line, feature, renderingOptions.outlineMaterial);
				if (feature.layer.useColliders)
					outline.AddComponent<MeshCollider> ().sharedMesh = outline.GetComponent<MeshFilter> ().sharedMesh;

				outline.layer = line.layer;
				outline.tag = line.tag;

			} else if (feature.layer.useColliders) {
				line.AddComponent<MeshCollider> ();
			}
		}

		public GameObject BuildPolygonFromPreloaded(GOFeature feature)
		{

			if (feature.preloadedMeshData == null)
				return null;

			GameObject polygon = new GameObject();

			MeshFilter filter = polygon.AddComponent<MeshFilter>();
			meshRenderer = polygon.AddComponent<MeshRenderer>();

			Mesh mesh = feature.preloadedMeshData.ToMesh();

			if (feature.height > 0 && feature.layer.layerType == GOLayer.GOLayerType.Buildings)
				SimpleExtruder.FixUV (mesh, feature.preloadedMeshData.sliceHeight);
			
			filter.sharedMesh = mesh;

			if (feature.layer.useColliders && mesh != null && feature.convertedGeometry.Count () > 2)
				polygon.AddComponent<MeshCollider> ();

			return polygon;

		}

		GameObject RoadOutlineFromPreloaded (GameObject line, GOFeature feature, Material material) {


			GameObject outline = new GameObject ("outline");
			outline.transform.parent = line.transform;

			material.renderQueue = -((int)feature.sort-1);

			MeshFilter filter = outline.AddComponent<MeshFilter>();
			MeshRenderer renderer = outline.AddComponent<MeshRenderer> ();
	
			renderer.material = material;
			filter.sharedMesh = feature.preloadedMeshData.secondaryMesh.ToMesh();

			Vector3 position = outline.transform.position;
			position.y = -0.039f;
			outline.transform.localPosition = position;

			return outline;
		}

		public GameObject CreateRoofFromPreloaded (GOMesh premesh){

			GameObject roof = new GameObject();
			MeshFilter filter = roof.AddComponent<MeshFilter>();
			roof.AddComponent(typeof(MeshRenderer));
			filter.mesh = premesh.ToMesh();
			return roof;
		}


		#endregion


    }




}
