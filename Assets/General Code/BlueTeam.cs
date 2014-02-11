using UnityEngine;
using System.Collections;

public class BlueTeam : Team {

	// Use this for initialization
	void Start () {
		GameBoard.Instance.blueTeam = this;
		color = TeamColor.Blue;

        if (GameBoard.Instance.curlevel == 1)
        { //multiplayer
            addUnit(13, 1, color, 0);
            addUnit(13, 4, color, 0);
            //addUnit(9, 7, color, 0);
        }
        else { //dead code
        }
	}
}
