using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Main menu UI script.
/// </summary>
public class MainMenu : MonoBehaviour {


	const string LEVEL_SCENE_NAME = "Level-1";


	public Dropdown Player1Mode;
	public Dropdown Player2Mode;

	public Dropdown Player1Difficulty;
	public Dropdown Player2Difficulty;


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Will load and start a new level. At the moment there is only one level. So no level select is implemented yet. Called by the 'Play' button on the main menu.
	/// </summary>
	public void OnPlayClick() {
		Options.Player1Mode = (PlayerMode)Player1Mode.value;
		Options.Player2Mode = (PlayerMode)Player2Mode.value;

		SceneManager.LoadScene(LEVEL_SCENE_NAME);
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	public void OnPlayerModeUpdate() {
		Player1Difficulty.gameObject.SetActive((PlayerMode)Player1Mode.value == PlayerMode.AI);
		Player2Difficulty.gameObject.SetActive((PlayerMode)Player2Mode.value == PlayerMode.AI);
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Opens the options sub-menu. Called by the 'Options' button on the main menu.
	/// </summary>
	public void OnOptionsClick() {

	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Closes the game. Called by the 'Quit' button on the main menu.
	/// </summary>
	public void OnQuitClick() => Application.Quit();

}
