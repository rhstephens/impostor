using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Decision component that contains logic for making decisions based off of heuristics.
/// The goal of this Decision is to always have the Agent moving, in semi-random directions
/// </summary>
class MovementDecision : MonoBehaviour, Decision {
    // Store floats for use in later decision making
    List<float> MakeMemory(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory) {
        return null;
    }

    // Make a decision based off of heuristics
    float[] Decide(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory) {
        return new float[]{0f};
    }
}