using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evolution {


	public EvolveData[] DATA;
	void AssignData() {
		// I would of assigned the data in the field initializer as readonly. But because OOP is shit, the delegate func calls need to refernece the object(Evolution), so the here it is.
		DATA = new EvolveData[] {
			new EvolveData(400,
				"Spitters now hit 2 enemies but have 50% damage",
				func => { IncreaseSpreadAndDecreaseDamage(UnitType.Spitter); return 0; },
				"Spitters now heal the front ally for 10% of damage dealt",
				func => { return 0; }),

			new EvolveData(400,
				"Soldiers run 5% faster",
				func => { IncreaseUnitSpeed(0.05f, UnitType.Soldier); return 0; },
				"Defenders have 5% more hp",
				func => { return 0; }),

			new EvolveData(400,
				"DNA generation is 5% faster",
				func => { return 0; },
				"Soldiers give DNA on kill",
				func => { return 0; }),

			new EvolveData(400,
				"Defenders have 20% damage reduction",
				func => { return 0; },
				"Your hive restores 10% hp",
				func => { return 0; }),

			new EvolveData(400,
				"Enemies that attack your hive take 5% damage per hit",
				func => { return 0; },
				"All units have 5% more hp",
				func => { return 0; }),

			new EvolveData(400,
				"Soldiers have 50% extra damage, but 50% slower attack speed",
				func => { return 0; },
				"Spitters increase nearby units damage by 10%",
				func => { return 0; }),

			new EvolveData(400,
				"Workers have extra speed",
				func => { return 0; },
				"When a defender dies, it deals 10% of its max hp as damage to the front 2 enemies",
				func => { return 0; }),
		};
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	private Player player;

    // How many evolutions the player has gone through so far
    private int evolutionCounter = 0;


	//----------------------------------------------------------------------------------------------------------------------------------<


	public Evolution(Player player) {
		this.player = player;
		AssignData();
    }


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Processes the evolution path the player has taken
	/// </summary>
	/// <param name="player"></param>
	/// <param name="evolution">The evolution path</param>
	public void Evolve(int option) {
		if (Game.Current.IsFinished) return;

		if (evolutionCounter + 1 >= DATA.Length) return;// else already fully evolved

		evolutionCounter += 1;

		if		(option == 0) DATA[evolutionCounter].Option1Func(0);
		else if (option == 1) DATA[evolutionCounter].Option2Func(0);


		Game.Current.UI.UpdateEvolutionText(GetEvolutionText(0), player.PlayerID, 0);
		Game.Current.UI.UpdateEvolutionText(GetEvolutionText(1), player.PlayerID, 1);
    }


	//----------------------------------------------------------------------------------------------------------------------------------<


	public string GetEvolutionText(int option) => option == 0  ? DATA[evolutionCounter].Option1Text : DATA[evolutionCounter].Option2Text;


	//----------------------------------------------------------------------------------------------------------------------------------<


	#region EVOLUTION UPGRAGE METHODS


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Decreases given units' damage and increases range
	/// </summary>
	/// <param name="unitType"></param>
	private void IncreaseSpreadAndDecreaseDamage(UnitType unitType)
    {
        // TODO: make unit hit 2 enimies

        foreach (UnitController unit in Game.Current.GetExistingUnits(player.PlayerID))
        {
            if (unit.Type == unitType && unit.GetUnitOwner().PlayerID == player.PlayerID)
            {
                unit.SetUnitDamage(unit.GetUnitDamage() / 2);
            }
        }
    }


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Increases given units' speed
	/// </summary>
	/// <param name="speedIncrease">(percentage in decimal)</param>
	/// <param name="unitType"></param>
	private void IncreaseUnitSpeed(float speedIncrease, UnitType unitType) {

		/* Instead of using a function, maybe change modifiers. Then the units use the modifier values in realtime. This way, current & future units are dealt with.
        foreach (UnitController unit in Game.Current.GetExistingUnits(player.PlayerID)) {
            if (unit.Type == unitType) unit.SetUnitSpeed(unit.GetUnitSpeed() * 1.05f);
        }*/


		switch(unitType) {
			case UnitType.Worker: {
				player.WorkerModifiers.MoveSpeed *= (1 + speedIncrease);
				break;
			}
			case UnitType.Soldier: {
				player.SoldierModifiers.MoveSpeed *= (1 + speedIncrease);
				break;
			}
			case UnitType.Spitter: {
				player.SpitterModifiers.MoveSpeed *= (1 + speedIncrease);
				break;
			}
			case UnitType.Defender: {
				player.DefenderModifiers.MoveSpeed *= (1 + speedIncrease);
				break;
			}
		}
    }


	//----------------------------------------------------------------------------------------------------------------------------------<


	#endregion
}


//----------------------------------------------------------------------------------------------------------------------------------<


public struct EvolveData {

	public int DNACost;

	public string Option1Text;
	public string Option2Text;

	public Func<int, int> Option1Func;
	public Func<int, int> Option2Func;


	public EvolveData(int cost, string op1Text, Func<int, int> op1Func, string op2Text, Func<int,int> op2Func) {
		DNACost = cost;
		Option1Text = op1Text;
		Option1Func = op1Func;
		Option2Text = op2Text;
		Option2Func = op2Func;
	}

}