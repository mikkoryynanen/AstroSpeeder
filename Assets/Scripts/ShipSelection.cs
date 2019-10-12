using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShipSelection : MonoBehaviour {
/*  I SUPPOSE THIS CODE IS NOT IN USE?
    [Header("AudioClips")]
    AudioSource audio;
    public AudioClip buttonPressSound;
    public AudioClip shipSelectionSound;
    public AudioClip backSound;

    public float movementSpeed = 10.0f;
    static public int currentShipPosition = 0;
    public Vector3[] shipPositions;

    bool canMove = false;

    public Button leftButton;
    public Button rightButton;


	void Start () {
        audio = GetComponent<AudioSource>();

        currentShipPosition = 0;
        transform.position = shipPositions[currentShipPosition];
	}

    void Update() {
        if(canMove) {
            transform.position = Vector3.Lerp(transform.position, shipPositions[currentShipPosition], Time.deltaTime * movementSpeed);
            //Invoke("DoneMoving", 2.5f);
        }

        //Button enabling and disabling
        if(currentShipPosition <= 0) {
            leftButton.interactable = false;
        }
        else {
            leftButton.interactable = true;
        }

        if(currentShipPosition >= shipPositions.Length - 1) {
            rightButton.interactable = false;
        }
        else {
            rightButton.interactable = true;
        }
    }

    void DoneMoving() {
        canMove = false;
    }

    //Button presses
    public void OnLeftButtonPress () {
        if(currentShipPosition >= 0 && currentShipPosition <= shipPositions.Length - 1) {
            //Sound effect
            audio.PlayOneShot(shipSelectionSound);

            currentShipPosition--;
            DataRepo.selectedShipNumber--;
            canMove = true;
        }
    }

    public void OnRightButtonPress () {
        if(currentShipPosition <= shipPositions.Length -1 && currentShipPosition >= 0 ) {
            //Sound effect
            audio.PlayOneShot(shipSelectionSound);

            currentShipPosition++;
            DataRepo.selectedShipNumber++;
            canMove = true;
        }
    }

    public void SelectShip() {
        //Sound effect
        audio.PlayOneShot(buttonPressSound);

        Application.LoadLevel(2);
    }*/
}
