using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Options {


	//Runtime options
	public static Difficulty Player1Difficulty = Difficulty.Casual;
	public static Difficulty Player2Difficulty = Difficulty.Casual;
	public static PlayerMode Player1Mode = PlayerMode.Real;
	public static PlayerMode Player2Mode = PlayerMode.Real;



	//Both volumes have a domain of [0, 1]
	public static float VolumeFX = 1f;
	public static float VolumeMusic = 1f;


	public static InputBindings Input = new InputBindings();


	//----------------------------------------------------------------------------------------------------------------------------------<


	public static float GetLinearDifficulty(int playerID) => Mathf.InverseLerp(1, 5, (int)(playerID == 0 ? Player1Difficulty : Player2Difficulty));


	//----------------------------------------------------------------------------------------------------------------------------------<


	public static void ReadFromDisk() {
		if (!File.Exists(GetOptionsPath())) return;

		string[] lines = File.ReadAllLines(GetOptionsPath());

		for(int i = 0; i < lines.Length; i++) {
			bool isHeader = false;

			isHeader = lines[i].StartsWith("[");

			if(!isHeader) {
				string[] split = lines[i].Trim().Split('=');

				string id = split[0].ToLower();
				string value = ""; if (split.Length > 1) value = split[1];

				switch (id) {
					case "music": {
						float fltValue = VolumeMusic ;
						float.TryParse(value, out fltValue);
						VolumeMusic = Mathf.Clamp01(fltValue);
						break;
					}

					case "fx": {
						float fltValue = VolumeFX;
						float.TryParse(value, out fltValue);
						VolumeFX = Mathf.Clamp01(fltValue);
						break;
					}
				}
			}
		}
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	public static void WriteToDisk() {
		string[] lines = new string[3];

		lines[0] = "[Audio]";
		lines[1] = "music=" + VolumeMusic.ToString();
		lines[2] = "fx=" + VolumeFX.ToString();

		File.WriteAllLines(GetOptionsPath(), lines);
	}


	//----------------------------------------------------------------------------------------------------------------------------------<


	static string GetOptionsPath() => Application.persistentDataPath + "/options.ini";


}

public class InputBindings {

	public  KeyCode Pause			= KeyCode.Escape;

	//-------------------------------------------<

	public  KeyCode Player1Worker	= KeyCode.None;
	public  KeyCode Player1Soldier	= KeyCode.A;
	public  KeyCode Player1Spitter	= KeyCode.S;
	public  KeyCode Player1Defender	= KeyCode.D;

	public  KeyCode Player1Surface	= KeyCode.Q;
	public  KeyCode Player1Tunnel	= KeyCode.W;

	public  KeyCode Player1Evolve1	= KeyCode.E;
	public  KeyCode Player1Evolve2	= KeyCode.R;

	//-------------------------------------------<

	public  KeyCode Player2Worker	= KeyCode.None;
	public  KeyCode Player2Soldier	= KeyCode.L;
	public  KeyCode Player2Spitter	= KeyCode.K;
	public  KeyCode Player2Defender	= KeyCode.J;
	
	public  KeyCode Player2Surface	= KeyCode.P;
	public  KeyCode Player2Tunnel	= KeyCode.O;

	public  KeyCode Player2Evolve1	= KeyCode.U;
	public  KeyCode Player2Evolve2	= KeyCode.I;
}


public enum Difficulty {
	Baby = 1,			//Very easy
	Gentle = 2,			//Easy
	Casual = 3,			//Normal/Medium
	Sweating = 4,		//Hard
	Barrage = 5,		//Crazy
}