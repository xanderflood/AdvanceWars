﻿using UnityEngine;
using System.Collections.Generic;

public enum Direction {
	Up,
	Down,
	Left,
	Right,
}

public class moveindicatorscript : MonoBehaviour {
	
	public List<Direction> path;
	public GameObject rightArrow;
	public GameObject upArrow;
	public GameObject downArrow;
	public GameObject leftArrow;
	
	static List<GameObject> drawn = new List<GameObject>();
	
	void OnTriggerEnter2D(Collider2D other) {
		
		CursorScript.Instance.currentIndicator = this.gameObject;
		
		drawPath (path, Unit.currentSelectedUnit.transform.position);
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

			Quaternion rot;
			switch(d) {
			case Direction.Right:
				pos.x += 0.5f;
				tmp = (GameObject)Instantiate(rightArrow, pos, Quaternion.identity);
				pos.x += 0.5f;
				break;
			case Direction.Left:
				pos.x -= 0.5f;
				tmp = (GameObject)Instantiate(leftArrow, pos, Quaternion.identity);
				pos.x -= 0.5f;
				break;
			case Direction.Up:
				pos.y += 0.5f;
				tmp = (GameObject)Instantiate(upArrow, pos, Quaternion.identity);
				pos.y += 0.5f;
				break;
			case Direction.Down:
				pos.y -= 0.5f;
				tmp = (GameObject)Instantiate(downArrow, pos, Quaternion.identity);
				pos.y -= 0.5f;
				break;
			}
			
			drawn.Add(tmp);
		}
		
	}
}
