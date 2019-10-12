using UnityEngine;
using System.Collections;

public class PooledObject : MonoBehaviour {

    public float lifeTime;


    void OnEnable() {
        Invoke("Destroy", lifeTime);
    }

    void Destroy() {
        gameObject.SetActive(false);
    }

    void OnDisable() {
        CancelInvoke();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Deadzone")
        {
            Destroy();
        }
    }
}
