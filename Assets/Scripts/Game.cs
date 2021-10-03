using System.Collections;
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
	public Transform ProjectileContainer;


	//----------------------------------------------------------------------------------------------------------------------------------<


	float time;
	bool hasStarted; public bool HasStarted => hasStarted;
	bool isFinished; public bool IsFinished => isFinished;
	bool isPaused; public bool IsPaused => isPaused;

	public bool Freeze => !hasStarted || isFinished || isPaused;


	float startCountdown = 3;


	//----------------------------------------------------------------------------------------------------------------------------------<


	void Awake() {
		instance = this;
    }


	//----------------------------------------------------------------------------------------------------------------------------------<


	void Update() {
		if (!hasStarted) {
			startCountdown -= Time.deltaTime;

			if(startCountdown <= 0) {
				startCountdown = 0;
				hasStarted = true;
			}

			UI.ShowCountdown(startCountdown);

		} else if (isFinished) {

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
		string displayText = "Game Over";

		if(Player1.IsAI && Player2.IsAI) {
			if (Player2.Base.IsDestroyed) displayText = "Bot 1 Won!";
			else if (Player1.Base.IsDestroyed) displayText = "Bot 2 Won!";
		} else if(!Player1.IsAI && !Player2.IsAI) {
			if (Player2.Base.IsDestroyed) displayText = "Player 1 Won!";
			else if (Player1.Base.IsDestroyed) displayText = "Player 2 Won!";
		} else {
			Player player = Player1.IsAI ? Player2 : Player1;
			Player ai = Player1.IsAI ? Player1 : Player2;

			if (ai.Base.IsDestroyed) displayText = "You Won!";
			else if (player.Base.IsDestroyed) displayText = "You Lose :(";
		}

		UI.ShowFinishScreen(displayText);
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Load back into the main menu.
	/// </summary>
	public void Leave() => SceneManager.LoadScene(0);


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Returns a list of units that are currently spawned and belong to specified player.
	/// </summary>
	/// <param name="playerID">(0 = Player 1), (1 = Player 2/AI)</param>
	/// <returns></returns>
	public UnitController[] GetExistingUnits(int playerID) => (playerID == 0 ? Player1.Base.UnitContainer : Player2.Base.UnitContainer).GetComponentsInChildren<UnitController>();


	//----------------------------------------------------------------------------------------------------------------------------------<


	public void Pause(bool pause) {
		isPaused = pause;

		if (isPaused) UI.ShowPauseScreen();
		else UI.ShowHUD();
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	void OnDestroy() {
		instance = null;
	}
}
