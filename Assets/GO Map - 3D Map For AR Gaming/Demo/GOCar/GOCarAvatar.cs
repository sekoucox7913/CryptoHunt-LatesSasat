using UnityEngine;
using System.Collections;
using GoMap;

using GoShared;
using System;
using UnityEngine.Events;


public class GOCarAvatar : MonoBehaviour {

	public LocationManager locationManager;
	public GameObject avatarFigure;
	public bool autoDrive = false;

	// Use this for initialization
	void Start () {

		locationManager.onOriginSet.AddListener((Coordinates) => {OnOriginSet(Coordinates);});
//		locationManager.onLocationChanged.AddListener((Coordinates) => {OnLocationChanged(Coordinates);});
	}

	void OnOriginSet (Coordinates currentLocation) {

		//Position
		Vector3 currentPosition = currentLocation.convertCoordinateToVector ();
		currentPosition.y = transform.position.y;

		transform.position = currentPosition;

	}

	void OnLocationChanged (Coordinates currentLocation) {

		Vector3 lastPosition = transform.position;

		//Position
		Vector3 currentPosition = currentLocation.convertCoordinateToVector ();
		currentPosition.y = transform.position.y;

		if (lastPosition == Vector3.zero) {
			lastPosition = currentPosition;
		}
			
	}


	void LateUpdate () {

		Vector3 dir = Vector3.forward;
		dir =  Camera.main.transform.forward;
		dir = Vector3.ProjectOnPlane (dir, Vector3.down);

		Vector3 lastPosition = transform.position;

		rotateAvatar (dir);

		Vector3 v1 = Vector3.forward;
		bool drag = false;
		if (Application.isMobilePlatform) {
			drag = Input.touchCount >= 1;
			if (drag)
				v1 = Input.GetTouch (0).position;
		} else {
			drag = Input.GetMouseButton (0);
			if (drag)
				v1 = Input.mousePosition;
		}


		Vector3 v2 = Camera.main.WorldToScreenPoint (avatarFigure.transform.position);
		float d = Vector2.Distance (v1, v2)/Screen.height;

		if (autoDrive) {
			d = 1; 
			drag = true;
		}

		if (d < 0.5f)
			d = 0.5f;

		if (v1.y > v2.y && Mathf.Abs(v2.x-v1.x)<80)
			d = -d;
		
		if (drag && !GOUtils.IsPointerOverUI())
			transform.Translate(Time.deltaTime * (d*60 * avatarFigure.transform.forward));


	}

	void rotateAvatar(Vector3 targetDir) {


		if (targetDir != Vector3.zero) {
			avatarFigure.transform.rotation = Quaternion.Slerp(
				avatarFigure.transform.rotation,
				Quaternion.LookRotation(targetDir),
				Time.deltaTime * 5
			);
		}
	}


}
