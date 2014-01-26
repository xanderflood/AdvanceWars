using UnityEngine;
using System.Collections;

public class AttackCollider : MonoBehaviour {
	
	
	public SpriteRenderer cursorsr;
	public SpriteRenderer crosshsr;

	void Start() {

		cursorsr = CursorScript.Instance.gameObject.transform.FindChild ("sprite").GetComponent<SpriteRenderer> ();
		crosshsr = CursorScript.Instance.gameObject.transform.FindChild ("crosshair").GetComponent<SpriteRenderer> ();
	}


	void OnTriggerExit2D(Collider2D other)
	{
		cursorsr.gameObject.SetActive(true);
		crosshsr.gameObject.SetActive(false);
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		cursorsr.gameObject.SetActive(false);
		crosshsr.gameObject.SetActive(true);
	}
}
