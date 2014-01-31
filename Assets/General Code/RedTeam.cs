using UnityEngine;
using System.Collections;

public class RedTeam : Team {

	// Use this for initialization
	void Start () {
		GameBoard.Instance.redTeam = this;
		color = TeamColor.Red;
		
		addUnit(0, 1, color);
        addUnit(0, 2, color);
        addUnit(0, 7, color);
	}
}
