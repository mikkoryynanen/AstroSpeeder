using UnityEngine;
using UnityEditor;
using System.Collections;

public class ResetPlayerPrefsMenu : MonoBehaviour {

    [MenuItem("Edit/ResetPlayerPrefs")]
    public static void ResetPlayerPrefs() {
        PlayerPrefs.DeleteAll();
        Debug.LogWarning("PlayerPrefs Reset");
    }
}
