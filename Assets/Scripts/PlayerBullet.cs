using UnityEngine;
using System.Collections;

public class PlayerBullet : MonoBehaviour {
    
    public float speed = 5f;

    static public GameObject target;


	void Update () {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        transform.LookAt(target.transform);
	}

    void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Deadzone") {
            Destroy(gameObject);
        }

        if(other.gameObject.tag == "Enemy") {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
