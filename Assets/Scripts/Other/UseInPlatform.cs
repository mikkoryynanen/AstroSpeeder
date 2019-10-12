using UnityEngine;
using System.Collections;

public class UseInPlatform : MonoBehaviour {
	void Start () {
        #if UNITY_ANDROID
        this.gameObject.SetActive(false);
        #endif

        #if UNITY_IOS
        this.gameObject.SetActive(true);
        #endif
    }
}
