using UnityEngine;
using System.Collections;

public class AiTeam : Team
{

    // Use this for initialization
    void Start()
    {
        GameBoard.Instance.blueTeam = this;
        color = TeamColor.Blue;
        addUnit(9, 1);
        addUnit(9, 2);
        addUnit(9, 7);
    }

    override public void  startTurn() {
        foreach (Unit u in units) {
            u.moveLeft(); 
        }
        GameBoard.Instance.changeTeam();

    }
}

