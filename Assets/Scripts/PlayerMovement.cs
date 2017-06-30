﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerMovement : MonoBehaviour {

    public float walkSpeed = 3f;
    public float runSpeed = 6f;

    Vector3 velocity;

    PlayerController con;

	// Use this for initialization
	void Start() {
        con = GetComponent<PlayerController>();
	}
	
	void Update() {
        Vector3 prevPos = transform.position;

        // Draw gun (with right-click)
        if (Input.GetMouseButtonUp(1)) {
            con.DrawGun();
        }

        // Check direction and return if none has been pressed
        Vector2 direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Calculate movement speeds
        float speed = calculateSpeed();

        velocity.x = direction.x * speed;
        velocity.y = direction.y * speed;
        
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
