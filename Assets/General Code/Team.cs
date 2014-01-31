using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Team : MonoBehaviour {

	public Unit unitModel;
	
	public TeamColor color;
	public List<Unit> units;

	// ASSUMES THAT (x,y) IS A VALID LOCATION!
	public void addUnit(int x, int y) {
        Unit u = (Unit)Instantiate(unitModel, new Vector3(x, y, 0), Quaternion.identity);
        u.owner = this;
        units.Add(u);
	}

	public void unitDestroyed(Unit unit) {
		units.Remove (unit);
	}
	
	public void endTurn() {
		foreach (Unit u in units) {
			u.hasMovedThisTurn = false;
		}
	}

    public virtual void startTurn() 
    { }

	public void unitMoved() {
		checkTurnEnded();
	}

	public void checkTurnEnded() {

		foreach (Unit u in units) {
			if (!u.hasMovedThisTurn) {
				return;
			}
		}

		GameBoard.Instance.changeTeam ();
	}

}
