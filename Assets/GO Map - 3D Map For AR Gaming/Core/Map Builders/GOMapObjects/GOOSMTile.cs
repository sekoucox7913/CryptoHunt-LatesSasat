using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

using GoShared;
using Mapbox.VectorTile;

namespace GoMap
{
	[ExecuteInEditMode]
	public class GOOSMTile : GOPBFTileAsync
	{

		public override string GetLayersStrings (GOLayer layer)
		{
			return layer.lyr_osm ();
		}
		public override string GetPoisStrings ()
		{
			return map.pois.lyr_osm();
		}
		public override string GetPoisKindKey ()
		{
			return "class";
		}

		public override GOFeature EditFeatureData (GOFeature goFeature) {

			IDictionary properties = goFeature.properties;

			if (goFeature.goFeatureType == GOFeatureType.Point){
				goFeature.name = (string)goFeature.properties ["name"];
				return goFeature;
			}

			if (goFeature.layer.layerType == GOLayer.GOLayerType.Roads) {
				((GORoadFeature)goFeature).isBridge = properties.Contains ("brunnel") && (string)properties ["brunnel"] == "bridge";
				((GORoadFeature)goFeature).isTunnel = properties.Contains ("brunnel") && (string)properties ["brunnel"] == "tunnel";
				((GORoadFeature)goFeature).isLink = properties.Contains ("brunnel") && (string)properties ["brunnel"] == "link";
			} 

			goFeature.kind = GOEnumUtils.MapboxToKind((string)properties["class"]);

			goFeature.y = goFeature.index/1000 + goFeature.layer.defaultLayerY();

			if (goFeature.kind == GOFeatureKind.lake) //terrible fix for vector maps without a sort value.
				goFeature.y = goFeature.layer.defaultLayerY (GOLayer.GOLayerType.Landuse)+0.1f;

			goFeature.setRenderingOptions ();
			goFeature.height = goFeature.renderingOptions.polygonHeight;

			if (goFeature.layer.useRealHeight && properties.Contains("render_height")) {
				double h =  Convert.ToDouble(properties["render_height"]);
				goFeature.height = (float)h;
			}

			if (goFeature.layer.useRealHeight && properties.Contains("render_min_height")) {
				double hm = Convert.ToDouble(properties["render_min_height"]);
				goFeature.y = (float)hm;
				if (goFeature.height >= hm) {
					goFeature.y = (float)hm;
					goFeature.height = (float)goFeature.height - (float)hm;
				}
			} 

			return goFeature;
		}

		#region NETWORK

		public override string GetTileUrl ()
		{
			var baseUrl = "https://free-0.tilehosting.com/data/v3/";
			var extension = ".pbf.pict";

			//Download vector data
			Vector2 realPos = tileCenter.tileCoordinates (map.zoomLevel);
			var tileurl = map.zoomLevel + "/" + realPos.x + "/" + realPos.y;

			var completeUrl = baseUrl + tileurl + extension; 
//			var filename = "[OSMVector]" + gameObject.name;

			if (map.osm_api_key != null && map.osm_api_key != "") {
				string u = completeUrl + "?key=" + map.osm_api_key;
				completeUrl = u;
			}

			return completeUrl;
		}

		#endregion

	}
}
