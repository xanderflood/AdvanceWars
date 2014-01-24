using UnityEngine;
using System.Collections;

public class GameSystemControl : MonoBehaviour {
	
	public GameObject field;
	public GameObject road;
	public GameObject mountain;
	public GameObject wood;

	public GameBoard game;

	// Use this for initialization
	void Start () {

		game = GameBoard.Instance;

		// Load the gameboard
		for (int x = 0; x < game.sizex; ++x) {
			for (int y = 0; y < game.sizey; ++y) {
				switch (game.terrains[x, y]) {
				case TerrainType.Field:
					Instantiate(field, new Vector3(x, y, 0), Quaternion.identity);
					break;
				case TerrainType.Road:
					Instantiate(road, new Vector3(x, y, 0), Quaternion.identity);
					break;
				case TerrainType.Mountain:
					Instantiate(mountain, new Vector3(x, y, 0), Quaternion.identity);
					break;
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {

		// Check whether turn ended
		if (Input.GetKeyDown (KeyCode.X)) {
			GameBoard.Instance.changeTeam();
		}
	}
}
