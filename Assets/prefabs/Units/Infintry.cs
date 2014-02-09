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
        else if (terrain == TerrainType.Forest) {
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
            return (int)System.Math.Round((hp / 10f) * 1.1f * ((100f - target.hp * GameBoard.Instance.getTerrainDefenceBonus(target.transform)) / 100f));
		}

        return (int)System.Math.Round((hp / 10f) * 6f * ((100f - target.hp * GameBoard.Instance.getTerrainDefenceBonus(target.transform)) / 100f));
	}

}