using UnityEngine;
using System.Collections;

public class PickUpEffectMover : MonoBehaviour {

    private Vector3 endLoc;
    public float moveAmount = 21f;
    public float speed = 15f;

    void Start ()
    {
	
	}

    void OnEnable()
    {
        endLoc = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z - moveAmount);
    }

    void Update()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, endLoc, step);
    }
}
