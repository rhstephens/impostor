using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerMovement : MonoBehaviour {

    public float walkSpeed = 3f;
    public float runSpeed = 6f;

    Vector3 velocity;
    float xAcceleration = 0.1f;
    float yAcceleration = 0.1f;
    float xSmoothing;
    float ySmoothing;
    float rotationSmoothing = 0.1f;

    PlayerController con;

	// Use this for initialization
	void Start () {
        con = GetComponent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector3 prevPos = transform.position;

        // Calculate movement speeds
        float speed = calculateSpeed();
        float xTarget = direction.x * speed;
        float yTarget = direction.y * speed;
        velocity.x = Mathf.SmoothDamp(velocity.x, xTarget, ref xSmoothing, xAcceleration);
        velocity.y = Mathf.SmoothDamp(velocity.y, yTarget, ref ySmoothing, yAcceleration);

        con.Rotate(velocity * Time.deltaTime);
        con.Move(velocity * Time.deltaTime);
    }

    float calculateSpeed() {
        if (Input.GetKey(KeyCode.LeftShift)) {
            return runSpeed;
        }

        return walkSpeed;
    }
}
