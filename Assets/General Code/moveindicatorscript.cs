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

	void OnTriggerEnter2D(Collider2D other) {

		CursorScript.Instance.currentIndicator = this.gameObject;
	}
}
