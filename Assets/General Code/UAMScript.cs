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
				target.prepareToAttack();
				gameObject.SetActive(false);
			} else {
				gameObject.SetActive(false);
				target.isSelected = false;
				GameBoard.Instance.isAnyoneSelected = false;
				target.hasMovedThisTurn = true;
				//Debug.Log(owner);
				target.owner.unitMoved();

				pos.y += diff;
				onFire = true;
			}
		}

		transform.FindChild ("MenuIcon").transform.position = pos;
	}
}
