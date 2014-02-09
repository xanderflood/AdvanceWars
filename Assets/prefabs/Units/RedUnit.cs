using UnityEngine;
using System.Collections;

public abstract class RedUnit : Unit {
	
	new void Start()
	{

		base.Start ();

		team = TeamColor.Red;

	}
}
