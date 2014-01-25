using UnityEngine;
using System.Collections;

public class BlueTeam : Team {

	// Use this for initialization
	void Start () {
		GameBoard.Instance.blueTeam = this;
		color = TeamColor.Blue;
		
		addUnit (3, 0);
		addUnit (5, 7);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
