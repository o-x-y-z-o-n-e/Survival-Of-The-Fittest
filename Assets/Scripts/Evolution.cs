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
			new EvolveData(
				"Spitters now hit 2 enemies but have 50% damage",
				() => { ModifyUnit(UnitType.Spitter, damage:-0.5f); RangedAttack(UnitType.Spitter, 2); }),

			new EvolveData(
				"Spitters now heal the front ally 5 hp every 1 second",
				() => { EnableHealingForntAlly(UnitType.Spitter); }),

			new EvolveData(
				"Soldiers run 5% faster",
				() => { ModifyUnit(UnitType.Soldier, moveSpeed:0.05f); }),

			new EvolveData(
				"Defenders have 5% more max hp",
				() => { ModifyUnit(UnitType.Defender, health:0.05f); }),

			new EvolveData(
				"DNA generation is increased by 100 per minute",
				() => { IncreaseDNAGeneration(100); }),

			new EvolveData(
				"Soldiers give extra DNA on kill",
				() => { AddExtraHarvestDNA(UnitType.Soldier, 10); }),

			new EvolveData(
				"Defenders have 20% damage reduction",
				() => { ModifyUnit(UnitType.Defender, damage:-0.2f); }),

			new EvolveData(
				"Your hive restores 10% hp",
				() => { RepairBase(0.1f); }),

			new EvolveData(
				"Enemies that attack your hive take 5% damage per hit",
				() => { AddBaseThorns(0.05f); }),

			new EvolveData(
				"All units have 5% more hp",
				() => { ModifyUnit(UnitType.Defender, health:0.05f);
					ModifyUnit(UnitType.Soldier, health:0.05f);
					ModifyUnit(UnitType.Spitter, health:0.05f);
					ModifyUnit(UnitType.Worker, health:0.05f); }),

			new EvolveData(
				"Soldiers have 50% extra damage, but 50% slower attack speed",
				() => { ModifyUnit(UnitType.Soldier, damage:0.5f, attackSpeed:-0.5f); }),

			new EvolveData(
				"Spitters increase nearby units damage by 10%",
				() => {  }),

			new EvolveData(
				"Workers have 10% extra speed",
				() => { ModifyUnit(UnitType.Worker, moveSpeed:0.1f); }),

			new EvolveData(
				"When a defender dies, it deals 30% of its max hp as damage to the front enemy",
				() => { EnableKamikase(UnitType.Defender); }),

			new EvolveData(
				"When a soldier attacks for the first time, the enemy unit is stunned briefly",
				() => { EnableStun(UnitType.Soldier); }),
			new EvolveData(
				"Defenders have a chance to block an attack from an enemy",
				() => { EnableNullify(UnitType.Defender); })
		};
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	static int GetCost(int level) => 400 + (level*100);


	//----------------------------------------------------------------------------------------------------------------------------------<


	#endregion


	//----------------------------------------------------------------------------------------------------------------------------------<


	private Player player;

    // How many evolutions the player has gone through so far
    private int nextEvolution = 0;


	//----------------------------------------------------------------------------------------------------------------------------------<


	public Evolution(Player player) {
		this.player = player;
		AssignData();
		GenerateTree();
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

			player.DNA -= TREE[nextEvolution].Cost;

			nextEvolution += 1;

			if (nextEvolution < TREE.Length) {
				Game.Current.UI.UpdateEvolutionText(GetEvolutionText(0), player.PlayerID, 0);
				Game.Current.UI.UpdateEvolutionText(GetEvolutionText(1), player.PlayerID, 1);
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


	void EnableHealingForntAlly(UnitType unitType) {
		UnitModifiers mod = player.GetModifierReference(unitType);
		mod.HealFrontAlly = true;
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
    }

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