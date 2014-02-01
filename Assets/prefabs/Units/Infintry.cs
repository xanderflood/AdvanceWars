using UnityEngine;
using System.Collections;

public class Infintry : Unit {

    override protected int GetUnitMoveCost(TerrainType terrain) {
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
            return 2;
        }
    }

    override protected int GetUnitMoveRange() {
        return 3;
    }
	
	override protected int GetAttack(Unit target) {

		if (target.isVehicle) {
			return 0;
		}
		
		return (int)System.Math.Round((hp / 10f)*5f);
	}

}