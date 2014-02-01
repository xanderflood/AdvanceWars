using UnityEngine;
using System.Collections;

public class RedTeam : Team {

	// Use this for initialization
	void Start () {
		GameBoard.Instance.redTeam = this;
		color = TeamColor.Red;
      
        addUnit(3, 1, color, 0);
        addUnit(3, 2, color, 2);
        addUnit(3, 3, color, 0);
         /* no mountain version
        addUnit(3, 3, color, 0);
        addUnit(3, 4, color, 2);
        addUnit(3, 5, color, 0);
         */
	}
}
