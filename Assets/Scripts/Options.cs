using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Options {

	public static InputBindings Input = new InputBindings();

}

public class InputBindings {

	public static KeyCode Pause				= KeyCode.Escape;

	//

	public static KeyCode Player1Working	= KeyCode.None;
	public static KeyCode Player1Soldier	= KeyCode.Q;
	public static KeyCode Player1Spitter	= KeyCode.W;
	public static KeyCode Player1Defender	= KeyCode.E;

	public static KeyCode Player1Surface	= KeyCode.Alpha1;
	public static KeyCode Player1Tunnel		= KeyCode.Alpha2;

	public static KeyCode Player1Evolve1	= KeyCode.Alpha3;
	public static KeyCode Player1Evolve2	= KeyCode.Alpha4;

	//

	public static KeyCode Player2Working	= KeyCode.None;
	public static KeyCode Player2Soldier	= KeyCode.P;
	public static KeyCode Player2Spitter	= KeyCode.O;
	public static KeyCode Player2Defender	= KeyCode.I;
	
	public static KeyCode Player2Surface	= KeyCode.Alpha0;
	public static KeyCode Player2Tunnel		= KeyCode.Alpha9;

	public static KeyCode Player2Evolve1	= KeyCode.Alpha8;
	public static KeyCode Player2Evolve2	= KeyCode.Alpha7;
}