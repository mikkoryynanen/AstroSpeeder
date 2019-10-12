using UnityEngine;
using System.Collections;

public class Collectable : MonoBehaviour {
    
    public float movementSpeed = 20.0f;

	void Update () {
        transform.Translate(-Vector3.forward * movementSpeed * Time.deltaTime);
	}

    void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Deadzone") {
            gameObject.SetActive(false);
        }
    }
}
