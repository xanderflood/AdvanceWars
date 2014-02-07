using UnityEngine;
using System.Collections;

public class APC : Unit
{

	void Start () {
		isVehicle = true;
	}
    protected override void Update() {
        base.Update();
        if (!GameBoard.Instance.someUnitAnimating)
        {
            DeleteIndicators();
            makeMoveIndicators();
        }
    
    }

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
            return 50;
        }
    }

    override protected int GetUnitMoveRange()
    {
        return 6;
    }
    protected override void Die()
    {
        if (!GameBoard.Instance.GameOver)
        {
            GameBoard.Instance.GameOver = true;
            GameObject victory = GameObject.Find("VictoryText");
            victory.renderer.enabled = true;
        }
        base.Die();

    }

	override protected int GetAttack(Unit target) {
		return 0;
	}
}