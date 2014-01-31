using UnityEngine;
using System.Collections;

public abstract class RedUnit : Unit {
	
	void Start()
	{

		base.Start ();

		team = TeamColor.Red;

	}
}
