using System;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class ImpostorAgent : Agent {

    public float walkSpeed = 3f;
    public float runSpeed = 6f;

    Brain studentBrain;
    Vector2 direction = Vector2.zero;
    Vector3 velocity;
    PlayerController con;


    // Essentially Start()
    public override void InitializeAgent() {
        base.InitializeAgent();
        con = GetComponent<PlayerController>();
        GameObject studentObj = GameObject.Find("StudentBrain");
        if (studentObj != null) {
            studentBrain = studentObj.GetComponent<Brain>();
            GiveBrain(studentBrain);
        } else {
            throw new UnityAgentsException("Ryan: Can't find initial brain object!");
        }
    }

    public void Update() {
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

    // This method gets called by the Brain to collect a list of floating point numbers.
    // These numbers are added to the Observations and used by the Brain to make a Decision
    public override void CollectObservations() {
        float rayDistance = 50f;
        float[] rayAngles = { 20f, 90f, 160f, 45f, 135f, 70f, 110f };
        string[] detectableObjects = { "AI", "Player", "wall", "badBanana", "frozenAgent" };
        AddVectorObs(1f);
    }

    // Upon making a Decision, the Brain calls this method with the appropriate action Vector
    public override void AgentAction(float[] act, string textAction) {
        // clamp returned action to nearest direction value
        direction.x = Mathf.RoundToInt(Mathf.Clamp(act[0], -1f, 1f));
        direction.y = Mathf.RoundToInt(Mathf.Clamp(act[1], -1f, 1f));

        AddReward(1f);
    }

    // This will return a list of floats corresponding to what our Agent can "see".
    // In other words, it is the distance from the Agent to some other visible object type
    float[] GetLocalObservations() {
        return new float[]{1};
    }

    // Calculate speed based off of certain factors
    float calculateSpeed() {
        if (Input.GetKey(KeyCode.LeftShift)) {
            return runSpeed;
        }
        return walkSpeed;
    }
}