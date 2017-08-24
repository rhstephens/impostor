using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerMotor : MonoBehaviour {

    public float walkSpeed = 3f;
    public float runSpeed = 6f;

	//TODO: Remove this once model has been trained successfully
	FeatureExporter fe = new FeatureExporter();
	float featureRate = 1f / 5f;
	public float sinceMotion = 0;
	public float sinceDirection = 0;
	public bool inMotion = false;
	Vector2 prevDirection;
	Vector2 curDirection;

    Vector3 velocity;

    PlayerController con;

	// Use this for initialization
	void Start() {
        con = GetComponent<PlayerController>();
		InvokeRepeating("GenerateFeature", 1f, featureRate);
	}
	
	void Update() {
        Vector3 prevPos = transform.position;

        // Check direction and return if none has been pressed
        Vector2 direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		//TODO: Remove this once model has been trained successfully
		curDirection = direction;
		sinceMotion += Time.deltaTime;
		sinceDirection += Time.deltaTime;

		if (inMotion && (direction == Vector2.zero)) {
			sinceMotion = 0;
		} else if (!inMotion && (direction != Vector2.zero)) {
			sinceMotion = 0;
		}

		inMotion = (direction != Vector2.zero);

		if (prevDirection != direction) {
			sinceDirection = 0;
		}
		prevDirection = direction;


        // Calculate movement speeds
        float speed = calculateSpeed();

        velocity.x = direction.x * speed;
        velocity.y = direction.y * speed;
        
        // Rotate and Move player accordingly
        if (con.gunDrawn) {
            Vector3 lookPos = GetComponentInChildren<Camera>().ScreenToWorldPoint(Input.mousePosition) - transform.position;
            con.Rotate(lookPos);
        } else {
            con.Rotate(velocity * Time.deltaTime);
        }
        con.Move(velocity * Time.deltaTime);
    }

    float calculateSpeed() {
        if (Input.GetKey(KeyCode.LeftShift)) {
            return runSpeed;
        }

        return walkSpeed;
    }

	void GenerateFeature() {
		Feature f = Feature.GeneratePlayerFeatures(gameObject, inMotion, sinceMotion, sinceDirection, curDirection);
		Debug.Log(f.W);
		fe.AddFeature(f);
	}

	void OnApplicationQuit() {
		fe.ExportFeatures();
	}
}
