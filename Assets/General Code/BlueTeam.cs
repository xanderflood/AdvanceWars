using UnityEngine;
using System.Collections;

public class BlueTeam : Team {

	// Use this for initialization
	void Start () {
		GameBoard.Instance.blueTeam = this;
		color = TeamColor.Blue;


        addUnit(9, 1);
        addUnit(9, 2);
        addUnit(9, 7);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
