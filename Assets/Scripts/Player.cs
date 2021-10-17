using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Core class for holding Player/AI data. References to units, bases, points, etc...
/// </summary>
public class Player : MonoBehaviour {

	const float DNA_UPDATE_INTERVAL = 1f;

	public Base Base;

	[Space]

	public int DNA; // Total amount of DNA the player has gained
	public int DNAPerMinute = 600;

	[Space]

	public Evolution Evolutions;
	public UnitModifiers WorkerModifiers = new UnitModifiers();
	public UnitModifiers SoldierModifiers = new UnitModifiers();
	public UnitModifiers SpitterModifiers = new UnitModifiers();
	public UnitModifiers DefenderModifiers = new UnitModifiers();

	[Space]

	public int PlayerID;

	[Space]

	public Color Color;

	[Space]

	[SerializeField] private int evolutionWindowIncrement;
	public GameObject PlayerUnitObjects;


	//----------------------------------------------------------------------------------------------------------------------------------<


	bool isAI; public bool IsAI => isAI;
	Path selectedPath = Path.Surface; public Path SelectedPath => selectedPath;

	float remainingDNA = 0;
	float counterDNA = 0;

	float AIStartTime;
	float AIFrameInterval;

	private float nextEvolutionWindow;


	//----------------------------------------------------------------------------------------------------------------------------------<


	private void Awake() {
		Evolutions = new Evolution(this);
		nextEvolutionWindow = evolutionWindowIncrement; // + timeDelta !!!!
		AIStartTime = Time.time; 
		AIFrameInterval = Mathf.Lerp(0f, 1f, Options.GetLinearDifficulty());
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	private void OnDestroy() {
		Evolutions = null;
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	void Update() {
		if (Game.Current.Freeze) return;

		GatherDNA();

		if (isAI) UpdateAI();
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	void GatherDNA() {
		float rate = DNAPerMinute / 60;
		remainingDNA += Time.deltaTime * rate;

		counterDNA += Time.deltaTime;
		if(counterDNA >= DNA_UPDATE_INTERVAL) {
			counterDNA = 0;

			int whole = (int)remainingDNA;
			remainingDNA = remainingDNA % 1;

			ChangeDNA(whole);
		}
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	public void ChangeDNA(int amount) {
		DNA += amount;

		Game.Current.UI.UpdateDNA(DNA, PlayerID);
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	public UnitModifiers GetModifierReference(UnitType type) {
		switch(type) {
			case UnitType.Worker: {
				return WorkerModifiers;
			}
			case UnitType.Soldier: {
				return SoldierModifiers;
			}
			case UnitType.Spitter: {
				return SpitterModifiers;
			}
			case UnitType.Defender: {
				return DefenderModifiers;
			}
		}
		return null;
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	public void SelectPath(Path path) {
		selectedPath = path;
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	public void SetMode(PlayerMode mode) {
		isAI = mode == PlayerMode.AI;

		Game.Current.UI.SetPlayerControls(PlayerID, !isAI);
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	#region AI


	//----------------------------------------------------------------------------------------------------------------------------------<


	void UpdateAI() 
	{
		//Options.GetLinearDifficulty will return a value of [0, 1]. You can use this as 't' for a Lerp func to get AI frame interval with upper/lower bounds.
		
		if (Time.time - AIStartTime > AIFrameInterval)
		{
			//CheckLaneDominance();

			AIStartTime = Time.time;
        }
	}
	

    //----------------------------------------------------------------------------------------------------------------------------------<


    /// <summary>
    /// 
    /// </summary>
    void CheckLaneDominance()
    {
        int n_units = 0;
        float SLD = 0, TLD = 0; // Surface / Tunnel lane dominance

        string unitSide = PlayerID == 2 ? "Left Unit" : "Right Unit";
        int enemyLayer = LayerMask.NameToLayer(unitSide);
        List<GameObject> units = GetEnemyPlayers(enemyLayer);

        if (units.Count > 0)
        {
            foreach (GameObject unit in units)
            {
                ++n_units;
                int path = units[0].GetComponent<UnitController>().GetPath() == Path.Surface ? 0 : 1;
                if (path == 0)
                    ++SLD;
                else if (path == 1)
                    ++TLD;
            }
            CheckPriorityStates(0f, SLD / n_units, TLD / n_units);
        }
        else
        {
            CheckPriorityStates(0f, 0f, 0f);
        }
    }
	
	
	//----------------------------------------------------------------------------------------------------------------------------------<


    List<GameObject> GetEnemyPlayers(int enemyLayer)
    {
        GameObject[] enemyUnits = FindObjectsOfType<GameObject>();
		List<GameObject> retUnits = new List<GameObject>();
		for (int i=0;i<enemyUnits.Length;++i)
        {
			if (enemyUnits[i].layer == enemyLayer)
            {
				retUnits.Add(enemyUnits[i]);
            }
        }

		return retUnits;
    }
	

    //----------------------------------------------------------------------------------------------------------------------------------<


    /// <summary>
    /// Checks if the AI needs to defend or push a hard attack.
    /// </summary>
    /// <param name="timeDelta">The change in time, between each AI frame.</param>
    /// <param name="a">Surface Lane Dominance</param>
    /// <param name="b">Tunnels Lane Dominance</param>
    void CheckPriorityStates(float timeDelta, float a, float b) {
		const float RUSH_ATTACK_THRESHOLD = 0.5f;

		if(a < 0 || b < 0) {
			Path path = a < b ? Path.Surface : Path.Tunnels;
			UnitType type = PickUnitType(0.5f,0.25f,0.25f);
			Base.SpawnUnit(type, Path.Surface); // Note to Jeremy: should this be path instead of Path.Surface???
			return;
		}

		if(a > RUSH_ATTACK_THRESHOLD) {
			UnitType type = PickUnitType(0.65f,0.3f,0.05f);
			Base.SpawnUnit(type, Path.Surface);
			return;
		}

		if(b > RUSH_ATTACK_THRESHOLD) {
			UnitType type = PickUnitType(0.65f,0.3f,0.05f);
			Base.SpawnUnit(type, Path.Tunnels);
			return;
		}

		CheckEvolutionStates(timeDelta);
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	void CheckEvolutionStates(float timeDelta) {
        //This is a note for Alex: you can rename this function if you want.
        //With the evolution counter, use timeDelta instead of Time.deltaTime. Because timeDelta will account for the AI frame interval.

        if (timeDelta > nextEvolutionWindow)
        {
            if (DNA > Evolutions.GetEvolutionCost())
            {
                Evolutions.Evolve(Random.Range(0, 2));
                nextEvolutionWindow += evolutionWindowIncrement;
            }
        }
        else
        {
            Base.SpawnUnit(PickUnitType(0.5f, 0.25f, 0.25f), (Path)Random.Range(0, 2));
        }
    }


	//----------------------------------------------------------------------------------------------------------------------------------<


	UnitType PickUnitType(float soldier, float spitter, float defender) {
		soldier = Mathf.Abs(soldier);
		spitter = Mathf.Abs(spitter);
		defender = Mathf.Abs(defender);

		float m = soldier + spitter + defender;

		float t = Random.value * m;

		if (t > soldier + spitter) return UnitType.Defender;
		else if (t > soldier) return UnitType.Spitter;
		else return UnitType.Soldier;
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	#endregion

}

public enum PlayerMode {
	Real = 0,
	AI = 1,
}