using UnityEngine;
using System.Collections;

public class MoveToNextSceneScript : MonoBehaviour {

    public float timeToNextScene = 0.1f;
    public int levelToChangeTo;


	void Start () {
        Invoke("MoveToNextScene", timeToNextScene);
	}

    void MoveToNextScene() {
        Application.LoadLevel(levelToChangeTo);
    }
}
