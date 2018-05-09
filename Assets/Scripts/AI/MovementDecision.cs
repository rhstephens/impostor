using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Decision component that contains logic for making decisions based off of heuristics.
/// The goal of this Decision is to always have the Agent moving, in semi-random directions
/// </summary>
class MovementDecision : MonoBehaviour, Decision {
    public float stopChance = 0.2f;

    int xDir;
    int yDir;
    float timeSinceMove = 0f;
    float minTime = 0.4f;
    float stopAmount = 0;

    // Don't need to use this func
    public List<float> MakeMemory(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory) {
        return null;
    }

    // Make a decision based off of heuristics
    public float[] Decide(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory) {
        if (stopAmount > 0) {
            return new float[]{0, 0};
        }
        if (timeSinceMove >= minTime) {
            // occassionally force AI to stop for a certain amount of time
            ShouldStop();
            xDir = UnityEngine.Random.Range(-1, 2);
            yDir = UnityEngine.Random.Range(-1, 2);
            timeSinceMove = 0;
            minTime = UnityEngine.Random.Range(0.1f, 0.75f);
        }
        return new float[]{xDir, yDir};
    }

    public void Update() {
        timeSinceMove += Time.deltaTime;
        if (stopAmount > 0) {
            stopAmount -= Time.deltaTime;
        }
    }

    public void ShouldStop() {
        float rand = UnityEngine.Random.Range(0f, 1f);
        if (rand < stopChance) {
            stopAmount = UnityEngine.Random.Range(0.8f, 2.5f);
        }
    }
}