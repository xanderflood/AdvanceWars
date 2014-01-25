using UnityEngine;
using System.Collections;

public class RedTeam : Team {

	// Use this for initialization
	void Start () {
		GameBoard.Instance.redTeam = this;
		color = TeamColor.Red;
		
		addUnit (1, 0);
		addUnit (5, 5);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
