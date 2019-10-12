using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public Transform player;

    public float cameraMovementSpeed = 5f;
    public float offsetZ;
    public float offsetY;
    public float xClamp;
    public float cameraSpeed = 5f;

    public float impact = 0f;

    Camera cam;
        

    void Awake() {
        cam = GetComponent<Camera>();
    }

    void LateUpdate() {
        //transform.position = new Vector3(Mathf.Lerp(transform.position.x, player.transform.position.x, cameraSpeed * Time.deltaTime), offsetY, player.position.z + offsetZ);
        transform.position = new Vector3(player.transform.position.x, offsetY, player.position.z + offsetZ);
    }

    void Update() {
        if(impact > 0) {
            cam.fieldOfView = Random.Range(58, 62);
            impact -= Time.deltaTime;
        }
        else {
            impact = 0f;
            cam.fieldOfView = 60;
        }
    }
}
