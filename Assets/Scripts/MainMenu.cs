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


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Will load and start a new level. At the moment there is only one level. So no level select is implemented yet. Called by the 'Play' button on the main menu.
	/// </summary>
	public void OnPlayClick() {
		SceneManager.LoadScene(LEVEL_SCENE_NAME);
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
