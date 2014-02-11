using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public enum TerrainType {
	Road = 1,
	Field = 2,
	Mountain = 3,
    Forest = 4,
};

[System.Serializable]
public enum TeamColor : int {
	Red = -1,
	Blue = +1,
	None = 0,
};

[System.Serializable]
public enum UnitType {
	Infantry,
};

//xander:: any reason not to just use a vector2 here?
/*[System.Serializable]
public struct Location {
	int x, y;
}; */

[System.Serializable]
public class GameBoard : ScriptableObject {
    public bool isAnyoneSelected = false;
	public int sizex, sizey;
	public TerrainType [,] terrains;
	public List<Vector2> unitLocs = new List<Vector2>();
    public bool GameOver = false;
	public TeamColor current = TeamColor.Red;
    public int curlevel = 0;
	public Team redTeam;
	public Team blueTeam;
    public bool someUnitAnimating;
	public bool someUnitActive;
	public bool someUnitAttacking;
	
	private static GameBoard instance;

	public bool tutorialing;
	
	private GameBoard() { }

	public static GameBoard Instance
	{
		get 
		{
			if (instance == null)
			{
				instance = ScriptableObject.CreateInstance<GameBoard>();
				instance.Initialize();
			}
			return instance;
		}
	}

	// Contains the sample level, basically
	private void Initialize() {

		sizex = sizey = 15;

		terrains = new TerrainType[sizex, sizey];

		// Load the gameboard
	//	for (int x = 0; x < sizex; ++x) {
	//		for (int y = 0; y < sizey; ++y) {
	//			terrains[x, y] = (TerrainType)(1 << (x+2*y)%3);
	//		}
	//	}
        MakeTutorial();

	}

    public void MakeCustomLevel() {
        sizex = sizey = 10;
        for (int x = 0; x < sizex; ++x)
        {
            terrains[x, 0] = TerrainType.Mountain;
            terrains[x, 1] = TerrainType.Field;
            terrains[x, 2] = TerrainType.Road;
            terrains[x, 3] = TerrainType.Field;
            terrains[x, 4] = TerrainType.Field;
            terrains[x, 5] = TerrainType.Field;
            terrains[x, 6] = TerrainType.Field;
            terrains[x, 7] = TerrainType.Road;
            terrains[x, 8] = TerrainType.Field;
            terrains[x, 9] = TerrainType.Mountain;
        }
        terrains[6, 1] = TerrainType.Forest;
        terrains[5, 1] = TerrainType.Forest;
        terrains[5, 3] = TerrainType.Forest;
        terrains[4, 3] = TerrainType.Forest;

        terrains[4, 4] = TerrainType.Forest;
        terrains[3, 4] = TerrainType.Forest;
        terrains[3, 5] = TerrainType.Forest;
        terrains[2, 5] = TerrainType.Forest;


        terrains[2, 6] = TerrainType.Forest;
        terrains[1, 6] = TerrainType.Forest;
        terrains[0, 8] = TerrainType.Forest;
        terrains[1, 8] = TerrainType.Forest;
    }

    public void Lost(TeamColor col)
    {
        if (blueTeam is AiTeam)
            return;

        string t = "";
        switch (col)
        {
            case TeamColor.Blue:
                t = "Red";
                break;
            case TeamColor.Red:
                t = "Blue";
                break;
        }

        CursorScript.Instance.endTextOther.GetComponent<TextMesh>().text = t;
        CursorScript.Instance.endText.SetActive(true);
    }

    public void MakeTutorial()
    {
        sizex = 15;
        sizey = 10;
        for (int x = 0; x < sizex; ++x)
        {
            for (int y = 0; y < sizey; ++y)
            {
                terrains[x, y] = TerrainType.Field;
            }
        }

        terrains[0, 0] = TerrainType.Mountain;
        terrains[1, 0] = TerrainType.Mountain;
        terrains[2, 0] = TerrainType.Mountain;
        terrains[3, 0] = TerrainType.Mountain;
        terrains[5, 0] = TerrainType.Mountain;
        terrains[6, 0] = TerrainType.Mountain;
        terrains[7, 0] = TerrainType.Mountain;
        terrains[9, 0] = TerrainType.Mountain;
        terrains[10, 0] = TerrainType.Mountain;
        terrains[11, 0] = TerrainType.Mountain;
        terrains[12, 0] = TerrainType.Mountain;
        terrains[13, 0] = TerrainType.Mountain;
        terrains[14, 0] = TerrainType.Mountain;


        terrains[0, 1] = TerrainType.Mountain;
        terrains[1, 1] = TerrainType.Mountain;
        terrains[2, 1] = TerrainType.Mountain;
        terrains[6, 1] = TerrainType.Mountain;
        terrains[11, 1] = TerrainType.Mountain;
        terrains[12, 1] = TerrainType.Mountain;
        terrains[13, 1] = TerrainType.Mountain;
        terrains[14, 1] = TerrainType.Mountain;


        terrains[0, 2] = TerrainType.Mountain;
        terrains[12, 2] = TerrainType.Mountain;
        terrains[13, 2] = TerrainType.Mountain;
        terrains[14, 2] = TerrainType.Mountain;

        terrains[13, 3] = TerrainType.Mountain;
        terrains[14, 3] = TerrainType.Mountain;

        terrains[14, 4] = TerrainType.Mountain;

        terrains[0, 5] = TerrainType.Mountain;


        terrains[0, 7] = TerrainType.Mountain;
        terrains[14, 7] = TerrainType.Mountain;


        terrains[0, 8] = TerrainType.Mountain;
        terrains[1, 8] = TerrainType.Mountain;
        terrains[10, 8] = TerrainType.Mountain;
        terrains[11, 8] = TerrainType.Mountain;
        terrains[13, 8] = TerrainType.Mountain;
        terrains[14, 8] = TerrainType.Mountain;



        terrains[0, 9] = TerrainType.Mountain;
        terrains[1, 9] = TerrainType.Mountain;
        terrains[2, 9] = TerrainType.Mountain;
        terrains[9, 9] = TerrainType.Mountain;
        terrains[10, 9] = TerrainType.Mountain;
        terrains[11, 9] = TerrainType.Mountain;
        terrains[12, 9] = TerrainType.Mountain;
        terrains[13, 9] = TerrainType.Mountain;
        terrains[14, 9] = TerrainType.Mountain;


        //FORESTS

        terrains[1, 2] = TerrainType.Forest;
        terrains[0, 3] = TerrainType.Forest;
        terrains[14, 6] = TerrainType.Forest;
        terrains[13, 7] = TerrainType.Forest;
        terrains[12, 8] = TerrainType.Forest;

        terrains[2, 8] = TerrainType.Forest;
        terrains[3, 8] = TerrainType.Forest;
        terrains[3, 9] = TerrainType.Forest;
        terrains[4, 9] = TerrainType.Forest;

    }


	public void changeTeam() {
		if (someUnitActive)
			return;

		SpriteRenderer sr = (SpriteRenderer)CursorScript.Instance.transform.
			FindChild("sprite").GetComponent ("SpriteRenderer");

		if (current == TeamColor.Red) {
			current = TeamColor.Blue;
			redTeam.endTurn();
            blueTeam.startTurn();
            // if blue team is not an AI, change the color of the cursor
            if (blueTeam is BlueTeam) 
            {
                sr.color = Color.blue;
            }
		} else {
			current = TeamColor.Red;
			blueTeam.endTurn();
            redTeam.startTurn();
			sr.color = Color.red;
		}
	}
    public int getTerrainDefenceBonus(Transform UnitLoc) {
        int x = (int)Math.Round(UnitLoc.position.x);
        int y = (int)Math.Round(UnitLoc.position.y);

        TerrainType t = terrains[x, y];
        switch (t) { 
            case TerrainType.Field:
                return 1;
            case TerrainType.Forest:
                return 2;
            case TerrainType.Mountain:
                return 4;
            case TerrainType.Road:
                return 0;      
        }
        return 0;
    }

}

