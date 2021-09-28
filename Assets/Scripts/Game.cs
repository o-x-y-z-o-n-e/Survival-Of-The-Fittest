﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Core class of each 'level'. This controls the state of the level, eg. Timer, score, etc...
/// </summary>
public class Game : MonoBehaviour {


	static Game instance; public static Game Current => instance;
	public GameUI UI;
	[Space]
	public Player Player1;		//Main person who is playing.
	public Player Player2;      //Used for AI or for second RL player.
	[Space]
	public Transform Player1Units;
	public Transform Player2Units;


	//----------------------------------------------------------------------------------------------------------------------------------<


	float time;
	bool isFinished; public bool IsFinished => isFinished;
	bool isPaused; public bool IsPaused => isPaused;


	//----------------------------------------------------------------------------------------------------------------------------------<


	void Awake() {
		instance = this;
    }


	//----------------------------------------------------------------------------------------------------------------------------------<


	void Update() {
		if (isFinished) {

		} else {

			if (Input.GetKeyDown(Options.Input.Pause)) Pause(!isPaused);

			if (!isPaused) {
				time += Time.deltaTime;
				UI.UpdateTime(time);
			}
		}
    }


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Ends the game and checks for a win state. eg. Who's base got destroyed, who has the most points, etc...
	/// </summary>
	public void End() {
		if (isFinished) return;

		isFinished = true;

		//Check for who won here...

		UI.ShowFinishScreen();
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Load back into the main menu.
	/// </summary>
	public void Leave() {
		instance = null;
		SceneManager.LoadScene(0);
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Returns a list of units that are currently spawned and belong to specified player.
	/// </summary>
	/// <param name="playerID">(0 = Player 1), (1 = Player 2/AI)</param>
	/// <returns></returns>
	public UnitController[] GetExistingUnits(int playerID) => (playerID == 0 ? Player1Units : Player2Units).GetComponentsInChildren<UnitController>();


	//----------------------------------------------------------------------------------------------------------------------------------<


	public void Pause(bool pause) {
		isPaused = pause;

		if (isPaused) UI.ShowPauseScreen();
		else UI.ShowHUD();
	}
}
