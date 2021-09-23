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

	public Text Player1DNAText;
	public Text Player2DNAText;


	//----------------------------------------------------------------------------------------------------------------------------------<


	void Awake() {
		//Reset which screens are active.
		HUD.gameObject.SetActive(true);
		FinishScreen.gameObject.SetActive(false);
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