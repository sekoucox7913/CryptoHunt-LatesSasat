using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.Events;
using System.Linq;

using GoShared;

#if GOLINK
//[GOLINK] GOTerrain link (This requires GOTerrain! https://www.assetstore.unity3d.com/#!/content/84198) 
using GoTerrain;
#endif

namespace GoMap
{
	[ExecuteInEditMode]
	[System.Serializable]
	public class GOMap : MonoBehaviour
	{

		#if GOLINK
		//[GOLINK] GOTerrain link (This requires GOTerrain! https://www.assetstore.unity3d.com/#!/content/84198) 
		[HideInInspector] public GOTerrain goTerrain;
		#endif
		public static bool GOLink = false;

		public LocationManager locationManager;
		[Range (0, 8)]  public int tileBuffer = 2;
		[ShowOnly] public int zoomLevel = 0;
		public bool useCache = true;

		[InspectorButton ("ClearCache")] public bool clearCache;

		[Regex (@"^(?!\s*$).+", "Please insert your Mapzen API key")] public string mapzen_api_key = "";
		[Regex (@"^(?!\s*$).+", "Please insert your MapBox Access Token")]public string mapbox_accessToken = "";
		[Regex (@"^(?!\s*$).+", "Please insert your OSM API key")]public string osm_api_key = "";

		public GOMapType mapType = GOMapType.MapzenJson;

		public enum GOMapType
		{
			
			Mapzen,
			Mapbox,
			OSM,
			MapzenJson,
			Esri
		}

		public Material tileBackground;
		[HideInInspector] public GameObject tempTileBackgorund;
		public GOLayer[] layers;
		public GOPOILayer pois;

		[HideInInspector]
		public bool dynamicLoad = true;

		public Shader textShader;
		public Color streetnameColor = new Color (61 / 255.0f, 61 / 255.0f, 83 / 255f);

		public GOTileEvent OnTileLoad;

		Vector2 Center_tileCoords;

		List <GOTile> tiles = new List<GOTile> ();

		//features ID
		[HideInInspector]public IList buildingsIds = new List<object> ();
		[HideInInspector]public  List <Vector3> buildingCenters = new List<Vector3> ();


		void Awake ()
		{

			locationManager.onOriginSet.AddListener ((Coordinates) => {
				OnOriginSet (Coordinates);
			});
			locationManager.onLocationChanged.AddListener ((Coordinates) => {
				OnLocationChanged (Coordinates);
			});

			GOLink = false;
			#if GOLINK
			//[GOLINK] GOTerrain link (This requires GOTerrain! https://www.assetstore.unity3d.com/#!/content/84198) 
			goTerrain = FindObjectOfType<GOTerrain>();
			if (goTerrain != null) {
				dynamicLoad = false;
			}
			GOLink = goTerrain != null;
			#endif
			//
			
			zoomLevel = locationManager.zoomLevel;

			if (mapType == GOMapType.OSM && locationManager.zoomLevel > 14) {
				Debug.LogWarning ("[GOMap] Zoom level " + zoomLevel + "is not available with this map provider. Zoom level 14 is set.");
				zoomLevel = 14;
				tileBuffer = tileBuffer == 0 ? 0 : tileBuffer - 1;
			}

			if (zoomLevel == 0) {
				zoomLevel = locationManager.zoomLevel;	
			}

			if (mapType == (GOMapType.MapzenJson | GOMapType.Mapzen) && (mapzen_api_key == null || mapzen_api_key == "")) {
				Debug.LogWarning ("[GOMap] Mapzen api key is missing, GET iT HERE: https://mapzen.com/developers");
				return;
			} else if (mapType == GOMapType.Mapbox && (mapbox_accessToken == null || mapbox_accessToken == "")) {
				Debug.LogWarning ("[GOMap] Mapbox access token is missing, GET IT HERE: https://www.mapbox.com/help/create-api-access-token/");
				return;
			} else if (mapType == GOMapType.OSM && (osm_api_key == null || osm_api_key == "")) {
				Debug.LogWarning ("[GOMap] Open Street Maps api key is missing, GET IT HERE: https://openmaptiles.com/hosting/"); 
				return;
			}

			#if UNITY_WEBPLAYER
				Debug.LogError ("GOMap is NOT supported in the webplayer! Please switch platform in the build settings window.");
			#endif		

			if (tileBackground != null && Application.isPlaying) {
				CreateTemporaryMapBackground ();
			}
		}


		#region Location Manager Events

		void OnLocationChanged (Coordinates currentLocation)
		{
			StartCoroutine (ReloadMap (currentLocation, true));
		}

		void OnOriginSet (Coordinates currentLocation)
		{

			DestroyCurrentMap ();

			if (tileBackground != null /*&& Application.isMobilePlatform*/) {
				DestroyTemporaryMapBackground ();
			}
			StartCoroutine (ReloadMap (currentLocation, false));
		}

		#endregion

		#region GoMap Load

		public IEnumerator ReloadMap (Coordinates location, bool delayed)
		{

			if (!dynamicLoad) {
				yield break;
			}
				
			//Get SmartTiles
			List <Vector2> tileList = location.adiacentNTiles (zoomLevel, tileBuffer);
				
			List <GOTile> newTiles = new List<GOTile> ();

			// Create new tiles
			foreach (Vector2 tileCoords in tileList) {

				if (!isSmartTileAlreadyCreated (tileCoords, zoomLevel)) {

					GOTile adiacentSmartTile = createSmartTileObject (tileCoords, zoomLevel);
					adiacentSmartTile.tileCenter = new Coordinates (tileCoords, zoomLevel);
					adiacentSmartTile.diagonalLenght = adiacentSmartTile.tileCenter.diagonalLenght (zoomLevel);
					adiacentSmartTile.gameObject.transform.position = adiacentSmartTile.tileCenter.convertCoordinateToVector ();

					newTiles.Add (adiacentSmartTile);

					if (tileBackground != null) {
						CreateTileBackground (adiacentSmartTile);
					}
				}
			}

			foreach (GOTile tile in newTiles) {


				if (OnTileLoad != null) {
					OnTileLoad.Invoke (tile);
				}

				#if !UNITY_WEBPLAYER
//				if (tile != null && FileHandler.Exist (tile.gameObject.name) && useCache) {

				if (Application.isPlaying)
					yield return tile.StartCoroutine (tile.LoadTileData (this, tile.tileCenter, zoomLevel, layers, delayed));
				else
					GORoutine.start (tile.LoadTileData (this, tile.tileCenter, zoomLevel, layers, delayed), tile);
//				} else {
//					tile.StartCoroutine(tile.LoadTileData(this, tile.tileCenter, zoomLevel,layers,delayed));
//				}
	
	
				#endif
			}
				
			//Destroy far tiles
			List <Vector2> tileListForDestroy = location.adiacentNTiles (zoomLevel, tileBuffer + 1);
			yield return StartCoroutine (DestroyTiles (tileListForDestroy));

		}

		public List <string> layerNames ()
		{
		
			List <string> layerNames = new List<string> ();
			for (int i = 0; i < layers.ToList ().Count; i++) {
				if (layers [i].disabled == false) {
					layerNames.Add (layers [i].json ());
				}
			}
			return layerNames;
		}

		public IEnumerator DestroyTiles (List <Vector2> list)
		{

			try {
				List <string> tileListNames = new List <string> ();
				foreach (Vector2 v in list) {
					string s = tilenameFormat (v);
					tileListNames.Add (s);
//					Debug.Log ("Tile list name: "+s);
				}

				List <GOTile> toDestroy = new List<GOTile> ();
				foreach (GOTile tile in tiles) {
					if (!tileListNames.Contains (tile.name)) {
						toDestroy.Add (tile);
					}
				}
				for (int i = 0; i < toDestroy.Count; i++) {

					GOTile tile = toDestroy [i];
					tiles.Remove (tile);
					GameObject.Destroy (tile.gameObject, i);
				}
			} catch {
				
			}

			yield return null;
		}

		bool isSmartTileAlreadyCreated (Vector2 tileCoords, int Zoom)
		{

			string name = tilenameFormat (tileCoords);
			return transform.Find (name);
		}

		GOTile createSmartTileObject (Vector2 tileCoords, int Zoom)
		{

			GameObject tileObj = new GameObject (tilenameFormat (tileCoords));
			tileObj.transform.parent = gameObject.transform;
			GOTile tile;

			if (mapType == GOMapType.Mapbox) {
				tile = tileObj.AddComponent<GOMapboxTile> ();
			} else if (mapType == GOMapType.MapzenJson) {
				tile = tileObj.AddComponent<GOMapzenTile> ();
			} else if (mapType == GOMapType.Mapzen) {
				tile = tileObj.AddComponent<GOMapzenProtoTile> ();
			} else if (mapType == GOMapType.OSM) {
				tile = tileObj.AddComponent<GOOSMTile> ();
			} else if (mapType == GOMapType.Esri) {
				tile = tileObj.AddComponent<GOEsriTIle> ();
			} else {
				tile = tileObj.AddComponent<GOTile> ();
			}

			tiles.Add (tile);
			return tile;
		}

		private string tilenameFormat (Vector2 tileCoords)
		{
			return tileCoords.x + "-" + tileCoords.y + "-" + zoomLevel;
		}

		void OnApplicationQuit ()
		{
			tiles.Clear ();
		}

		#endregion

		#region Utils

		public void dropPin (double lat, double lng, GameObject go)
		{

			Coordinates coordinates = new Coordinates (lat, lng, 0);
			go.transform.localPosition = coordinates.convertCoordinateToVector (0);

		}

		public void DestroyCurrentMap ()
		{
		
			while (transform.childCount > 0) {
				foreach (Transform child in transform) {
					GameObject.DestroyImmediate (child.gameObject);
				}
			}

			GOEnvironment env = GameObject.FindObjectOfType<GOEnvironment> ();
			if (env == null) {
				return;
			}

			while (env.transform.childCount > 0) {
				foreach (Transform child in env.transform) {
					GameObject.DestroyImmediate (child.gameObject);
				}
			}
		}

		#endregion

		#region Tile Background

		private void CreateTileBackground (GOTile tile)
		{

			MeshFilter filter = tile.gameObject.AddComponent<MeshFilter> ();
			MeshRenderer renderer = tile.gameObject.AddComponent<MeshRenderer> ();

			tile.vertices = new List<Vector3> ();
			IList verts = tile.tileCenter.tileVertices (zoomLevel);
			foreach (Vector3 v in verts) {
				tile.vertices.Add (tile.transform.InverseTransformPoint (v));
			}

			Poly2Mesh.Polygon poly = new Poly2Mesh.Polygon ();
			poly.outside = tile.vertices;
			Mesh mesh = Poly2Mesh.CreateMesh (poly);

			Vector3[] meshverts = mesh.vertices;
			Vector2[] uvs = new Vector2[meshverts.Length];
			for (int i = 0; i < uvs.Length; i++) {
				uvs [i] = new Vector2 (meshverts [i].x, meshverts [i].z);
			}
			mesh.uv = uvs;

			filter.sharedMesh = mesh;
			tile.gameObject.AddComponent<MeshCollider> ();

			renderer.material = tileBackground;

		}

		private void CreateTemporaryMapBackground ()
		{

			tempTileBackgorund = new GameObject ("Temporary tile background");

			MeshFilter filter = tempTileBackgorund.AddComponent<MeshFilter> ();
			MeshRenderer renderer = tempTileBackgorund.AddComponent<MeshRenderer> ();

			float size = 1000;

			Poly2Mesh.Polygon poly = new Poly2Mesh.Polygon ();
			poly.outside = new List<Vector3> {
				new Vector3 (size, -0.1f, size),
				new Vector3 (size, -0.1f, -size),
				new Vector3 (-size, -0.1f, -size),
				new Vector3 (-size, -0.1f, size)
			};
			Mesh mesh = Poly2Mesh.CreateMesh (poly);

			Vector2[] uvs = new Vector2[mesh.vertices.Length];
			for (int i = 0; i < uvs.Length; i++) {
				uvs [i] = new Vector2 (mesh.vertices [i].x, mesh.vertices [i].z);
			}
			mesh.uv = uvs;

			filter.sharedMesh = mesh;
			renderer.material = tileBackground;

			tempTileBackgorund.AddComponent<MeshCollider> ();
		
		}

		private void DestroyTemporaryMapBackground ()
		{
			GameObject.DestroyImmediate (tempTileBackgorund);
			tempTileBackgorund = null;
		}

		#endregion

		#region GoTerrain Link

		public IEnumerator createTileWithPreloadedData (Vector2 tileCoords, int Zoom, bool delayedLoad, object mapData)
		{

			#if GOLINK
			switch (goTerrain.goTerrainOptions.vectorAPI) {
			case  GOTerrainOptions.VectorAPI.MapzenJSON:
					mapType = GOMapType.MapzenJson;
					break;
			case  GOTerrainOptions.VectorAPI.Mapzen:
					mapType = GOMapType.Mapzen;
					break;
			case  GOTerrainOptions.VectorAPI.Mapbox:
					mapType = GOMapType.Mapbox;
					break;
			}
			#endif

			if (!isSmartTileAlreadyCreated (tileCoords, zoomLevel)) {

				GOTile goTile = createSmartTileObject (tileCoords, zoomLevel);
				goTile.tileCoordinates = tileCoords;
				goTile.tileCenter = new Coordinates (tileCoords, zoomLevel);
				goTile.diagonalLenght = goTile.tileCenter.diagonalLenght (zoomLevel);
				goTile.gameObject.transform.position = goTile.tileCenter.convertCoordinateToVector ();
				goTile.mapData = mapData;
				goTile.map = this;

				if (tileBackground != null) {
					CreateTileBackground (goTile);
				}

				if (OnTileLoad != null) {
					OnTileLoad.Invoke (goTile);
				}

				yield return goTile.StartCoroutine (goTile.ParseTileData (this, goTile.tileCenter, Zoom, layers, delayedLoad, layerNames ()));

			}
			yield return null;
		}

		#endregion

		#region CacheControl

		public void ClearCache ()
		{
			FileHandler.ClearEverythingInFolder (FileHandler.GoCachePath ());
		}

		#endregion

		#region Editor Map Builder

		public void BuildInsideEditor ()
		{

			dynamicLoad = true;
			//This fixes the map origin
			locationManager.LoadDemoLocation ();

			//Wipe buildings id list
			buildingsIds = new List<object> ();

			//Start load routine (This might take some time...)
			if (locationManager.demo_CenterWorldCoordinates == null) {
				Debug.LogWarning ("[GOMap Editor] Warning, check if you have \"No GPS Test\" set on the Location Manager.");
				return;
			}

			IEnumerator routine = ReloadMap (locationManager.demo_CenterWorldCoordinates.tileCenter (locationManager.zoomLevel), false);
			GORoutine.start (routine, this);

		}

		public void TestEditorWWW ()
		{
			#if UNITY_EDITOR
			var www = new WWW ("https://tile.mapzen.com/mapzen/vector/v1//buildings,landuse,water,roads/17/70076/48701.json");

			ContinuationManager.Add (() => www.isDone, () => {
				if (!string.IsNullOrEmpty (www.error))
					Debug.Log ("WWW failed: " + www.error);
				Debug.Log ("[GOMap Editor] Request success: " + www.text);
			});
			#endif
		}

		#endregion
	}




	#region Events

	[Serializable]
	public class GOFeatureEvent : UnityEvent <GOFeature>
	{


	}

	[Serializable]
	public class GOEvent : UnityEvent <Mesh,GOLayer,GOFeatureKind,Vector3>
	{


	}

	[Serializable]
	public class GOTileEvent : UnityEvent <GOTile>
	{


	}
	#endregion
}