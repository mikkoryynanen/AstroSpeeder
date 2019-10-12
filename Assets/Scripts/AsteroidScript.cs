using UnityEngine;
using System.Collections;

public class AsteroidScript : MonoBehaviour
{

    //public GameObject destroyParticle;

    //float movementSpeed = 20.0f;

    float randomRotationSpeedX;
    float randomRotationSpeedY;
    float randomRotationSpeedZ;

    bool isDone = false;

    void Start()
    {
        //Randomize size and rotation
        //      float randomSize = Random.Range(3f, 4.5f);
        float randomSize = Random.Range(3, 3.5f);
        //Debug.Log(randomSize);
        float randomRotation = Random.Range(-359, 359);

        //Randomize rotation direction
        //randomRotationSpeedX = Random.Range(30f, 100f);
        //randomRotationSpeedY = Random.Range(30f, 100f);
        //randomRotationSpeedZ = Random.Range(30f, 100f);

        randomRotationSpeedX = Random.Range(25f, 50f);
        randomRotationSpeedY = Random.Range(25f, 50f);
        randomRotationSpeedZ = Random.Range(25f, 50f);

        transform.localScale = new Vector3(randomSize, randomSize, randomSize);
        transform.Rotate(randomRotation, randomRotation, randomRotation);
    }

    void Update()
    {
        //Rotating
        transform.Rotate(new Vector3(randomRotationSpeedX * Time.deltaTime, randomRotationSpeedY * Time.deltaTime, randomRotationSpeedZ * Time.deltaTime));
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Deadzone")
        {
            gameObject.SetActive(false);
        }
    }
}
