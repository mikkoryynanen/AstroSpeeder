using UnityEngine;
using System.Collections;

public class ManagerMover : MonoBehaviour {

    public Transform player;
    public float zOffset;

    void Start()
    {
        if (player == null)
            player = GameObject.Find("Player").transform;
    }

    void LateUpdate () {
        transform.position = new Vector3(0, transform.position.y, player.transform.position.z + zOffset);
	}
}
