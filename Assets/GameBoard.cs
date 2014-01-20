using UnityEngine;
using System.Collections;

[System.Serializable]
public enum Terrain_Type {
	
};

[System.Serializable]
public enum Team {
	
};

[System.Serializable]
public enum Unit_Type {
	
};

public class GameBoard : MonoBehaviour {

	public int sizex, sizey;
	public Terrain_Type [,] terrain = new Terrain_Type[40,40];
	public Team [,] unit_team = new Team[40,40];
	public Unit_Type [,] unit = new Unit_Type[40,40];

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
