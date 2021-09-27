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
				func => { ModifyUnit(UnitType.Soldier, moveSpeed:0.05f); return 0; },
				"Defenders have 5% more hp",
				func => { ModifyUnit(UnitType.Defender, health:0.05f); return 0; }),

			new EvolveData(400,
				"DNA generation is 5% faster",
				func => { return 0; },
				"Soldiers give DNA on kill",
				func => { return 0; }),

			new EvolveData(400,
				"Defenders have 20% damage reduction",
				func => { ModifyUnit(UnitType.Defender, damage:-0.2f); return 0; },
				"Your hive restores 10% hp",
				func => { return 0; }),

			new EvolveData(400,
				"Enemies that attack your hive take 5% damage per hit",
				func => { return 0; },
				"All units have 5% more hp",
				func => { ModifyUnit(UnitType.All, health:0.05f); return 0; }),

			new EvolveData(400,
				"Soldiers have 50% extra damage, but 50% slower attack speed",
				func => { ModifyUnit(UnitType.Soldier, damage:0.5f, attackSpeed:-0.5f); return 0; },
				"Spitters increase nearby units damage by 10%",
				func => { return 0; }),

			new EvolveData(400,
				"Workers have 10% extra speed",
				func => { ModifyUnit(UnitType.Worker, moveSpeed:0.1f); return 0; },
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

	private void ModifyUnit(UnitType unitType, float moveSpeed = 1, float attackSpeed = 1, 
		float damage = 1, float health = 1, bool ranged = false)
    {
		switch(unitType)
        {
			case UnitType.Worker:
				player.WorkerModifiers.MoveSpeed *= (1 + moveSpeed);
				player.WorkerModifiers.AttackSpeed *= (1 + attackSpeed);
				player.WorkerModifiers.Damage *= (1 + damage);
				player.WorkerModifiers.Health *= (1 + health);
				player.WorkerModifiers.HitTwoEnemies = ranged;
				break;
			case UnitType.Soldier:
				player.SoldierModifiers.MoveSpeed *= (1 + moveSpeed);
				player.SoldierModifiers.AttackSpeed *= (1 + attackSpeed);
				player.SoldierModifiers.Damage *= (1 + damage);
				player.SoldierModifiers.Health *= (1 + health);
				player.SoldierModifiers.HitTwoEnemies = ranged;
				break;
			case UnitType.Spitter:
				player.SpitterModifiers.MoveSpeed *= (1 + moveSpeed);
				player.SpitterModifiers.AttackSpeed *= (1 + attackSpeed);
				player.SpitterModifiers.Damage *= (1 + damage);
				player.SpitterModifiers.Health *= (1 + health);
				player.SpitterModifiers.HitTwoEnemies = ranged;
				break;
			case UnitType.Defender:
				player.DefenderModifiers.MoveSpeed *= (1 + moveSpeed);
				player.DefenderModifiers.AttackSpeed *= (1 + attackSpeed);
				player.DefenderModifiers.Damage *= (1 + damage);
				player.DefenderModifiers.Health *= (1 + health);
				player.DefenderModifiers.HitTwoEnemies = ranged;
				break;
			case UnitType.All:
				player.WorkerModifiers.MoveSpeed *= (1 + moveSpeed);
				player.WorkerModifiers.AttackSpeed *= (1 + attackSpeed);
				player.WorkerModifiers.Damage *= (1 + damage);
				player.WorkerModifiers.Health *= (1 + health);
				player.WorkerModifiers.HitTwoEnemies = ranged;
				player.SoldierModifiers.MoveSpeed *= (1 + moveSpeed);
				player.SoldierModifiers.AttackSpeed *= (1 + attackSpeed);
				player.SoldierModifiers.Damage *= (1 + damage);
				player.SoldierModifiers.Health *= (1 + health);
				player.SoldierModifiers.HitTwoEnemies = ranged;
				player.SpitterModifiers.MoveSpeed *= (1 + moveSpeed);
				player.SpitterModifiers.AttackSpeed *= (1 + attackSpeed);
				player.SpitterModifiers.Damage *= (1 + damage);
				player.SpitterModifiers.Health *= (1 + health);
				player.SpitterModifiers.HitTwoEnemies = ranged;
				player.DefenderModifiers.MoveSpeed *= (1 + moveSpeed);
				player.DefenderModifiers.AttackSpeed *= (1 + attackSpeed);
				player.DefenderModifiers.Damage *= (1 + damage);
				player.DefenderModifiers.Health *= (1 + health);
				player.DefenderModifiers.HitTwoEnemies = ranged;
				break;
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