using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Main level UI (HUD) controller class. This governs screen controls within a level.
/// </summary>
public class GameUI : MonoBehaviour {


	[Header("Screens/Menus")]
	public RectTransform FinishScreen;
	public RectTransform HUD;
	public RectTransform PauseScreen;


	[Space]

	[Header("Finish Elements")]

	public Text FinishText;

	[Space]

	[Header("HUD Elements")]

	[Space]

	public Text TimeDisplay;

	[Space]

	public Button Player1Evolve1Button;
	public Button Player1Evolve2Button;
	public Button Player2Evolve1Button;
	public Button Player2Evolve2Button;

	[Space]

	public Text Player1Evolve1Text;
	public Text Player1Evolve2Text;
	public Text Player2Evolve1Text;
	public Text Player2Evolve2Text;

	[Space]

	public Text Player1DNAText;
	public Text Player2DNAText;


	[Space]

	[Header("Misc Elements")]
	public Text CountdownText;




	//----------------------------------------------------------------------------------------------------------------------------------<


	void Awake() {
		//Reset which screens are active.
		HUD.gameObject.SetActive(true);
		FinishScreen.gameObject.SetActive(false);
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	private void Start() {
		// Initially update evolution buttons' text
		UpdateEvolutionText(Game.Current.Player1.Evolutions.GetEvolutionText(0), 0, 0);
		UpdateEvolutionText(Game.Current.Player1.Evolutions.GetEvolutionText(1), 0, 1);
		UpdateEvolutionText(Game.Current.Player2.Evolutions.GetEvolutionText(0), 1, 0);
		UpdateEvolutionText(Game.Current.Player2.Evolutions.GetEvolutionText(1), 1, 1);
	}


    //----------------------------------------------------------------------------------------------------------------------------------<


    /// <summary>
    /// Displays the time at the top center of the screen.
    /// </summary>
    /// <param name="time">Time (in seconds).</param>
    public void UpdateTime(float time) {
		string ss = ((int)(time % 60)).ToString("00");
		string mm = ((int)(Mathf.Floor(time / 60) % 60)).ToString("00");
		string hh = ((int)(Mathf.Floor(time / 60 / 60))).ToString("00");

		TimeDisplay.text = "Time: " + hh + ":" + mm + ":" + ss;
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	public void ShowHUD() {
		HUD.gameObject.SetActive(true);
		FinishScreen.gameObject.SetActive(false);
		PauseScreen.gameObject.SetActive(false);
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Display finish sequence screen (once game is finished).
	/// </summary>
	public void ShowFinishScreen(string text) {
		HUD.gameObject.SetActive(false);
		FinishScreen.gameObject.SetActive(true);
		PauseScreen.gameObject.SetActive(false);

		FinishText.text = text;
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	public void ShowPauseScreen() {
		HUD.gameObject.SetActive(false);
		FinishScreen.gameObject.SetActive(false);
		PauseScreen.gameObject.SetActive(true);
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Displays DNA Count in player HUD for specified player. (0 = player one), (1 = player two/AI).
	/// </summary>
	/// <param name="count"></param>
	/// <param name="player">(0 = player one), (1 = player two/AI)</param>
	public void UpdateDNA(int count, int player) => (player == 0 ? Player1DNAText : Player2DNAText).text = "DNA: " + count.ToString();


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// 
	/// </summary>
	/// <param name="text"></param>
	/// <param name="player"></param>
	/// <param name="evolution"></param>
	public void UpdateEvolutionText(string text, int player, int evolution) => 
		(player == 0 ? (evolution == 0 ? Player1Evolve1Text : Player1Evolve2Text) 
		: (evolution == 0 ? Player2Evolve1Text : Player2Evolve2Text)).text = text;


	//----------------------------------------------------------------------------------------------------------------------------------<


	public void ShowCountdown(float t) {
		if(t <= 0) {
			CountdownText.gameObject.SetActive(false);
			return;
		}

		CountdownText.text = ((int)t).ToString();
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	public void DisableEvolutionButtons(int player) {
		if(player == 0) {
			Player1Evolve1Button.interactable = false;
			Player1Evolve2Button.interactable = false;
		} else {
			Player2Evolve1Button.interactable = false;
			Player2Evolve2Button.interactable = false;
		}
	} 


	//----------------------------------------------------------------------------------------------------------------------------------<


	#region Input Events


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Is called by Path (Surface/Tunnel) button click event.
	/// </summary>
	public void OnPlayer1PathClick(int path) => Game.Current.Player1.SelectPath(path);


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Is called by Path (Surface/Tunnel) button click event.
	/// </summary>
	public void OnPlayer2PathClick(int path) => Game.Current.Player2.SelectPath(path);


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Is called by Unit (Soldier/Spitter/Tank) button click event.
	/// </summary>
	/// <param name="unit">(0 = soldier), (1 = spitter), (2 = tank)</param>
	public void OnPlayer1UnitClick(int unit) {
		//Hook into base script
		Game.Current.Player1.Base.SpawnUnit((UnitType)unit, Game.Current.Player1.SelectedPath);
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Is called by Unit (Soldier/Spitter/Tank) button click event.
	/// </summary>
	/// <param name="unit">(0 = soldier), (1 = spitter), (2 = tank)</param>
	public void OnPlayer2UnitClick(int unit) {
		//Hook into base script
		Game.Current.Player2.Base.SpawnUnit((UnitType)unit, Game.Current.Player2.SelectedPath);
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Is called by Evolution button click event
	/// </summary>
	/// <param name="evo"></param>
	public void OnPlayer1EvolutionClick(int option) => Game.Current.Player1.Evolutions.Evolve(option);


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Is called by Evolution button click event
	/// </summary>
	/// <param name="evo"></param>
	public void OnPlayer2EvolutionClick(int option) => Game.Current.Player2.Evolutions.Evolve(option);


	//----------------------------------------------------------------------------------------------------------------------------------<


	#endregion
}