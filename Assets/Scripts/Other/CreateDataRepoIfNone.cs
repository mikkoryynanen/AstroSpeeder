using UnityEngine;
using System.Collections;

public class CreateDataRepoIfNone : MonoBehaviour {

    public GameObject dRepo;

	void Awake ()
    {
        if (GameObject.FindWithTag("DataRepo") == null)
            Instantiate(dRepo, Vector3.zero, Quaternion.identity);
    }
}
