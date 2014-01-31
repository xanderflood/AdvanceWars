using UnityEngine;
using System.Collections;

public class BlueTeam : Team {

	// Use this for initialization
	void Start () {
		GameBoard.Instance.blueTeam = this;
		color = TeamColor.Blue;


        addUnit(9, 1, color, 0);
        addUnit(9, 2, color, 0);
        addUnit(9, 7, color, 0);
	}
}
