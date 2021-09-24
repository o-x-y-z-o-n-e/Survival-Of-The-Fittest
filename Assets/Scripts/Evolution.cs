using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evolution
{
    private List<string> Evolution1Text = new List<string>()
                { "Spitters now hit 2 enemies but have 50% damage", 
                    "Soldiers run 5% faster",
                    "DNA generation is 5% faster",
                    "Defenders have 20% damage reduction",
                    "Enemies that attack your hive take 5% damage per hit",
                    "Soldiers have 50% extra damage, but 50% slower attack speed",
                    "Workers have extra speed"};
    private List<string> Evolution2Text = new List<string>()
                { "Spitters now heal the front ally for 10% of damage dealt",
                    "Defenders have 5% more hp",
                    "Soldiers give DNA on kill",
                    "Your hive restores 10% hp",
                    "All units have 5% more hp",
                    "Spitters increase nearby units damage by 10%",
                    "When a defender dies, it heals 10% of its hp as damage to the front 2 enemies" };

    // How many evolutions the player has gone through so far
    private int Evolutions = 0;

    private UnitController[] Units;

    public Evolution()
    {
        Units = GameObject.FindObjectsOfType<UnitController>();
    }


    /// <summary>
    /// Processes the evolution path the player has taken
    /// </summary>
    /// <param name="player"></param>
    /// <param name="evolution">The evolution path</param>
    public void Evolve(int player, int evolution)
    {
        if (Evolutions < Evolution1Text.Count - 1)
            Evolutions += 1;
        // else already fully evolved

        if (evolution == 0)
        {
            switch(Evolutions)
            {
                case 1:
                    IncreaseSpreadAndDecreaseDamage(player, "Spitter");
                    break;
                case 2:
                    IncreaseUnitSpeed(player, 0.05f, "Soldier");
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
            }
        }
        else if(evolution == 1)
        {
            switch(Evolutions)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
            }
        }

		Game.Current.UI.UpdateEvolutionText(GetEvolutionText(evolution), player, evolution);
    }

    public string GetEvolutionText(int evolution) { return evolution == 0  ? 
            Evolution1Text[Evolutions] : Evolution2Text[Evolutions]; }


    /*
     *  EVOLUTION UPGRAGE METHODS
     */


    /// <summary>
    /// Decreases given units' damage and increases range
    /// </summary>
    /// <param name="player"></param>
    private void IncreaseSpreadAndDecreaseDamage(int player, string unitName)
    {
        // TODO: make unit hit 2 enimies

        foreach (UnitController unit in Units)
        {
            if (unit.name.Contains(unitName) && unit.GetUnitOwner().playerID == player)
            {
                unit.SetUnitDamage(unit.GetUnitDamage() / 2);
            }
        }
    }

    /// <summary>
    /// Increases given units' speed
    /// </summary>
    /// <param name="player"></param>
    /// <param name="speedIncrease">(percentage in decimal)</param>
    /// <param name="Unit"></param>
    private void IncreaseUnitSpeed(int player, float speedIncrease, string unitName)
    {
        foreach (UnitController unit in Units)
        {
            if (unit.name.Contains(unitName) && unit.GetUnitOwner().playerID == player)
            {
                unit.SetUnitSpeed(unit.GetUnitSpeed() * 1.05f);
            }
        }
    }
}
