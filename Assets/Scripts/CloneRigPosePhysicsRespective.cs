using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneRigPosePhysicsRespective : MonoBehaviour {

    public GameObject SourceRig;
    public GameObject TargetPhysicsRig;

    List<OriginalRotationDouble> OriginalRotations = new List<OriginalRotationDouble>();

    void Start() {

        void getOriginalPosition(Transform obj) {

            ConfigurableJoint joint = obj.GetComponent<ConfigurableJoint>();
            if (joint)
                OriginalRotations.Add(new OriginalRotationDouble(joint, obj.localRotation));

            foreach(Transform Child in obj) {
                getOriginalPosition(Child);
            }
        }

        getOriginalPosition(TargetPhysicsRig.transform);
    }
    void Update() {

        itterate(SourceRig.transform, TargetPhysicsRig.transform);
        
    }
    void itterate(Transform sourceParent, Transform targetParent) {
        ConfigurableJoint joint = targetParent.GetComponent<ConfigurableJoint>();
        if(joint)
            ClonePose(sourceParent, joint);

        foreach(Transform targetChild in targetParent) {
            foreach(Transform sourceChild in sourceParent) {
                if(targetChild.name == sourceChild.name) {
                    itterate(sourceChild, targetChild);
                }
            }
        }
    }
    void ClonePose(Transform source, ConfigurableJoint target) {
        //Quaternion globalRotation = target.transform.parent.rotation * source.localRotation;
        //Quaternion connectedSpaceRotation = Quaternion.Inverse(target.connectedBody.transform.rotation) * globalRotation;
        target.targetRotation = source.localRotation; //connectedSpaceRotation;

        Quaternion originalLocalRotation = new Quaternion();
        foreach(OriginalRotationDouble originalRotation in OriginalRotations) {
            if(originalRotation.Joint == target) originalLocalRotation = originalRotation.OriginalRotation;
        }
        SetTargetRotation(target, source.localRotation, originalLocalRotation);
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
class OriginalRotationDouble {
    public ConfigurableJoint Joint;
    public Quaternion OriginalRotation;

    public OriginalRotationDouble(ConfigurableJoint j, Quaternion rot) {
        Joint = j;
        OriginalRotation = rot;
    }
}