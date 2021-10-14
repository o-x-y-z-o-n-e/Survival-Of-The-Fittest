using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Options {


	//Runtime options
	public static Difficulty Difficulty = Difficulty.Casual;
	public static PlayerMode Player1Mode = PlayerMode.Real;
	public static PlayerMode Player2Mode = PlayerMode.Real;


	public static InputBindings Input = new InputBindings();



	public static float GetLinearDifficulty() => Mathf.InverseLerp(1, 5, (int)Difficulty);

}

public class InputBindings {

	public  KeyCode Pause			= KeyCode.Escape;

	//

	public  KeyCode Player1Worker	= KeyCode.None;
	public  KeyCode Player1Soldier	= KeyCode.A;
	public  KeyCode Player1Spitter	= KeyCode.S;
	public  KeyCode Player1Defender	= KeyCode.D;

	public  KeyCode Player1Surface	= KeyCode.Q;
	public  KeyCode Player1Tunnel	= KeyCode.W;

	public  KeyCode Player1Evolve1	= KeyCode.E;
	public  KeyCode Player1Evolve2	= KeyCode.R;

	//

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