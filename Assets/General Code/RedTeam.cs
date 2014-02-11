using UnityEngine;
using System.Collections;

public class RedTeam : Team {

	// Use this for initialization
	void Start () {
		GameBoard.Instance.redTeam = this;
		color = TeamColor.Red;
      

        if (GameBoard.Instance.curlevel == 1)
        { //multiplayer
            addUnit(3, 4, color, 0);
            addUnit(5, 6, color, 0);

        }
        else //singleplayer
        {
            addUnit(3, 1, color, 0);
            addUnit(3, 2, color, 2);
            addUnit(3, 3, color, 0);
        }

	}
}
