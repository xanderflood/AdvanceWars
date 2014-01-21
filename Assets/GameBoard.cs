using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public enum TerrainType {
	Road = 1,
	Field = 2,
	Mountain = 4,
	Forest = 4, //Ask Alex about this?
};

[System.Serializable]
public enum Team : int {
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
	public TerrainType [] terrain;
	public List<Vector2> unitLocs = new List<Vector2>();
	//public List<Location> units;
	
	public Team current = Team.Red;
	
	private static GameBoard instance = new GameBoard();
	
	private GameBoard() { }

	public static GameBoard Instance
	{
		get 
		{
			if (instance == null)
			{
				instance = ScriptableObject.CreateInstance<GameBoard>();
			}
			return instance;
		}
	}

	public void changeTeam() {
		current = (Team)(-(int)current);
	}

}
