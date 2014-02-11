using UnityEngine;
using System.Collections.Generic;

public class TutorialScript : MonoBehaviour {

	public List<string> TXT = new List<string>();
	
	public GameObject txt;
	public TextMesh tm;
	public int i = 0;

	// Use this for initialization
	void Start () {
		tm = txt.GetComponent<TextMesh> ();

		TXT.Add("Stop the blue personnel carrier from\n" +
				"reaching the left side. Its movement\n" +
				"range, as well as the move it currently\n" +
				"plans to take, is displayed by the grey\n" +
				"region and the visible red arrows.");
		TXT.Add("The arrow keys move the cursor, and\n" +
				"the Z button selects. Selecting a unit\n" +
				"displays its movement range, and\n" +
				"selecting a location moves the unit.\n" +
				"If an enemy unit is nearby, you then\n" +
				"have the option to attack. At any\n" +
				"point in this process, the X button can\n" +
				"cancel your action.");
		TXT.Add("Your mech unit (carrying the bazooka)\n" +
				"can't move as far as the other units,\n" +
				"but can do far more damage. The\n" +
				"infantry units do very little damage,\n" +
				"but are useful for blocking the APC's\n" +
		        "path.");
		TXT.Add("Terrain is also an important tool. If\n" +
		        "the APC is forced to move through the\n" +
		        "woods, its progress will be slowed. Your\n" +
		        "units, however, move through these\n" +
		        "regions with no trouble.");
		TXT.Add("Good luck!");

		nextText ();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown (KeyCode.Z))
			nextText();
	}

	void nextText() {
		if (i < TXT.Count) {
			tm.text = TXT[i];
			++i;
		} else {
			gameObject.SetActive(false);
			GameBoard.Instance.tutorialing = false;
		}
	}
}
