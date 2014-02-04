using UnityEngine;
using System.Collections;

public class UAMScript : MonoBehaviour {

	public Unit target;
	public bool onFire;
	public bool canFire;

	public float diff;

	public void turnOn(Unit u) {
		target = u;
		gameObject.SetActive (true);

		u.makeAttackIndicators ();
		
		Vector3 pos = transform.FindChild ("MenuIcon").transform.position;
		Color col = transform.FindChild ("Fire").GetComponent<SpriteRenderer>().color;

		if (u.IndicatorList.Count == 0) {
			pos.y -= diff;
			canFire = false;
			col.a = 50;

		} else {
			canFire = true;
			col.a = 255;
		}

		transform.FindChild ("MenuIcon").transform.position = pos;
		transform.FindChild ("Fire").GetComponent<SpriteRenderer> ().color = col;
		//Debug.Log (transform.FindChild ("Fire").GetComponent<SpriteRenderer> ().color.a);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = transform.FindChild ("MenuIcon").transform.position;

		if (canFire) {
			if (Input.GetKeyDown ("up") && !onFire) {
				onFire = true;
				pos.y += diff;
			}

			if (Input.GetKeyDown ("down") && onFire) {
				onFire = false;
				pos.y -= diff;
			}
		}

		if (Input.GetKeyDown ("space")) {
			if (canFire && onFire) {
				target.prepareToAttack();
				gameObject.SetActive(false);
			} else {
				gameObject.SetActive(false);
				target.isSelected = false;
				GameBoard.Instance.isAnyoneSelected = false;
				GameBoard.Instance.someUnitAttacking = false;
				GameBoard.Instance.someUnitActive = false;
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
