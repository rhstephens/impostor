using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach this to agents to enable "local perception" via the use of ray casts
/// directed outward from the agent on the X/Y plane. A modification of the ML-Agents version
/// </summary>
public class RayPerception2D : MonoBehaviour {
    List<float> perceptionBuffer = new List<float>();
    Vector3 endPosition;
    RaycastHit2D hit;

    /// <summary>
    /// Creates perception vector to be used as part of an observation of an agent.
    /// Each angle given corresponds to detectableObjects.Length + 2 number of floats
    /// The two extra floats correspond to:
    ///   1) is 1f if the raycast didn't hit any object
    ///   2) is the relative distance if we did hit an object
    /// </summary>
    /// <returns>The partial vector observation corresponding to the set of rays</returns>
    /// <param name="rayDistance">Radius of rays</param>
    /// <param name="rayAngles">Anlges of rays (starting from (1,0) on unit circle).</param>
    /// <param name="detectableObjects">List of tags which correspond to object types agent can see</param>
    public List<float> Perceive(float rayDistance, float[] rayAngles, string[] detectableObjects) {
        perceptionBuffer.Clear();
        // For each ray sublist stores categorial information on detected object
        // along with object distance.
        foreach (float angle in rayAngles) {
            endPosition = transform.TransformDirection(
                PolarToCartesian(rayDistance, angle));
            if (Application.isEditor) {
                Debug.DrawRay(transform.position + new Vector3(0f, 0f, 0f), endPosition, Color.black, 0.01f, true);
            }
            float[] subList = new float[detectableObjects.Length + 2];
            // todo, make this raycast only hit certain objects
            hit = Physics2D.Raycast(transform.position, endPosition, rayDistance);
            if (hit) {
                for (int i = 0; i < detectableObjects.Length; i++) {
                    if (hit.collider.gameObject.CompareTag(detectableObjects[i])) {
                        subList[i] = 1;
                        subList[detectableObjects.Length + 1] = hit.distance / rayDistance;
                        break;
                    }
                }
            } else {
                subList[detectableObjects.Length] = 1f;
            }
            perceptionBuffer.AddRange(subList);
        }
        return perceptionBuffer;
    }

    /// <summary>
    /// Converts polar coordinate to cartesian coordinate in 2D space.
    /// </summary>
    public static Vector3 PolarToCartesian(float radius, float angle) {
        float x = radius * Mathf.Cos(DegreeToRadian(angle));
        float y = radius * Mathf.Sin(DegreeToRadian(angle));
        return new Vector3(x, y, 0f);
    }

    /// <summary>
    /// Converts degrees to radians.
    /// </summary>
    public static float DegreeToRadian(float degree) {
        return degree * Mathf.PI / 180f;
    }
}
