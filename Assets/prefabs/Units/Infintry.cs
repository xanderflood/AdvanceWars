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
}