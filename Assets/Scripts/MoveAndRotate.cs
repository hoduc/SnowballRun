using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MoveAndRotate : MonoBehaviour {
	public Transform[] Targets;
	//public Transform Target;
	public float speed = 1.0f;
	public float rotationDegree = 90.0f;
	public float scalingFactor = 0.1f;
	public bool startMoving = false;
	public GameObject Plane;

	private Vector3 PlaneNormal;
	private Vector3 targetPos;
	private int targetIndex = -1;
	private Dictionary<string,Func<Collider, bool>> colliderHandlers = new Dictionary<string,Func<Collider, bool>>();

	bool PowerUpHandler(Collider other){
		//other.enabled = false;
		other.gameObject.SetActive(false);
		//other.isTrigger = false;
		//other.gameObject.GetComponent<MeshRenderer> ().enabled = false;
		transform.localScale += new Vector3 (scalingFactor, scalingFactor, scalingFactor);
		return true;
	}

	void SetUpColliderHandler(){
		colliderHandlers.Add ("powerup", PowerUpHandler);
	}

	Transform GetNextTransform(){
		if (Targets.Length == 0)
			return transform;
		return Targets [++targetIndex%Targets.Length];
	}

	// Use this for initialization
	void Start () {
		SetUpColliderHandler ();
		PlaneNormal = Plane.transform.TransformDirection (Plane.GetComponent<MeshFilter> ().mesh.normals [0]);
		Debug.Log ("currentpos:" + transform.position);
		targetPos = GetNewTargetPos();
		Debug.Log ("targetPos(new):" + targetPos);
	}

	Vector3 GetNewTargetPos(){
		Transform targetTransform = GetNextTransform();
		Debug.Log ("target transform:" + targetTransform.name);
		Vector3 targetPosProjected = Vector3.ProjectOnPlane (targetTransform.position, PlaneNormal);
		return new Vector3 (targetPosProjected.x, transform.position.y, targetPosProjected.z);
	}

	void OnTriggerEnter(Collider other){
		Debug.Log ("OnTriggerEnter:" + other.name);
		Func<Collider, bool> handler;
		if (colliderHandlers.TryGetValue (other.tag, out handler)) {
			handler (other);
		}
	}

	// Update is called once per frame
	void Update () {
		if (!startMoving)
			return;
		Debug.Log ("current index:" + targetIndex);

		transform.position = Vector3.MoveTowards (transform.position, targetPos, Time.deltaTime * speed);
		if (Vector3.Distance(transform.position,targetPos) < 0.5f){
			Debug.Log ("reached target!!!");
			targetPos = GetNewTargetPos ();	
		}

		//transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * speed);
		transform.Rotate (transform.right, Time.deltaTime * rotationDegree);
	}

	//handle state changing
	void FixedUpdate(){
		if (Input.GetKeyDown (KeyCode.Space)) {
			startMoving = !startMoving;
		}
	}
}
