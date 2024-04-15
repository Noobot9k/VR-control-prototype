using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor.Presets; //disabled due to UnityEditor namespace

public class HandInteractiveHandler : MonoBehaviour {

    public enum HandSideENUM { Left, Right }
    public HandSideENUM HandSide = HandSideENUM.Left;
    public GameObject JointParent;
    //public Preset GripPreset; //disabled due to UnityEditor namespace
    ConfigurableJoint joint;

    PhysicsInteractable HeldItem;

    PhysicsInteractable ClosestObject = null;
    float ClosestDistance = 100;

    void GripClosed() {
        if(ClosestObject) {
            HeldItem = ClosestObject;
            if(joint == null) joint = JointParent.AddComponent<ConfigurableJoint>();
            //GripPreset.ApplyTo(joint); //disabled due to UnityEditor namespace
            //SetTargetRotation(joint, new Quaternion(), Quaternion.LookRotation(Random.onUnitSphere));
            joint.targetRotation = Quaternion.Inverse(Quaternion.Inverse(JointParent.transform.rotation) * HeldItem.transform.rotation);
            //HeldItem.transform.rotation = JointParent.transform.rotation;
            HeldItem.gameObject.layer = LayerMask.NameToLayer("IgnorePlayerPhysics");
            Rigidbody rb = HeldItem.GetComponent<Rigidbody>();
            joint.connectedBody = rb;
            HeldItem.OnPickedUp();
        }
    }
    void GripOpened() {
        if(HeldItem) {
            HeldItem.OnReleased();
            HeldItem = null;
            Destroy(joint);
        }
    }
    private void OnTriggerStay(Collider other) {
        PhysicsInteractable interactable = other.GetComponent<PhysicsInteractable>();
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if(interactable && rb) {
            Vector3 closestPoint = other.ClosestPoint(transform.position);
            float distance = (closestPoint - transform.position).magnitude;
            if (distance <= ClosestDistance) {
                ClosestObject = interactable;
                ClosestDistance = distance;
            }
        }
    }
    private void FixedUpdate() {
        ClosestObject = null;
        ClosestDistance = 100;
    }
    private void OnEnable() {
        if(HandSide == HandSideENUM.Left) {
            InputManager.LeftGripPulled.AddListener(GripClosed);
            InputManager.LeftGripReleased.AddListener(GripOpened);
        } else {
            InputManager.RightGripPulled.AddListener(GripClosed);
            InputManager.RightGripReleased.AddListener(GripOpened);
        }
    }
    private void OnDisable() {
        if(HandSide == HandSideENUM.Left) {
            InputManager.LeftGripPulled.RemoveListener(GripClosed);
            InputManager.LeftGripReleased.RemoveListener(GripOpened);
        }
    }

    /// <summary>
    /// Sets the target rotation of the joint to be the given rotation relative to the original rotation
    /// </summary>
    /// <param name="joint">The joint whose target rotation is to be set</param>
    /// <param name="currentRotation">The orientation you would like the joint to be in</param>
    /// <param name="originalRotation">The original orientation of the joint</param>
    public static void SetTargetRotation(ConfigurableJoint joint, Quaternion currentRotation, Quaternion originalRotation) {
        joint.targetRotation = Quaternion.identity * (originalRotation * Quaternion.Inverse(currentRotation));
    }
}
