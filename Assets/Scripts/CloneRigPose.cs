using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneRigPose : MonoBehaviour {

    public GameObject SourceRig;
    public GameObject TargetRig;

    void Start() {

    }
    void Update() {

        itterate(SourceRig.transform, TargetRig.transform);
        
    }
    void itterate(Transform sourceParent, Transform targetParent) {
        ClonePose(sourceParent, targetParent);

        foreach(Transform targetChild in targetParent) {
            foreach(Transform sourceChild in sourceParent) {
                if(targetChild.name == sourceChild.name) {
                    itterate(sourceChild, targetChild);
                }
            }
        }
    }
    void ClonePose(Transform source, Transform target) {
        target.localRotation = source.localRotation;
    }
}
