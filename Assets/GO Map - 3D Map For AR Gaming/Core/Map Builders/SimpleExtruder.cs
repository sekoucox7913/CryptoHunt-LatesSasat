﻿using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine.Profiling;
using GoShared;

public class SimpleExtruder : MonoBehaviour
{
	public Vector3 normal;

	public float height;

	public SimpleExtruder (float _height)
	{
		height = _height;
	}

	void Start () {

		Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
		normal = mesh.normals [0];
		bool normalFaceDown = mesh.normals [0].y > 0 ;

		Matrix4x4 [] extrusionPath = new Matrix4x4 [2];
		extrusionPath[0] = gameObject.transform.worldToLocalMatrix * Matrix4x4.TRS(gameObject.transform.position, Quaternion.identity, Vector3.one);
		extrusionPath[1] = gameObject.transform.worldToLocalMatrix * Matrix4x4.TRS(gameObject.transform.position + new Vector3(0, height, 0), Quaternion.identity, Vector3.one);
		MeshExtrusion.ExtrudeMesh(mesh, gameObject.GetComponent<MeshFilter>().mesh, extrusionPath, false);

		mesh = Extrude (mesh, gameObject, height);

		//Check if normal are facing inside
		if (normalFaceDown) {
			gameObject.AddComponent<ReverseNormals>();
		}

	}

	public static Mesh Extrude(Mesh mesh, GameObject obj, float height) {

		bool normalFaceDown = mesh.normals [0].y > 0 ;

		Matrix4x4 [] extrusionPath = new Matrix4x4 [2];
		Matrix4x4 a = obj.transform.worldToLocalMatrix * Matrix4x4.TRS(obj.transform.position, Quaternion.identity, Vector3.one);
		Matrix4x4 b = obj.transform.worldToLocalMatrix * Matrix4x4.TRS(obj.transform.position + new Vector3(0, height, 0), Quaternion.identity, Vector3.one);

		//		Check if normal are facing inside
		if (!normalFaceDown) {
			extrusionPath [0] = a;
			extrusionPath [1] = b;
		} else {   //This is the base case
			extrusionPath [0] = b;
			extrusionPath [1] = a;
		}

		MeshExtrusion.ExtrudeMesh(mesh, mesh, extrusionPath, false);

//		FixUV (mesh, height);

		return mesh;
	}

	public static Mesh SliceExtrude(Mesh mesh, GameObject obj, float height, float topSectionH, float bottomSectionH, float sliceHeight) {

//		bool normalFaceDown = mesh.normals [0].y > 0 ;

		if (height < sliceHeight)
			return Extrude (mesh, obj, height);

		int numberOfSlices = (int) Mathf.Ceil((height) / sliceHeight);
		sliceHeight = height / numberOfSlices;

		List<Matrix4x4> extrusion = new List<Matrix4x4> ();
		for (int i = numberOfSlices; i > -1; i --) {

			Vector3 pos = obj.transform.position;
			pos.y = (i-1) * sliceHeight;
			Matrix4x4 mat = obj.transform.worldToLocalMatrix * Matrix4x4.TRS(pos + new Vector3(0, sliceHeight, 0), Quaternion.identity, Vector3.one);
			extrusion.Add (mat);
		}
			
		Profiler.BeginSample("[GoMap] extrudemesh");
		MeshExtrusion.ExtrudeMesh(mesh, mesh, extrusion.ToArray(), false);
		Profiler.EndSample ();

		Profiler.BeginSample("[GoMap] FixUv extruded");
//		FixUV (mesh,sliceHeight);
		Profiler.EndSample ();


//		//TODO: TEST
//		ChangeUV changeUV = obj.AddComponent<ChangeUV>();
//		changeUV.sliceH = sliceHeight;
//		changeUV.numberOfSlices = numberOfSlices;

		return mesh;
	}
		

	public static void FixUV(Mesh mesh, float sliceH)
	{
		Profiler.BeginSample ("[GOFeature] fix UV");
		var newUvs = new Vector2[mesh.vertices.Length];

		Vector3 clippedPart = Vector3.zero;
		Vector3[] vertices = mesh.vertices;
		Vector3[] normals = mesh.normals;
		Vector3 vertex;
		Vector3 normal;

		for (var v = 0; v < vertices.Length; v++){

			vertex = vertices[v];
			normal = normals[v];


			float textureSlice = 0.25f; //a quarter of image
			float sliceHOriginal = 10f;
			clippedPart.x = textureSlice * (sliceHOriginal / sliceH) /10; 

			// This gives a vector pointing up the roof:
			Vector3 vAxis = Vector3.Scale(normal, new Vector3(-1, 0, -1)).normalized;
			// This will make the u axis perpendicular to the v axis (ie. parallel to the roof edge)
			Vector3 uAxis = new Vector3(vAxis.z, 0, -vAxis.x) ;

			// I originally used vAxis here, but changed to position.y so you get more predticable alignment at edges.
			// Set eaveHeight to the y coordinate of the bottom edge of the roof.
			//			var uv = new Vector2(Vector3.Dot(vertex, uAxis), vertex.y) ;
			Vector2 uv = new Vector2(Vector3.Dot(vertex, uAxis)* clippedPart.x + clippedPart.z, vertex.y * clippedPart.x + clippedPart.y) ;
			//var uv = new Vector2(Vector3.Dot(vertex, uAxis), vertex.y * clippedPart.x + clippedPart.y) ;

			newUvs[v] = uv;

			// You may need to scale the uv vector's x and y to get the aspect ratio you want.
			// The scale factor will vary with the roof's slope.
		}


		mesh.uv = newUvs;

		Profiler.EndSample ();
	}

	#region Premesh

	public static GOMesh ExtrudePremesh(GOMesh mesh, float height) {

		Matrix4x4 [] extrusionPath = new Matrix4x4 [2];
		Matrix4x4 a =  Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
		Matrix4x4 b =  Matrix4x4.TRS(Vector3.zero + new Vector3(0, height, 0), Quaternion.identity, Vector3.one);

		extrusionPath [0] = b;
		extrusionPath [1] = a;

		GOMeshExtrusion.ExtrudeMesh(mesh, mesh, extrusionPath, false);

		mesh.sliceHeight = height;

//		FixPremeshUV (mesh,height);

		return mesh;
	}


	public static GOMesh SliceExtrudePremesh(GOMesh mesh, float height, float topSectionH, float bottomSectionH, float sliceHeight) {

		if (height < sliceHeight)
			return ExtrudePremesh (mesh, height);

		int numberOfSlices = (int) Mathf.Ceil((height) / sliceHeight);
		sliceHeight = height / numberOfSlices;

		List<Matrix4x4> extrusion = new List<Matrix4x4> ();
		for (int i = numberOfSlices; i > -1; i --) {

			Vector3 pos = Vector3.zero;
			pos.y = (i-1) * sliceHeight;
			Matrix4x4 mat = Matrix4x4.TRS(pos + new Vector3(0, sliceHeight, 0), Quaternion.identity, Vector3.one);
			extrusion.Add (mat);
		}

		mesh.sliceHeight = sliceHeight;

		GOMeshExtrusion.ExtrudeMesh(mesh, mesh, extrusion.ToArray(), false);

//		FixPremeshUV (mesh,height);

		return mesh;
	}

	private static void FixPremeshUV(GOMesh mesh, float sliceH)
	{


		var newUvs = new Vector2[mesh.vertices.Length];

//		Debug.Log (sliceH);

		Vector3 clippedPart = Vector3.zero;
		Vector3[] vertices = mesh.vertices;
		Vector3 vertex;
		Vector3 normal;

		for (var v = 0; v < vertices.Length; v++){

			vertex = vertices[v];
//			normal = new Vector3 (0.1f,0,0.1f);


			float textureSlice = 0.25f; //a quarter of image
			float sliceHOriginal = 10f;
			clippedPart.x = textureSlice * (sliceHOriginal / sliceH); 

			// This gives a vector pointing up the roof:
			//Vector3 vAxis = Vector3.Scale(normal, new Vector3(-1, 0, -1)).normalized;
			Vector3 vAxisProto = Vector3.Scale(vertex, new Vector3(1, 1, 1));

			// This will make the u axis perpendicular to the v axis (ie. parallel to the roof edge)
			Vector3 uAxis = new Vector3(vAxisProto.z, 0, -vAxisProto.x) ;

			// I originally used vAxis here, but changed to position.y so you get more predticable alignment at edges.
			// Set eaveHeight to the y coordinate of the bottom edge of the roof.
			//			var uv = new Vector2(Vector3.Dot(vertex, uAxis), vertex.y) ;
			Vector2 uv = new Vector2(Vector3.Dot(vertex, vAxisProto)* clippedPart.x + clippedPart.z, vertex.y * clippedPart.x + clippedPart.y) ;
			//var uv = new Vector2(Vector3.Dot(vertex, uAxis), vertex.y * clippedPart.x + clippedPart.y) ;

			newUvs[v] = uv;

		}


		mesh.uv = newUvs;
	}

	#endregion
		
}

