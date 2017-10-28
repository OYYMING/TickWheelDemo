using UnityEngine;
using System.Collections;

public static class DebugUtils {
	
	public static void Log (string log) {
        Debug.Log(log);
    }

	public static void LogError (string log) {
        Debug.LogError(log);
    }
}
