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

    public void Evolve()
    {
        Debug.Log(Evolutions);
        if (Evolutions < Evolution1Text.Count - 1)
            Evolutions += 1;
        // else already fully evolved
    }

    public string GetEvolutionText(int evolution) { return evolution == 0  ? 
            Evolution1Text[Evolutions] : Evolution2Text[Evolutions]; }
}
