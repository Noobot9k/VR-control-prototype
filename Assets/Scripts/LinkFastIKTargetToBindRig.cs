using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkFastIKTargetToBindRig : MonoBehaviour {

    public DitzelGames.FastIK.FastIKFabric LeftHandLink;
    public DitzelGames.FastIK.FastIKFabric RightHandLink;

    BindRigToVR BindRig;
    VRRigAutoPole AutoPole;

    void Start() {
        BindRig = GetComponent<BindRigToVR>();
        AutoPole = GetComponent<VRRigAutoPole>();
    }
    void Update() {
        if(LeftHandLink) LeftHandLink.Target = BindRig.leftHandObj;
        if(RightHandLink) RightHandLink.Target = BindRig.rightHandObj;
        if(AutoPole) {
            if(AutoPole.LeftPole)
                LeftHandLink.Pole = AutoPole.LeftPole;
            if(AutoPole.RightPole)
                RightHandLink.Pole = AutoPole.RightPole;
        }
    }
}
