using UnityEngine;
using System.Collections;

public class InputEvent {
	
	public Axis axis;
	public float val;
	
	/**
	 * Creates an InputEvent in the specified axis.
	 * the value gives additional information:
	 * Direction: -1 (left); +1 (right)
	 * Throttle: -1 (brake); +1 (gas)
	 * Powerup: -1 (use powerup in negative); 0 (don't use/deactivate); +1 (use Powerup positive)
	 */
	public InputEvent(Axis axis, float val) {
		this.axis = axis;
		this.val = val;
	}
	
	public enum Axis {
		Null, Direction, Throttle, Powerup 
	}
}
