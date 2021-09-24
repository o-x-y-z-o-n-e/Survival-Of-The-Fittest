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

	[Space]

	public Text TimeDisplay;

	[Space]
	[Header("HUD Elements")]

	public Player Player1;
	public Player Player2;

	public Text Player1Evolve1Text;
	public Text Player1Evolve2Text;
	public Text Player2Evolve1Text;
	public Text Player2Evolve2Text;

	public Text Player1DNAText;
	public Text Player2DNAText;


	//----------------------------------------------------------------------------------------------------------------------------------<


	void Awake() {
		//Reset which screens are active.
		HUD.gameObject.SetActive(true);
		FinishScreen.gameObject.SetActive(false);
	}

    private void Start()
    {
		// Initially update evolution buttons' text
		UpdateEvolutionText(Player1.evolution.GetEvolutionText(0), 0, 0);
		UpdateEvolutionText(Player1.evolution.GetEvolutionText(1), 0, 1);
		UpdateEvolutionText(Player2.evolution.GetEvolutionText(0), 1, 0);
		UpdateEvolutionText(Player2.evolution.GetEvolutionText(1), 1, 1);
	}

    private void Update()
    {
		// updates the UI for the players' DNA
		UpdateDNA(Player1.DNA, 0);
		UpdateDNA(Player2.DNA, 1);
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


	/*
	 * On click methods for evolution buttons
	 */

	/// <summary>
	/// Player 1 Evolution 1 on click method
	/// </summary>
	public void UpdatePlayer1Evolution1()
	{
		Player1.evolution.Evolve(0, 0);
		UpdateEvolutionText(Player1.evolution.GetEvolutionText(0), 0, 0);
		UpdateEvolutionText(Player1.evolution.GetEvolutionText(1), 0, 1);
	}

	/// <summary>
	/// Player 1 Evolution 2 on click method
	/// </summary>
	public void UpdatePlayer1Evolution2()
	{
		Player1.evolution.Evolve(0, 1);
		UpdateEvolutionText(Player1.evolution.GetEvolutionText(1), 0, 1);
		UpdateEvolutionText(Player1.evolution.GetEvolutionText(0), 0, 0);
	}

	/// <summary>
	/// Player 2 Evolution 1 on click method
	/// </summary>
	public void UpdatePlayer2Evolution1()
	{
		Player2.evolution.Evolve(1, 0);
		UpdateEvolutionText(Player1.evolution.GetEvolutionText(0), 1, 0);
		UpdateEvolutionText(Player1.evolution.GetEvolutionText(1), 1, 1);
	}

	/// <summary>
	/// Player 2 Evolution 2 on click method
	/// </summary>
	public void UpdatePlayer2Evolution2()
	{
		Player2.evolution.Evolve(1, 1);
		UpdateEvolutionText(Player1.evolution.GetEvolutionText(1), 1, 1);
		UpdateEvolutionText(Player1.evolution.GetEvolutionText(0), 1, 0);
	}

	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Display finish sequence screen (once game is finished).
	/// </summary>
	public void ShowFinishScreen() {
		HUD.gameObject.SetActive(false);
		FinishScreen.gameObject.SetActive(true);
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Displays DNA Count in player HUD for specified player. (0 = player one), (1 = player two/AI).
	/// </summary>
	/// <param name="count"></param>
	/// <param name="player">(0 = player one), (1 = player two/AI)</param>
	public void UpdateDNA(int count, int player) => (player == 0 ? Player1DNAText : Player2DNAText).text = "DNA: " + count.ToString();

	public void UpdateEvolutionText(string text, int player, int evolution) => 
		(player == 0 ? (evolution == 0 ? Player1Evolve1Text : Player1Evolve2Text) 
		: (evolution == 0 ? Player2Evolve1Text : Player2Evolve2Text)).text = text;

	//----------------------------------------------------------------------------------------------------------------------------------<


	#region Input Events


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Is called by Path (Surface/Tunnel) button click event.
	/// </summary>
	/// <param name="path">(0 = surface/top), (1 = tunnel/bottom)</param>
	/// <param name="player">(0 = player one), (1 = player two/AI)</param>
	public void OnPathSelectClick(int path, int player) {
		//Hook into base script
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Is called by Unit (Soldier/Spitter/Tank) button click event.
	/// </summary>
	/// <param name="unit">(0 = soldier), (1 = spitter), (2 = tank)</param>
	/// <param name="player">(0 = player one), (1 = player two/AI)</param>
	public void OnUnitSpawnClick(int unit, int player) {
		//Hook into base script
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	#endregion
}