using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Team : MonoBehaviour {

	public Unit unitModel;
	
	public TeamColor color;
	public List<Unit> units;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// ASSUMES THAT (x,y) IS A VALID LOCATION!
	public void addUnit(int x, int y) {
		Instantiate (unitModel, new Vector3(x, y, 0), Quaternion.identity);
		((Unit)unitModel.GetComponent("Unit")).owner = this;
		units.Add(unitModel);
	}

	public void unitDestroyed(Unit unit) {
		units.Remove (unit);
	}
	
	public void startTurn() {
		foreach (Unit u in units) {
			u.hasMovedThisTurn = false;
		}
	}

	public void unitMoved() {
		checkTurnEnded();
	}

	public void checkTurnEnded() {

		bool unitsRemaining = false;
		foreach (Unit u in units) {
			if (!u.hasMovedThisTurn) {
				unitsRemaining = true;
				break;
			}
		}

		if (!unitsRemaining) {
			GameBoard.Instance.changeTeam ();
		} else {
			
			int zero = 0;
			int error = 1 / zero;
		}
	}

}
