using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Team : MonoBehaviour {

	public Unit Infentry;
	public Unit APC;
	public Unit Mech;
	
	public TeamColor color;
	public List<Unit> units;

	// ASSUMES THAT (x,y) IS A VALID LOCATION!
	public void addUnit(int x, int y, TeamColor color, int type) {
        Unit u;
        if (type == 1) {
			u = (Unit)Instantiate (APC, new Vector3 (x, y, 0), Quaternion.identity);
		} else if (type == 0) {
			u = (Unit)Instantiate (Infentry, new Vector3 (x, y, 0), Quaternion.identity);
		} else {
			u = (Unit)Instantiate (Mech, new Vector3 (x, y, 0), Quaternion.identity);
		}
        u.owner = this;
        u.team = color;
        units.Add(u);
	}

	public void unitDestroyed(Unit unit) {
		units.Remove (unit);
	}
	
	public void endTurn() {
		foreach (Unit u in units) {
			u.hasMovedThisTurn = false;
			u.menuing = false;
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
