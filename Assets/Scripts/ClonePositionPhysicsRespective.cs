using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClonePositionPhysicsRespective : MonoBehaviour {

    // references
    public Transform TargetTransform;
    Rigidbody rb;

    public float MaxForce = 5;
    public float ForceMultiplier = 5;
    public float MaxTorque = 5;
    public float TorqueMultiplier = 5;

    public float maxAngularVelocity = 100;

    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = maxAngularVelocity;
    }
    private void FixedUpdate() {
        Vector3 toTargetVector = TargetTransform.position - transform.position;
        Vector3 forceVector = Vector3.ClampMagnitude(toTargetVector * ForceMultiplier, MaxForce);
        rb.AddForce(forceVector, ForceMode.Acceleration);

        Quaternion differenceRotation = TargetTransform.rotation * Quaternion.Inverse(transform.rotation);
        differenceRotation.ToAngleAxis(out float differenceAngle, out Vector3 differenceAxis);
        differenceAxis = (Vector3.Cross(-TargetTransform.forward, transform.forward) + Vector3.Cross(-TargetTransform.up, transform.up)).normalized;
        differenceAngle = Quaternion.Angle(TargetTransform.rotation, transform.rotation);
        Vector3 torqueVector = Vector3.ClampMagnitude(differenceAxis * differenceAngle * TorqueMultiplier, MaxTorque);
        rb.AddTorque(torqueVector, ForceMode.Acceleration);
    }
}
