﻿using UnityEngine;
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

		sizex = sizey = 10;

		terrains = new TerrainType[sizex, sizey];

		// Load the gameboard
		for (int x = 0; x < sizex; ++x) {
			for (int y = 0; y < sizey; ++y) {
				terrains[x, y] = (TerrainType)(1 << (x+2*y)%3);
			}
		}
		// case our custom map:

		for (int x = 0; x < sizex; ++x) {
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

