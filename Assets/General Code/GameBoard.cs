using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public enum TerrainType {
	Road = 1,
	Field = 2,
	Mountain = 3,
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
	public List<Unit> redunits;
	
	public TeamColor current = TeamColor.Red;

	public Team redTeam;
	public Team blueTeam;

	public bool someUnitActive;
	
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
			terrains[x, 4] = TerrainType.Mountain;
		}

	}

	public void changeTeam() {

		if (someUnitActive)
			return;

		SpriteRenderer sr = (SpriteRenderer)CursorScript.Instance.transform.
			FindChild("sprite").GetComponent ("SpriteRenderer");

		if (current == TeamColor.Red) {
			current = TeamColor.Blue;
			redTeam.endTurn();

			sr.color = Color.blue;
		} else {
			current = TeamColor.Red;
			blueTeam.endTurn();
			
			sr.color = Color.red;
		}
	}
}
