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

[System.Serializable]
public struct Location {
	int x, y;
};

[System.Serializable]
public class GameBoard : ScriptableObject {

	public int sizex, sizey;
	public TerrainType [] terrain;
	public List<Location> unitLocs;
	public List<Location> units;
	
	public Team current = Team.Red;
	
	private static GameBoard instance;
	
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
