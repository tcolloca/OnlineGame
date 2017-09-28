using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput {

	//Vector2 lookDir; //si moving esta en true mandar 2 floats
	public bool up;
	public bool down;
	public bool left;
	public bool right;
	public bool shoot;

	public PlayerInput() {
	}

	public PlayerInput(bool up, bool down, bool left, bool right, bool shoot) {
		this.up = up;
		this.down = down;
		this.left = left;
		this.right = right;
		this.shoot = shoot;
	}

	public void Save(BitBuffer buffer) {
		buffer.PutBit (up);
		buffer.PutBit (down);
		buffer.PutBit (left);
		buffer.PutBit (right);
		buffer.PutBit (shoot);
	}
		
	public void Load(BitBuffer buffer) {
		up = buffer.GetBit ();
		down = buffer.GetBit ();
		left = buffer.GetBit ();
		right = buffer.GetBit ();
		shoot = buffer.GetBit ();
	}
}
