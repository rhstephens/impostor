using System;
using UnityEngine;

public class ImpostorAcademy : Academy {

    [HideInInspector]
    public GameObject[] agents;

    public override void AcademyStep() {
        // scoreText.text = string.Format(@"Score: {0}", totalScore);
    }
}