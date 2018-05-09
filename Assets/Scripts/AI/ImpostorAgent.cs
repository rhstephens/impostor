using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class ImpostorAgent : Agent {

    public float walkSpeed = 1.5f;
    public float minTime = 0.4f;

    Brain studentBrain;
    RayPerception2D rayComponent;
    Vector2 direction = Vector2.zero;
    Vector3 velocity;
    PlayerController con;

    float counter = 0f;

    // Essentially Start()
    public override void InitializeAgent() {
        base.InitializeAgent();
        con = GetComponent<PlayerController>();
        GameObject studentObj = GameObject.Find("AlwaysMoveBrain");
        if (gameObject.tag == "Player") {
            studentObj = GameObject.Find("TeacherBrain");
        }
        if (studentObj != null) {
            studentBrain = studentObj.GetComponent<Brain>();
            GiveBrain(studentBrain);
        } else {
            throw new UnityAgentsException("Ryan: Can't find initial brain object!");
        }

        rayComponent = GetComponent<RayPerception2D>();
    }

    public void Update() {
        counter += Time.deltaTime;
        velocity.x = direction.x * walkSpeed;
        velocity.y = direction.y * walkSpeed;

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
        float rayDistance = 55f;
        float[] rayAngles = { 20f, 70f, 90f, 110f, 160f, 45f, 135f, -20f, -70f, -90f, -110f, -160f, -45f, -135f };
        string[] detectableObjects = { "Player", "Unpassable" };
        AddVectorObs(rayComponent.Perceive(10f, rayAngles, detectableObjects));
    }

    // Upon making a Decision, the Brain calls this method with the appropriate action Vector
    public override void AgentAction(float[] act, string textAction) {
        // clamp returned action to nearest direction value
        int xDir = Mathf.RoundToInt(Mathf.Clamp(act[0], -1f, 1f));
        int yDir = Mathf.RoundToInt(Mathf.Clamp(act[1], -1f, 1f));

        // In order to prevent "spastic" movements, we need to hold the previously set direction for some amount of time
        //    before changing it. It works best if this "set amount of time" is randomly determined within an interval
        if (counter >= minTime || gameObject.tag == "Player") { // Player tag is used for training
            direction.x = xDir;
            direction.y = yDir;
            counter = 0;
            minTime = UnityEngine.Random.Range(0.2f, 0.75f);
        }

        AddReward(1f);
    }

    // This will return a list of floats corresponding to what our Agent can "see".
    // In other words, it is the distance from the Agent to some other visible object type
    float[] GetLocalObservations() {
        return new float[]{1};
    }
}