using UnityEngine;

public static class Extensions {
	public static float Mod(this float a, float b) {
    	return a - b * Mathf.Floor(a / b);
	}
}