using UnityEngine;
using System.Collections.Generic;

public class PlatformDepenence : MonoBehaviour {
    public List<GameObject> iosObjects;
    public List<GameObject> androidObjects;

    void OnLevelWasLoaded(int level) {
        if(level == 1) {
            #if UNITY_IOS
            //Set iosObjects to active
            for(int i = 0; i < iosObjects.Count; i++) {
                iosObjects[i].SetActive(true);
            }
            //Set Android objects to false
            for(int i = 0; i < androidObjects.Count; i++) {
                androidObjects[i].SetActive(false);
            }
            #elif UNITY_ANDROID || UNITY_EDITOR
            //Set iosObjects to false
            for(int i = 0; i < iosObjects.Count; i++) {
                iosObjects[i].SetActive(false);
            }
            //Set Android objects to true
            for(int i = 0; i < androidObjects.Count; i++) {
                androidObjects[i].SetActive(true);
            }
            #endif
        }

        else if(level == 2) {
         #if UNITY_IOS
            //Set iosObjects to active
            for (int i = 0; i < iosObjects.Count; i++) {
                iosObjects[i].SetActive(true);
            }
            #elif UNITY_ANDROID
            //Set iosObjects to false
            for(int i = 0; i < iosObjects.Count; i++) {
                iosObjects[i].SetActive(false);
            }
            #endif
        }
    }
}
