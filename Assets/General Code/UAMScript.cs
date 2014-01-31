using UnityEngine;
using System.Collections;

public class UAMScript : MonoBehaviour {

	public Unit target;
	public bool onFire;

	public float diff;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = transform.FindChild ("MenuIcon").transform.position;

		if (Input.GetKeyDown("up") && !onFire) {
			onFire = true;
			pos.y += diff;
		}

		if (Input.GetKeyDown("down") && onFire) {
			onFire = false;
			pos.y -= diff;
		}

		if (Input.GetKeyDown ("space")) {
			if (onFire) {
				//do attack
			} else {
				//cancel
			}
		}

		transform.FindChild ("MenuIcon").transform.position = pos;
	}
}
