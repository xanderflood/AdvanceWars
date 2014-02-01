using UnityEngine;
using System.Collections;

public class Mech : Unit {
	
	override protected int GetUnitMoveCost(TerrainType terrain)
	{
		if (terrain == TerrainType.Field)
		{
			return 1;
		}
		else if (terrain == TerrainType.Road)
		{
			return 1;
		}
		else ///if (terrain == TerrainType.Mountain)
		{
			return 1;
		}
	}
	
	override protected int GetUnitMoveRange()
	{
		return 2;
	}

	override protected int GetAttack(Unit target) {
		
		return (int)System.Math.Round((hp / 10f)*6f);
	}

	protected override void Die()
	{
		base.Die();
		
	}
}
