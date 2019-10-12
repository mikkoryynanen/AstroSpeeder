using UnityEngine;
using System.Collections;

public class TextureOffset : MonoBehaviour {
    public float scrollSpeedY = 0.5f;
    public float scrollSpeedX = 0.5f;

    Renderer rend;


    void Start() {
        rend = GetComponent<Renderer>();
    }

    void Update() {
        float offsetX = Time.time * scrollSpeedX;
        float offsetY = Time.time * scrollSpeedY;

        rend.material.SetTextureOffset("_MainTex", new Vector2(offsetX, offsetY * PlayerController.forwardMovementSpeed / 100));
    }
}
