using UnityEngine;
using System.Collections;

public class DestroyParticle : MonoBehaviour {

    public bool destroyOverTime = false;
    public bool destroyGameObject = false;
    public float lifeTime = 3f;

    bool startingGridsInstantiated = false;
    public float destroyGameObjectSpawnTime = 20f;

    ParticleSystem particleSystem;

    //movement Variables
    private Vector3 endLoc;
    public float moveAmount = 21f; // Should be pretty much the same as pickUpSpawns and players distance in z
    public float speed = 15f;
    public bool isShieldEffect;

    void Start () {
        Invoke("StartingGridsDone", 20f);

        if(!destroyGameObject) {
            if(destroyOverTime) {
                Destroy(gameObject, lifeTime);
            }
            else {
                particleSystem = GetComponent<ParticleSystem>();

                if(isShieldEffect == false)
                    Destroy(gameObject, particleSystem.duration);
                if (isShieldEffect == true)
                    Destroy(gameObject, 5);
            }
        }

        if(destroyGameObject) {
            Destroy(gameObject, destroyGameObjectSpawnTime);
        }
	}

    void StartingGridsDone() {
        destroyGameObjectSpawnTime = 3f;
    }

    void OnEnable()
    {
        endLoc = new Vector3(transform.localPosition.x, transform.localPosition.y-2, transform.localPosition.z - moveAmount);
    }

    void Update()
    {
        if (isShieldEffect == false)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, endLoc, step);
        }
    }
}
