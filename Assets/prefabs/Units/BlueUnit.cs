using UnityEngine;
using System.Collections;

public class BlueUnit : Unit {
	
	new void Start()
	{
		base.Start ();
		
		team = Team.Blue;
	}
}
