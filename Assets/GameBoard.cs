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
public enum Team {
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
	public Team current;
	public TerrainType [] terrain;
	public List<Location> unitLocs;
	public List<Location> units;
	
}
