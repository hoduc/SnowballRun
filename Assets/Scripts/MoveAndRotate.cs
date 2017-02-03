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

	private Vector3 targetPos;
	private int targetIndex = 0;
	private Dictionary<string,Func<Collider, bool>> colliderHandlers = new Dictionary<string,Func<Collider, bool>>();

	bool PowerUpHandler(Collider other){
		other.gameObject.SetActive(false);
		transform.localScale += new Vector3 (scalingFactor, scalingFactor, scalingFactor);
		return true;
	}

	void SetUpColliderHandler(){
		colliderHandlers.Add ("powerup", PowerUpHandler);
	}

	// Use this for initialization
	void Start () {
		SetUpColliderHandler ();
		Debug.Log ("currentpos:" + transform.position);
		targetPos = GetNewTargetPos(Targets[targetIndex]);
		Debug.Log ("targetPos(new):" + targetPos);
	}

	Vector3 GetNewTargetPos(Transform targetTransform){
		Vector3 targetPosProjected = Vector3.ProjectOnPlane (targetTransform.position, Vector3.up);
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
		//transform.position = Vector3.Lerp(startPos, Target.position
		if (transform.position == Vector3.MoveTowards (transform.position, targetPos, Time.deltaTime * speed))
			targetPos = GetNewTargetPos (Targets[++targetIndex%Targets.Length]);
		transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * speed);
		transform.Rotate (transform.right, Time.deltaTime * rotationDegree);
	}
}
