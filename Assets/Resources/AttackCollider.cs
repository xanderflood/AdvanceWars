using UnityEngine;
using System.Collections;

public class AttackCollider : MonoBehaviour {

	void OnTriggerExit2D(Collider2D other)
	{
		CursorScript.Instance.cursorsr.gameObject.SetActive(true);
		CursorScript.Instance.crosshsr.gameObject.SetActive(false);
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		CursorScript.Instance.cursorsr.gameObject.SetActive(false);
		CursorScript.Instance.crosshsr.gameObject.SetActive(true);
	}
}
