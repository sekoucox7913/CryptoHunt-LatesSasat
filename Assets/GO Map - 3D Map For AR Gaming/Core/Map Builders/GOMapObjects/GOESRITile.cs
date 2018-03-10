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
	public class GOEsriTIle : GOPBFTileAsync
	{

		public override string GetLayersStrings (GOLayer layer)
		{
			return layer.lyr_esri();
		}

		public override string GetPoisStrings ()
		{
			return "";
		}

		public override string GetPoisKindKey ()
		{
			return "";
		}

		public override GOFeature EditFeatureData (GOFeature goFeature) {

			IDictionary properties = goFeature.properties;

			if (goFeature.goFeatureType == GOFeatureType.Point ){
				goFeature.name = (string)goFeature.properties ["name"];
				return goFeature;
			}

			goFeature.kind = GOEnumUtils.MapboxToKind(goFeature.layer.name);

			goFeature.y = goFeature.layer.defaultLayerY();
			if (properties.Contains ("_symbol"))
				goFeature.y = Convert.ToInt64 (properties ["_symbol"]) / 10.0f;

			goFeature.height = goFeature.layer.defaultRendering.polygonHeight;

			return goFeature;

		}

		#region NETWORK

		public override string GetTileUrl ()
		{
			var baseUrl = "https://basemaps.arcgis.com/v1/arcgis/rest/services/World_Basemap/VectorTileServer/tile/";
			var extension = ".pbf";

			//Download vector data
			Vector2 realPos = tileCenter.tileCoordinates (map.zoomLevel);
			var tileurl = map.zoomLevel + "/" + realPos.y + "/" + realPos.x; //of course Esri uses inverted tile x,y. =/
			var completeUrl = baseUrl + tileurl + extension; 
//			var filename = "[ESRIVector]" + gameObject.name;

			return completeUrl;
		}
			

		#endregion

	}
}
