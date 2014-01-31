using UnityEngine;
using System.Collections;

public abstract class BlueUnit : Unit {
	
	new void Start()
	{
		base.Start ();
		
		team = TeamColor.Blue;
	}
}
