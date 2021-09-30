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
				() => { ModifyUnit(UnitType.Spitter, damage:-0.5f); 
					RangedAttack(UnitType.Spitter, true); },
				"Spitters now heal the front ally for 10% of damage dealt",
				() => {  }),

			new EvolveData(400,
				"Soldiers run 5% faster",
				() => { ModifyUnit(UnitType.Soldier, moveSpeed:0.05f); },
				"Defenders have 5% more max hp",
				() => { ModifyUnit(UnitType.Defender, health:0.05f); }),

			new EvolveData(400,
				"DNA generation is 5% faster",
				() => {  },
				"Soldiers give extra DNA on kill",
				() => { ModifyUnit(UnitType.Soldier, dna:100); }),

			new EvolveData(400,
				"Defenders have 20% damage reduction",
				() => { ModifyUnit(UnitType.Defender, damage:-0.2f); },
				"Your hive restores 10% hp",
				() => { player.Base.Health *= 1.1f; }),

			new EvolveData(400,
				"Enemies that attack your hive take 5% damage per hit",
				() => {  },
				"All units have 5% more hp",
				() => { ModifyUnit(UnitType.Defender, health:0.05f);
					ModifyUnit(UnitType.Soldier, health:0.05f);
					ModifyUnit(UnitType.Spitter, health:0.05f);
					ModifyUnit(UnitType.Worker, health:0.05f); }),

			new EvolveData(400,
				"Soldiers have 50% extra damage, but 50% slower attack speed",
				() => { ModifyUnit(UnitType.Soldier, damage:0.5f, attackSpeed:-0.5f); },
				"Spitters increase nearby units damage by 10%",
				() => {  }),

			new EvolveData(400,
				"Workers have 10% extra speed",
				() => { ModifyUnit(UnitType.Worker, moveSpeed:0.1f); },
				"When a defender dies, it deals 10% of its max hp as damage to the front 2 enemies",
				() => {  }),
			new EvolveData(0,
				"Evolution tree complete",
				() => {  },
				"Evolution tree complete",
				() => {  }),
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
		if (Game.Current.Freeze) return;

		if (evolutionCounter + 1 >= DATA.Length) return;// else already fully evolved

		if (player.DNA >= DATA[evolutionCounter].DNACost)
		{
			if (option == 0) DATA[evolutionCounter].Option1Func();
			else if (option == 1) DATA[evolutionCounter].Option2Func();

			player.DNA -= DATA[evolutionCounter].DNACost;

			evolutionCounter += 1;

			Game.Current.UI.UpdateEvolutionText(GetEvolutionText(0), player.PlayerID, 0);
			Game.Current.UI.UpdateEvolutionText(GetEvolutionText(1), player.PlayerID, 1);
		}
        else
        {
			// not enough DNA to evolve
        }
    }


	//----------------------------------------------------------------------------------------------------------------------------------<


	public string GetEvolutionText(int option) => option == 0  ? DATA[evolutionCounter].Option1Text : DATA[evolutionCounter].Option2Text;


	//----------------------------------------------------------------------------------------------------------------------------------<


	#region EVOLUTION UPGRAGE METHODS


	//----------------------------------------------------------------------------------------------------------------------------------<


	private void ModifyUnit(UnitType unitType, float moveSpeed = 0, float attackSpeed = 0,
		float damage = 0, float health = 0, int dna = 0)
	{
		UnitModifiers modifier = player.GetModifierReference(unitType);
		modifier.MoveSpeed *= (1 + moveSpeed);
		modifier.AttackSpeed *= (1 + attackSpeed);
		modifier.Damage *= (1 + damage);
		modifier.Health *= (1 + health);
		modifier.GiveDNA += dna;
	}

	private void RangedAttack(UnitType unitType, bool ranged)
	{
		UnitModifiers modifier = player.GetModifierReference(unitType);
		modifier.HitTwoEnemies = ranged;
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	#endregion
}


//----------------------------------------------------------------------------------------------------------------------------------<


public struct EvolveData {

	public int DNACost;

	public string Option1Text;
	public string Option2Text;

	public Action Option1Func;
	public Action Option2Func;


	public EvolveData(int cost, string op1Text, Action op1Func, string op2Text, Action op2Func) {
		DNACost = cost;
		Option1Text = op1Text;
		Option1Func = op1Func;
		Option2Text = op2Text;
		Option2Func = op2Func;
	}

}