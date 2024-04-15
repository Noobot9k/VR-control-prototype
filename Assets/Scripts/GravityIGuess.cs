using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityIGuess : MonoBehaviour {

    public float GravityMultiplier = 1;
    public bool UseGravity = true;
    Rigidbody reggiesBody;

    private void Start() {
        reggiesBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        if (UseGravity && !reggiesBody.IsSleeping())
            reggiesBody.AddForce(Physics.gravity * GravityMultiplier, ForceMode.Acceleration);
    }
}
