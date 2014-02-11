using UnityEngine;
using System.Collections;

public class LevelSelect : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            GameBoard.Instance.curlevel = 1;
            Application.LoadLevel("SampleLvl1");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            GameBoard.Instance.curlevel = 0;
            Application.LoadLevel("MyCustomLevel");
        }
	}
}
