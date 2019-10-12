using UnityEngine;
using System.Collections;

public class PickUpMovement : MonoBehaviour
{

    private Vector3 endLoc;
    public float moveAmount = 21f; // Should be pretty much the same as pickUpSpawns and players distance in z
    public float speed = 15f;

    void Start()
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

    void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.tag == "Deadzone")
            gameObject.SetActive(false);
    }
}
