using UnityEngine;
using System.Collections.Generic;

public enum Direction {
	Up,
	Down,
	Left,
	Right,
}

public class moveindicatorscript : MonoBehaviour {
	
	public List<Direction> path;
	public GameObject moveArrow;
	
	static List<GameObject> drawn = new List<GameObject>();
	
	void OnTriggerEnter2D(Collider2D other) {
		
		CursorScript.Instance.currentIndicator = this.gameObject;
		
		drawPath (path, Vector3.zero);//Unit.currentSelected.transform.position);
	}
	
	void OnTriggerExit2D(Collider2D other) {
		
		if (CursorScript.Instance.currentIndicator == this) {
			CursorScript.Instance.currentIndicator = null;
		}
	}
	
	public static void destroyPath() {
		foreach (GameObject go in drawn) {
			Destroy (go);
		}
	}
	
	void drawPath(List<Direction> path, Vector3 pos) {
		
		destroyPath ();
		
		GameObject tmp = null;
		foreach (Direction d in path) {
			
			switch(d) {
			case Direction.Right:
				pos.x += 0.5f;
				tmp = (GameObject)Instantiate(moveArrow, pos, new Quaternion(0,0,0,0));
				pos.x += 0.5f;
				break;
			case Direction.Left:
				pos.x -= 0.5f;
				tmp = (GameObject)Instantiate(moveArrow, pos, new Quaternion(0,0,180,0));
				pos.x -= 0.5f;
				break;
			case Direction.Up:
				pos.y += 0.5f;
				tmp = (GameObject)Instantiate(moveArrow, pos, new Quaternion(0,0,90,0));
				pos.y += 0.5f;
				break;
			case Direction.Down:
				pos.y -= 0.5f;
				tmp = (GameObject)Instantiate(moveArrow, pos, new Quaternion(0,0,-90,0));
				pos.y -= 0.5f;
				break;
			}
			
			drawn.Add(tmp);
		}
		
	}
}
