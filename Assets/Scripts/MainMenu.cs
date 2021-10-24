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

	[Space]


	public Text SomeRandomText;

	[Space]


	public Slider MusicVolumeSlider;
	public Slider FXVolumeSlider;

	[Space]

	public AudioSource MenuMusic;


	void Awake() {
		Options.ReadFromDisk();
	}


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
		ShowSecret();
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	void Update() {
		//Nothing to see here....
		ShhhhItsASecret();
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	/// <summary>
	/// Closes the game. Called by the 'Quit' button on the main menu.
	/// </summary>
	public void OnQuitClick() => Application.Quit();


	//----------------------------------------------------------------------------------------------------------------------------------<


	public void LoadOptions() {
		MusicVolumeSlider.value = Options.VolumeMusic * 10;
		FXVolumeSlider.value = Options.VolumeFX * 10;

		MenuMusic.volume = Options.VolumeMusic;
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	public void SaveOptions() {
		Options.VolumeMusic = MusicVolumeSlider.value / 10;
		Options.VolumeFX = FXVolumeSlider.value / 10;

		MenuMusic.volume = Options.VolumeMusic;

		Options.WriteToDisk();
	}


	//----------------------------------------------------------------------------------------------------------------------------------<



	#region TOP SECRET, DO NOT PEEK

	//I told you not to look >:(

	Text verySecretText = null;
	float verySecretCounter = 60;
	const float VERY_VERY_SECRET_INTERVAL = 120;
	const float GOTTA_GO_FAST = 25;
	readonly string[] TOP_SECRET_INFO = {
		"Next up   ->   Ducks vs Cats: Battle for Quantum Supremacy",
		"To win the game, you must kill me, Jeremy Kiel!",
		"Where is the bread thief?",
		"Bob was here",
		"Make sure to take breaks, or you might... Blackout!",
		"Don't be Sklate!",
		"Let Bygones be Bygones",
		"I'm free... free Falling!",
		"Defenders like protein shakes",
		"Spitters are the most educated",
		"Soldiers are fans of reality TV",
		"How many ants does it take to change a light bulb?",
		"Supercalifragilisticexpialidocious",
		"WW91IGhhdmUgdG9vIG11Y2ggdGltZQ==", //You have too much time
		"https://bit.ly/3pujyjl", //hehe ;)
		"Party boy time!",
	};
	void ShhhhItsASecret() {
		verySecretCounter += Time.fixedDeltaTime;

		if(verySecretCounter > VERY_VERY_SECRET_INTERVAL) {
			verySecretCounter = 0;

			ShowSecret();
		}

		if (verySecretText != null) {
			verySecretText.rectTransform.localPosition -= Vector3.right * Time.fixedDeltaTime * GOTTA_GO_FAST;
			if(verySecretText.rectTransform.localPosition.x < -verySecretText.rectTransform.sizeDelta.x) {
				Destroy(verySecretText.gameObject);
				verySecretText = null;
			}
		}
	}
	void ShowSecret() {
		verySecretText = Instantiate(SomeRandomText, SomeRandomText.transform.parent);

		int i = Random.Range(0, TOP_SECRET_INFO.Length);
		verySecretText.text = TOP_SECRET_INFO[i];

		verySecretText.gameObject.SetActive(true);
	}

	#endregion
}
