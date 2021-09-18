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


}