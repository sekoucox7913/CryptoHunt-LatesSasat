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
	public class GOMapboxTile : GOPBFTileAsync
	{
		public override string GetLayersStrings (GOLayer layer)
		{
			return layer.lyr();
		}
		public override string GetPoisStrings ()
		{
			return map.pois.lyr();
		}
		public override string GetPoisKindKey ()
		{
			return "type";
		}

		public override GOFeature EditFeatureData (GOFeature goFeature) {

			if (goFeature.goFeatureType == GOFeatureType.Point ){
				goFeature.name = (string)goFeature.properties ["name"];
				return goFeature;
			}

			IDictionary properties = goFeature.properties;

			if (goFeature.layer.layerType == GOLayer.GOLayerType.Roads) {

				((GORoadFeature)goFeature).isBridge = properties.Contains ("structure") && (string)properties ["structure"] == "bridge";
				((GORoadFeature)goFeature).isTunnel = properties.Contains ("structure") && (string)properties ["structure"] == "tunnel";
				((GORoadFeature)goFeature).isLink = properties.Contains ("structure") && (string)properties ["structure"] == "link";
			} 

			goFeature.kind = GOEnumUtils.MapboxToKind((string)properties["class"]);

			goFeature.name = (string)properties ["class"];

			goFeature.y = (goFeature.index / 50.0f) + goFeature.layer.defaultLayerY() /150.0f;

			goFeature.setRenderingOptions ();
			goFeature.height = goFeature.renderingOptions.polygonHeight;

			bool extrude = properties.Contains("extrude") && (string)properties["extrude"] == "true";

			if (goFeature.layer.useRealHeight && properties.Contains("height") && extrude) {
				double h =  Convert.ToDouble(properties["height"]);
				goFeature.height = (float)h;
			}

			if (goFeature.layer.useRealHeight && properties.Contains("min_height") && extrude) {
				double minHeight = Convert.ToDouble(properties["min_height"]);
				goFeature.y = (float)minHeight;
				goFeature.height = (float)goFeature.height - (float)minHeight;
			} 

			if (goFeature.height < goFeature.layer.defaultRendering.polygonHeight && goFeature.y == 0)
				goFeature.height = goFeature.layer.defaultRendering.polygonHeight;

			return goFeature;

		}

		#region NETWORK

		public override string GetTileUrl ()
		{
			var baseUrl = "https://api.mapbox.com:443/v4/mapbox.mapbox-streets-v7/";
			var extension = ".vector.pbf";

			//Download vector data
			Vector2 realPos = tileCenter.tileCoordinates (map.zoomLevel);
			var tileurl = map.zoomLevel + "/" + realPos.x + "/" + realPos.y;

			var completeUrl = baseUrl + tileurl + extension; 
//			var filename = "[MapboxVector]" + gameObject.name;

			if (map.mapbox_accessToken != null && map.mapbox_accessToken != "") {
				string u = completeUrl + "?access_token=" + map.mapbox_accessToken;
				completeUrl = u;
			}

			return completeUrl;
		}
			

		#endregion

	}
}
