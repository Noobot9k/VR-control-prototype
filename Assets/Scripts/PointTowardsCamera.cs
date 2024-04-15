using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointTowardsCamera : MonoBehaviour {

    public enum FacingType { FaceTowards, AlignWith };
    public FacingType facingMode = FacingType.AlignWith;

    private void LateUpdate() {
        if(facingMode == FacingType.FaceTowards) {
            transform.LookAt(Camera.main.transform.position);
        } else if(facingMode == FacingType.AlignWith) {
            transform.rotation = Camera.main.transform.rotation * Quaternion.Euler(0,180,0);
        }
    }
}
