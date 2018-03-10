using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoShared {

	[System.Serializable]
	public class GODEMTexture2D {

		public Color32[] arcolors;
		public float height;
		public float width;
		public byte[] data;

		Vector2 tileSize = Vector3.zero;
		float altitudeMultiplier = 1;
		public GOElevationAPI elevationAPI;

		public Texture2D texture;

		#region Constructors

		public GODEMTexture2D (byte[] b, Vector2 tileSize, GOElevationAPI elevationAPI, float altitudeMultiplier = 1) {

			this.data = b;
			this.tileSize = tileSize;
			this.elevationAPI = elevationAPI;
			this.altitudeMultiplier = altitudeMultiplier;

			this.texture = new Texture2D (256, 256);
			this.texture.wrapMode = TextureWrapMode.Clamp;
			this.texture.LoadImage (data);
	
			this.width = texture.width;
			this.height = texture.height;
			this.arcolors = texture.GetPixels32();
		} 

		public GODEMTexture2D (Texture2D t, Vector2 tileSize,  GOElevationAPI elevationAPI, float altitudeMultiplier = 1) {

			this.texture = t;
			this.tileSize = tileSize;
			this.elevationAPI = elevationAPI;
			this.width = t.width;
			this.height = t.height;
			this.arcolors = t.GetPixels32();
			this.altitudeMultiplier = altitudeMultiplier;
		} 

		#endregion

		#region Output

		public Texture2D ToTexture2D () {

			texture = new Texture2D (256, 256);
			texture.wrapMode = TextureWrapMode.Clamp;
			texture.LoadImage (data);

			return texture;
		}

		#endregion

		#region Utils

		public Color32 calculateColor (Vector2 position, Vector2 offset, Vector2 resolution){

			Color32 color = Color.black;

			try {

				int xOff =  Mathf.FloorToInt(position.x * width/resolution.x);
				int zOff =  Mathf.FloorToInt(position.y * height/resolution.y);
				if (xOff > 255)	xOff = 255;
				if (zOff > 255)	zOff = 255;

				int colorFormula = xOff + (int)offset.x + (int)width * (zOff + (int)offset.y);

				color = arcolors[colorFormula];

			} catch {
				int positionOverflowX = (int)position.x;
				int positionOverflowY = (int)position.y;

				if (positionOverflowX > resolution.x) {
					positionOverflowX = (int)resolution.x;
				}
				if (positionOverflowY > resolution.y) {
					positionOverflowY = (int)resolution.y;
				}
				int xOff =  Mathf.FloorToInt(positionOverflowX* width/resolution.x);
				int zOff =  Mathf.FloorToInt(positionOverflowY* height/resolution.y);
				if (xOff > 255)	xOff = 255;
				if (zOff > 255)	zOff = 255;

				int colorFormula = xOff + (int)offset.x + (int)width * (zOff + (int)offset.y);

				color = arcolors[colorFormula];

			}
			return color;
		}

		public float ConvertColorToAltitude (Color32 c32) {

			switch (elevationAPI) {

			case GOElevationAPI.Mapzen :
				return (c32.r * 256.0f + c32.g + c32.b / 256.0f) - 32768.0f;
			case GOElevationAPI.Mapbox :
				return -10000.0f + ((c32.r * 256.0f * 256 + c32.g * 256 + c32.b)*0.1f);
			}

			return 0;
		}

		public float FindAltitudeForVector (Vector3 inputVector, Texture2D tex, Vector3 tileOriginVector) {

			if (tex == null)
				return 0;

			float stepSizeWidth = tileSize.x;
			float stepSizeHeight = tileSize.y;

			//Move the tile origin to match the DEM origin
			Vector2 fixedOrigin = tileOriginVector.ToVector2xz() - new Vector2 (0,tileSize.y);

			float x = 0;
			float z = 0;

			//Convert coords to unity 
			Vector2 input = inputVector;

			//Compute the distance for each axis
			x = input.x - fixedOrigin.x;
			z = input.y - fixedOrigin.y;

			//Adapt the values to the stepsize
			x = tex.width * x / stepSizeWidth;
			z = tex.height * z / stepSizeHeight;

			Color32 c32 = calculateColor (new Vector2 (x, z), Vector2.zero, new Vector2(tex.width,tex.height));

			float height = ConvertColorToAltitude(c32);
			height = height * altitudeMultiplier;

			return height;

		}

		public float FindAltitudeForCoordinate (Coordinates inputCoordinate, Coordinates tileOrigin) {

			float stepSizeWidth = tileSize.x;
			float stepSizeHeight = tileSize.y;

			//Move the tile origin to match the DEM origin
			Vector2 fixedOrigin = tileOrigin.convertCoordinateToVector2D () - new Vector2 (0,tileSize.y);

			float x = 0;
			float z = 0;

			//Convert coords to unity 
			Vector2 input = inputCoordinate.convertCoordinateToVector2D ();

			//Compute the distance for each axis
			x = input.x - fixedOrigin.x;
			z = input.y - fixedOrigin.y;

			//Adapt the values to the stepsize
			x = width * x / stepSizeWidth;
			z = height * z / stepSizeHeight;

			Color32 c32 = calculateColor (new Vector2 (x, z), Vector2.zero, new Vector2(width,this.height));

			float h = ConvertColorToAltitude (c32);
			h = h * altitudeMultiplier;

			return h;

		}
			
		#endregion
	}

}
