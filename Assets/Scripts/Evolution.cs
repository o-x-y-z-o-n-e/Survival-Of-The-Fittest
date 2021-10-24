using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evolution {


	#region Evolution Tree & Data


	//----------------------------------------------------------------------------------------------------------------------------------<


	const int TREE_LENGTH = 16;


	//----------------------------------------------------------------------------------------------------------------------------------<


	EvolveLevel[] TREE;
	void GenerateTree() {
		TREE = new EvolveLevel[TREE_LENGTH];
		for (int i = 0; i < TREE.Length; i++) {
			int lastOp1 = -1;
			int lastOp2 = -1;

			if(i > 0) {
				lastOp1 = TREE[i - 1].Option1;
				lastOp2 = TREE[i - 1].Option2;
			}

			int op1 = GetNewFuncIndex(lastOp1, lastOp2, -1);
			int op2 = GetNewFuncIndex(lastOp1, lastOp2, op1);

			TREE[i] = new EvolveLevel(GetCost(i), op1, op2);
		}
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Find new evolution option index that was not in the last level and is not that same as it's counterpart option
	/// </summary>
	int GetNewFuncIndex(int lastOp1, int lastOp2, int pairOp) {
		while(true) {
			int i = UnityEngine.Random.Range(0, DATA.Length);

			if (i != lastOp1 && i != lastOp2 && i != pairOp) return i;
		}
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	EvolveData[] DATA;
	void AssignData() {
		// I would of assigned the data in the field initializer as readonly. But because OOP is shit, the delegate func calls need to refernece the object(Evolution), so the here it is.
		DATA = new EvolveData[] {

			//***SPITTER***
			new EvolveData( //Multi Hit
				"Spitters now hit an additional enemy but deal 70% damage",
				() => { ModifyUnit(UnitType.Spitter, damage:-0.3f); RangedAttack(UnitType.Spitter, 2); }),

			new EvolveData( //Spitter Heal
				"Spitters heal the front unit 5 health per second",
				() => { EnableHealingFrontAlly(UnitType.Spitter); }),

			new EvolveData( //Stat Aura
				"Spitters increase the front unit Attack Speed by 30%",
				() => { EnableAttackSpeedAllyBuff(); }),

			new EvolveData( //M_Spd+
				"Spitters Move Speed +10%",
				() => { ModifyUnit(UnitType.Spitter, moveSpeed:0.10f); }),

			new EvolveData( //A_Spd+
				"Spitters Attack Speed +10%",
				() => { ModifyUnit(UnitType.Spitter, attackSpeed:0.05f); }),

			new EvolveData( //HP+
				"Spitters Health +10%",
				() => { ModifyUnit(UnitType.Spitter, health:0.10f); }),

			new EvolveData( //Damage+
				"Spitters Damage +15%",
				() => { ModifyUnit(UnitType.Spitter, damage:0.15f); }),

			new EvolveData( //Critical+
				"Spitters Critical Hit Chance +10%",
				() => { AddCriticalChance(UnitType.Spitter, 0.10f); }),
			

			//**SOLDIER**
			new EvolveData( //Harvest
				"Soldiers harvest 30 additional DNA on kill",
				() => { AddExtraHarvestDNA(UnitType.Soldier, 30); }),

			new EvolveData( //Stun
				"Soldiers first attack stuns the enemy",
				() => { EnableStun(UnitType.Soldier); }),

			new EvolveData( //Berserk
				"Soldiers gain 30% damage while below 30% health",
				() => { EnableBloodlust(UnitType.Soldier); }),

			new EvolveData( //M_Spd+
				"Soldiers Move Speed +15%",
				() => { ModifyUnit(UnitType.Soldier, moveSpeed:0.15f); }),

			new EvolveData( //A_Spd+
				"Soldiers Attack Speed +5%",
				() => { ModifyUnit(UnitType.Soldier, attackSpeed:0.15f); }),

			new EvolveData( //HP+
				"Soldiers Health +20%",
				() => { ModifyUnit(UnitType.Soldier, health:0.20f); }),

			new EvolveData( //Damage+
				"Soldiers Damage +10%",
				() => { ModifyUnit(UnitType.Soldier, damage:0.10f); }),

			new EvolveData( //Critical+
				"Soldiers Critical Hit Chance +10%",
				() => { AddCriticalChance(UnitType.Soldier, 0.10f); }),


			//**DEFENDER**
			new EvolveData(//Mitigate
				"Defenders take 15% less damage",
				() => { AddArmorModifier(UnitType.Defender, 0.15f); }),

			new EvolveData(//Martyr
				"Defenders explode on death, damaging enemies for 30% of their health",
				() => { EnableKamikase(UnitType.Defender); }),

			new EvolveData(//Nullfiy
				"Defenders gain a chance to ignore an attack",
				() => { EnableNullify(UnitType.Defender); }),

			new EvolveData( //M_Spd+
				"Defenders Move Speed +15%",
				() => { ModifyUnit(UnitType.Defender, moveSpeed:0.15f); }),

			new EvolveData( //A_Spd+
				"Defenders Attack Speed +10%",
				() => { ModifyUnit(UnitType.Defender, attackSpeed:0.10f); }),

			new EvolveData( //HP+
				"Defenders Health +20%",
				() => { ModifyUnit(UnitType.Defender, health:0.20f); }),

			new EvolveData( //Damage+
				"Defenders Damage +10%",
				() => { ModifyUnit(UnitType.Defender, damage:0.10f); }),

			new EvolveData( //Critical+
				"Defenders Critical Hit Chance +5%",
				() => { AddCriticalChance(UnitType.Defender, 0.5f); }),


			//**GENERAL**
			new EvolveData(//DNA+
				"DNA generation is increased by 100 per minute",
				() => { IncreaseDNAGeneration(100); }),

			new EvolveData(//HiveHeal
				"Your hive restores 50% hp",
				() => { RepairBase(0.5f); }),

			new EvolveData(//HiveThorns
				"Hive Thorns: Melee Attackers take 15% damage per hit.",
				() => { AddBaseThorns(0.15f); }),

			new EvolveData(//ALL_HP+
				"All units Health +15%",
				() => { ModifyUnit(UnitType.Defender, health:0.15f);
					ModifyUnit(UnitType.Soldier, health:0.15f);
					ModifyUnit(UnitType.Spitter, health:0.15f);}),

			new EvolveData( //ALL_M_Spd+
				"All Units Move Speed +15%",
				() => { ModifyUnit(UnitType.Defender, moveSpeed:0.15f);
						ModifyUnit(UnitType.Soldier, moveSpeed:0.15f);
						ModifyUnit(UnitType.Spitter, moveSpeed:0.15f);}),

			new EvolveData( //ALL_A_Spd+
				"All Units Attack Speed +15%",
				() => { ModifyUnit(UnitType.Defender, attackSpeed:0.15f);
						ModifyUnit(UnitType.Soldier, attackSpeed:0.15f);
						ModifyUnit(UnitType.Spitter, attackSpeed:0.15f);}),

			new EvolveData( //ALL_Damage+
				"All Units Damage +15%",
				() => { ModifyUnit(UnitType.Defender, damage:0.15f);
						ModifyUnit(UnitType.Soldier, damage:0.15f);
						ModifyUnit(UnitType.Spitter, damage:0.15f);}),

			new EvolveData( //ALL_Critical+
				"All Units Critical Hit Chance +10%",
				() => { AddCriticalChance(UnitType.Defender, 0.10f);
						AddCriticalChance(UnitType.Soldier, 0.10f);
						AddCriticalChance(UnitType.Spitter, 0.10f); }),



			};
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	static int GetCost(int level) => 100 + (level*50);


	//----------------------------------------------------------------------------------------------------------------------------------<


	#endregion


	//----------------------------------------------------------------------------------------------------------------------------------<


	private Player player;

    // How many evolutions the player has gone through so far
    private int nextEvolution = 0;
	public bool HasFinished => nextEvolution >= TREE.Length;


	//----------------------------------------------------------------------------------------------------------------------------------<


	public Evolution(Player player) {
		this.player = player;
		AssignData();
		GenerateTree();

		Game.Current.UI.UpdateEvolutionCost(GetEvolutionCost(), player.PlayerID);
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Processes the evolution path the player has taken
	/// </summary>
	/// <param name="player"></param>
	/// <param name="evolution">The evolution path</param>
	public void Evolve(int option) {
		if (Game.Current.Freeze) return;

		if (nextEvolution >= TREE.Length) return;// else already fully evolved

		if (player.DNA >= TREE[nextEvolution].Cost)
		{
			if (option == 0) DATA[TREE[nextEvolution].Option1].Func();
			else if (option == 1) DATA[TREE[nextEvolution].Option2].Func();

			player.ChangeDNA(-TREE[nextEvolution].Cost);

			nextEvolution += 1;

			if (nextEvolution < TREE.Length) {
				Game.Current.UI.UpdateEvolutionText(GetEvolutionText(0), player.PlayerID, 0);
				Game.Current.UI.UpdateEvolutionText(GetEvolutionText(1), player.PlayerID, 1);
				Game.Current.UI.UpdateEvolutionCost(GetEvolutionCost(), player.PlayerID);
			} else {
				Game.Current.UI.UpdateEvolutionText("Evolution tree complete", player.PlayerID, 0);
				Game.Current.UI.UpdateEvolutionText("Evolution tree complete", player.PlayerID, 1);
				Game.Current.UI.DisableEvolutionButtons(player.PlayerID);
			}

			ChangeSprites();
		}
        else
        {
			// not enough DNA to evolve
        }
    }


	//----------------------------------------------------------------------------------------------------------------------------------<


	public int GetEvolutionCost() => TREE[nextEvolution].Cost;


	//----------------------------------------------------------------------------------------------------------------------------------<


	public string GetEvolutionText(int option) => option == 0  ? DATA[TREE[nextEvolution].Option1].Text : DATA[TREE[nextEvolution].Option2].Text;


	//----------------------------------------------------------------------------------------------------------------------------------<


	#region Evolution Upgrade Methods


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Changes a specified unit type's modifiers
	/// </summary>
	/// <param name="unitType"></param>
	/// <param name="moveSpeed"></param>
	/// <param name="attackSpeed"></param>
	/// <param name="damage"></param>
	/// <param name="health"></param>
	/// <param name="dna"></param>
	private void ModifyUnit(UnitType unitType, float moveSpeed = 0, float attackSpeed = 0,
		float damage = 0, float health = 0)
	{
		UnitModifiers modifier = player.GetModifierReference(unitType);
		modifier.MoveSpeed *= (1 + moveSpeed);
		modifier.AttackSpeed *= (1 + attackSpeed);
		modifier.Damage *= (1 + damage);
		modifier.Health *= (1 + health);
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// [For Spitters] Defines how many enemies a spitter's projectile will pass through.
	/// </summary>
	/// <param name="unitType"></param>
	/// <param name="ranged"></param>
	private void RangedAttack(UnitType unitType, int passCount)
	{
		UnitModifiers modifier = player.GetModifierReference(unitType);
		modifier.RangedPassCount = passCount;
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	private void ChangeSprites()
    {
		// Get All player units and change sprites
		foreach (Transform child in player.PlayerUnitObjects.transform)
		{
			child.gameObject.GetComponent<UnitController>().SetSprite(nextEvolution);
		}

	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Adds a percent of the base's max health [100], back onto base's current health.
	/// </summary>
	/// <param name="percent"></param>
	void RepairBase(float percent) => player.Base.Repair(percent);


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Flicks the toggle for the bloodlust feature on a specified unit type.
	/// </summary>
	/// <param name="unitType"></param>
	void EnableBloodlust(UnitType unitType) {
		UnitModifiers mod = player.GetModifierReference(unitType);
		mod.Bloodlust = true;
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Add to the damage reduction mulitplier for a specified unit type.
	/// </summary>
	/// <param name="unitType"></param>
	/// <param name="addedPercent"></param>
	void AddArmorModifier(UnitType unitType, float addedPercent) {
		UnitModifiers mod = player.GetModifierReference(unitType);
		mod.Armor = Mathf.Clamp01(mod.Armor + addedPercent);
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Add to the chances that the unit will do a critical attack.
	/// </summary>
	/// <param name="unitType"></param>
	/// <param name="addedPercent"></param>
	void AddCriticalChance(UnitType unitType, float addedPercent) {
		UnitModifiers mod = player.GetModifierReference(unitType);
		mod.CriticalChance = Mathf.Clamp01(mod.CriticalChance + addedPercent);
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Player earns extra amount of dna when a unit of specified type kills and enemy unit.
	/// </summary>
	/// <param name="unitType"></param>
	/// <param name="amount"></param>
	void AddExtraHarvestDNA(UnitType unitType, int amount) {
		UnitModifiers mod = player.GetModifierReference(unitType);
		mod.ExtraDNAHarvest += amount;
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	void IncreaseDNAGeneration(int increase) {
		player.DNAPerMinute += increase;
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	void AddBaseThorns(float damagePercent)
	{
		player.Base.ReflectedDamage += damagePercent;
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	void EnableKamikase(UnitType unitType) {
		UnitModifiers mod = player.GetModifierReference(unitType);
		mod.Kamikaze = true;
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	void EnableHealingFrontAlly(UnitType unitType) {
		UnitModifiers mod = player.GetModifierReference(unitType);
		mod.HealFrontAlly = true;
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	void EnableAttackSpeedAllyBuff() {
		UnitModifiers mod = player.GetModifierReference(UnitType.Spitter);
		mod.BuffFrontAlly = true;
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	void EnableStun(UnitType unitType)
	{
		UnitModifiers mod = player.GetModifierReference(unitType);
		mod.StunNextAttackAcquired = true;
	}

	void EnableNullify(UnitType unitType)
    {
		UnitModifiers mod = player.GetModifierReference(unitType);
		mod.BlockEnemy = true;
		mod.BlockEnemyChance = 0.15f;
    }

	public int GetEvolutionLevel() => nextEvolution;

	#endregion
}


//----------------------------------------------------------------------------------------------------------------------------------<


#region Evolve Data Structures


//----------------------------------------------------------------------------------------------------------------------------------<


public struct EvolveData {

	public string Text;
	public Action Func;


	public EvolveData(string text, Action func) {
		Text = text;
		Func = func;
	}

}


//----------------------------------------------------------------------------------------------------------------------------------<


public struct EvolveLevel {

	public int Cost;

	public int Option1;
	public int Option2;

	public EvolveLevel(int cost, int op1, int op2) {
		Cost = cost;
		Option1 = op1;
		Option2 = op2;
	}

}


#endregion