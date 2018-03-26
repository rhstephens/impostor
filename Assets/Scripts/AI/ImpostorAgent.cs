using System;
using UnityEngine;

public class ImpostorAgent : Agent {
    Vector2 agentPos;
    Vector3 velocity;
    PlayerController con;


    // Essentially Start()
    public override void InitializeAgent() {
        base.InitializeAgent();
        con = GetComponent<PlayerController>();
    }

    // This method gets called by the Brain to collect a list of floating point numbers.
    // These numbers are added to the Observations and used by the Brain to make a Decision
    public override void CollectObservations() {
        float rayDistance = 50f;
        float[] rayAngles = { 20f, 90f, 160f, 45f, 135f, 70f, 110f };
        string[] detectableObjects = { "AI", "Player", "wall", "badBanana", "frozenAgent" };
        AddVectorObs(GetLocalObservations());
    }

    // Upon making a Decision, the Brain calls this method with the appropriate action Vector
    public override void AgentAction(float[] act, string textAction) {

    }

    // This will return a list of floats corresponding to what our Agent can "see".
    // In other words, it is the distance from the Agent to some other visible object type
    float[] GetLocalObservations() {
        return new float[]{};
    }
}