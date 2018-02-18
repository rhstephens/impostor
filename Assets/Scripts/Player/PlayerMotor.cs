using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerMotor : MonoBehaviour {

    public float walkSpeed = 3f;
    public float runSpeed = 6f;

	//TODO: Remove this once model has been trained successfully
	FeatureExporter fe;
	Train trainer;
	float featureRate = 2f / 5f;

	// 1 in the "direction" the player is facing. Index 0 is up, 1 is top-right, ... 8 is "no direction"
	int[] labelledData = new int[GameManager.YLABEL_LENGTH];

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
		fe = new FeatureExporter();
		InvokeRepeating("GenerateFeature", 1f, featureRate);
	}
	
	void Update() {
		GameManager.Instance.GeneratePlayerMatrix(gameObject);
        Vector3 prevPos = transform.position;

        // Check direction and return if none has been pressed
        Vector2 direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		//TODO: Remove this once model has been trained successfully
		if (Input.GetKeyDown(KeyCode.M)) {
			fe.ExportFeatures();
		}
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
		fe.AddPlayerMatrix(GameManager.Instance.GeneratePlayerMatrix(gameObject));
		fe.AddObstacleMatrix(GameManager.Instance.GenerateObstacleMatrix());
		fe.AddEnemyMatrix(GameManager.Instance.GenerateEnemyMatrix());

		// TODO: make this smarter (maybe. it is fine as it is currently)
		// determine labelled data from input direction
		int horiz = (int) Input.GetAxisRaw("Horizontal");
		int vert = (int) Input.GetAxisRaw("Vertical");
		if (horiz == 1) {
			labelledData[2 - vert] = 1;
		} else if (horiz == 0) {
			labelledData[2 - (vert * 2)] = 1;
		} else if (horiz == -1) {
			labelledData[6 + vert] = 1;
		}

		// for no input
		if (horiz == 0 && vert == 0) {
			for (int i = 0; i < 9; i++) {
				labelledData[i] = 0;
			}
			labelledData[8] = 1;
		}
		fe.AddLabelledData(labelledData);
	}
}
