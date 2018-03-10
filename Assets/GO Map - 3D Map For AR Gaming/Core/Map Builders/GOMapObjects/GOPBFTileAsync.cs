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
	public abstract class GOPBFTileAsync : GOTile
	{

		public VectorTile vt;

		//THis method is called on a background thread
		public abstract GOFeature EditFeatureData (GOFeature feature);
		//THis method is called on a background thread
		public abstract string GetLayersStrings (GOLayer layer);
		//THis method is called on a background thread
		public abstract string GetPoisStrings ();
		//THis method is called on a background thread
		public abstract string GetPoisKindKey ();


		public override IEnumerator BuildTile(IDictionary mapData, GOLayer layer, bool delayedLoad)
		{ 
			yield return null;
		}

		public override IEnumerator ParseTileData(object m, Coordinates tilecenter, int zoom, GOLayer[] layers, bool delayedLoad, List <string> layerNames)
		{

			if (mapData == null) {
				Debug.LogWarning ("Map Data is null! ASYNC tileurl: "+this.tileUrl);
				#if !UNITY_WEBPLAYER
				FileHandler.Remove (gameObject.name);
				#endif
				yield break;
			}
			float t0 = Time.time;

			GOPbfProcedure procedure = new GOPbfProcedure();
			procedure.tileData = (byte[])mapData;
			procedure.zoomlevel = map.zoomLevel;
			procedure.tileCoords = tileCoordinates;
			procedure.layers = layers;
			procedure.tile = this;
			procedure.Start();

			if (Application.isPlaying) { //Runtime build
				
				yield return StartCoroutine (procedure.WaitFor ());

				float t1 = Time.time;

				foreach (GOParsedLayer p in procedure.list) {
					yield return StartCoroutine (BuildLayer (p, delayedLoad));
				}

//				Debug.Log ("time parsing: " + (t1 - t0) + "  Total time: " + (Time.time - t0));
			} 
			else { //In editor build

				while(!procedure.Update())
				{
					
				}
			}

			Complete = true;

			yield return null;
		}

		public IEnumerator BuildLayer(GOParsedLayer parsedLayer, bool delayedLoad)
		{
			
			GOLayer goLayer = parsedLayer.goLayer;

			Profiler.BeginSample("[GoMap] [BuildLayer] game object");
			GameObject parent = null;
			parent = new GameObject ();
			parent.name = parsedLayer.name;
			parent.transform.parent = this.transform;
			if (parsedLayer.goLayer != null)
				parent.SetActive (!goLayer.startInactive);
			else {
				parent.SetActive (!map.pois.startInactive);
			}
			
			Profiler.EndSample ();

			int  featureCount = parsedLayer.goFeatures.Count;

			if (featureCount == 0)
				yield break;

			IList iList = new List<GOFeature> ();
			for (int i = 0; i < featureCount; i++) {

				GOFeature goFeature = (GOFeature)parsedLayer.goFeatures [i];

				if (goFeature.goFeatureType == GOFeatureType.Undefined || goFeature.goFeatureType == GOFeatureType.MultiPoint) {
					continue;
				}

				if (goFeature.goFeatureType == GOFeatureType.Point){ //POIS 
					goFeature.parent = parent;
					iList.Add (goFeature);
					continue;
				}

				if (goLayer.useOnly.Length > 0 && !goLayer.useOnly.Contains (goFeature.kind)) {
					continue;
				}
				if (goLayer.avoid.Length > 0 && goLayer.avoid.Contains (goFeature.kind)) {
					continue;
				}

				if (goLayer.layerType == GOLayer.GOLayerType.Roads) {

					if (goFeature.goFeatureType != GOFeatureType.Line && goFeature.goFeatureType != GOFeatureType.MultiLine)
						continue;

					GORoadFeature grf = (GORoadFeature)goFeature;
					if ((grf.isBridge && !goLayer.useBridges) || (grf.isTunnel && !goLayer.useTunnels) || (grf.isLink && !goLayer.useBridges)) {
						continue;
					}
				}

				goFeature.parent = parent;

				iList.Add (goFeature);

			}

//			Profiler.BeginSample("[GoMap] [BuildLayer] merge roads");
//			if (goLayer.layerType == GOLayer.GOLayerType.Roads) {
//				iList = GORoadFeature.MergeRoads (iList);
//			}
//			Profiler.EndSample ();

			int n = 15;
			for (int i = 0; i < iList.Count; i+=n) {

				for (int k = 0; k<n; k++) {
					if (i + k >= iList.Count) {
						yield return null;
						break;
					}

					GOFeature r = (GOFeature)iList [i + k];
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
//				
			yield return null;
		}
			
		#if UNITY_EDITOR
		//This is called only in the editor mode.
		public void OnProcedureComplete (GOPbfProcedure procedure) {

			if (!Application.isPlaying) {
				foreach (GOParsedLayer p in procedure.list) {
					GORoutine.start (BuildLayer (p, false), this);
				}
			}
		}

		#endif
	}
}
