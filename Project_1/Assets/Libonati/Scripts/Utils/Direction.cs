using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class Direction{
	public bool up = true;
	public bool down = true;
	public bool left = true;
	public bool right = true;

	public Direction(bool startValue){
		setAll (startValue);
	}
	public Direction(){

	}
	public void setAll(bool b){
		up = down = left = right = b;
	}
}
