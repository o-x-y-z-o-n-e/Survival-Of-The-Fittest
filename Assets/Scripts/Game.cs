using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

	[Space]
	public Sprite player1Win;
	public Sprite player2Win;
	public Sprite bot1Win;
	public Sprite bot2Win;



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


	void Start() {

		//For testing... This will change to fit the Start Menu options.
		Player1.SetMode(Options.Player1Mode);
		Player2.SetMode(Options.Player2Mode);
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

			CheckHotkeyInput();
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
		Sprite displaySprite = null;

		if(Player1.IsAI && Player2.IsAI) {
			if (Player2.Base.IsDestroyed) displaySprite = bot1Win;//"Bot 1 Won!";
			else if (Player1.Base.IsDestroyed) displaySprite = bot2Win;//"Bot 2 Won!";
		} else if(!Player1.IsAI && !Player2.IsAI) {
			if (Player2.Base.IsDestroyed) displaySprite = player1Win;//"Player 1 Won!";
			else if (Player1.Base.IsDestroyed) displaySprite = player2Win;//"Player 2 Won!";
		} else {
			Player player = Player1.IsAI ? Player2 : Player1;
			Player ai = Player1.IsAI ? Player1 : Player2;

			if (ai.Base.IsDestroyed) displaySprite = player1Win;//"You Won!";
			else if (player.Base.IsDestroyed) displaySprite = player2Win;//"You Lose :(";
		}

		UI.ShowFinishScreen(displaySprite);
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


	//----------------------------------------------------------------------------------------------------------------------------------<


	void CheckHotkeyInput() {
		// Player 1 Hotkeys
		if (!Player1.IsAI) {
			// Unit Spawning
			if (Input.GetKeyDown(Options.Input.Player1Soldier))
				GameUI.ClickButton(UI.Player1SoldierButton.gameObject);

			if (Input.GetKeyDown(Options.Input.Player1Spitter))
				GameUI.ClickButton(UI.Player1SpitterButton.gameObject);

			if (Input.GetKeyDown(Options.Input.Player1Defender))
				GameUI.ClickButton(UI.Player1DefenderButton.gameObject);


			// Path Selection
			if (Input.GetKeyDown(Options.Input.Player1Surface))
				GameUI.ClickButton(UI.Player1Path1Button.gameObject);

			if (Input.GetKeyDown(Options.Input.Player1Tunnel))
				GameUI.ClickButton(UI.Player1Path2Button.gameObject);


			// Evolution Selection
			if (Input.GetKeyDown(Options.Input.Player1Evolve1))
				GameUI.ClickButton(UI.Player1Evolve1Button.gameObject);

			if (Input.GetKeyDown(Options.Input.Player1Evolve2))
				GameUI.ClickButton(UI.Player1Evolve2Button.gameObject);
		}

		// Player 2 Hotkeys
		if (!Player2.IsAI) {
			// Unit Spawning
			if (Input.GetKeyDown(Options.Input.Player2Soldier))
				GameUI.ClickButton(UI.Player2SoldierButton.gameObject);

			if (Input.GetKeyDown(Options.Input.Player2Spitter))
				GameUI.ClickButton(UI.Player2SpitterButton.gameObject);

			if (Input.GetKeyDown(Options.Input.Player2Defender))
				GameUI.ClickButton(UI.Player2DefenderButton.gameObject);


			// Path Selection
			if (Input.GetKeyDown(Options.Input.Player2Surface))
				GameUI.ClickButton(UI.Player2Path1Button.gameObject);

			if (Input.GetKeyDown(Options.Input.Player2Tunnel))
				GameUI.ClickButton(UI.Player2Path2Button.gameObject);


			// Evolution Selection
			if (Input.GetKeyDown(Options.Input.Player2Evolve1))
				GameUI.ClickButton(UI.Player2Evolve1Button.gameObject);

			if (Input.GetKeyDown(Options.Input.Player2Evolve2))
				GameUI.ClickButton(UI.Player2Evolve2Button.gameObject);
		}
	}
}


//----------------------------------------------------------------------------------------------------------------------------------<


public enum Path {
	Surface = 0,
	Tunnels = 1,
}