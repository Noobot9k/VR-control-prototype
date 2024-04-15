using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class BindRigToVR : MonoBehaviour {

    public Animator animator;

    [Tooltip("DEPRICATED! leave false.")]
    public bool ikActive = false;
    public Transform rightHandObj = null;
    public Transform leftHandObj = null;
    public Transform lookObj = null;
    public Vector3 cameraOffset = new Vector3();
    public float torsoTurnSpeedMultiplier = 4;

    public Transform headReference;
    public Transform neckReference;
    Quaternion defaultHeadRot;
    Quaternion defaultNeckRot;

    void Start() {
        if (animator == null) animator = GetComponent<Animator>();
        defaultHeadRot = headReference.localRotation;
        defaultNeckRot = neckReference.localRotation;
    }

    private void FixedUpdate() {
        if(animator) {

            float leftGripAlpha = SteamVR_Input.GetSingleAction("default", "GripSqueeze").GetAxis(SteamVR_Input_Sources.LeftHand);
            float rightGripAlpha = SteamVR_Input.GetSingleAction("default", "GripSqueeze").GetAxis(SteamVR_Input_Sources.RightHand);
            float leftTriggerAlpha = SteamVR_Input.GetSingleAction("default", "TriggerSqueeze").GetAxis(SteamVR_Input_Sources.LeftHand);
            float rightTriggerAlpha = SteamVR_Input.GetSingleAction("default", "TriggerSqueeze").GetAxis(SteamVR_Input_Sources.RightHand);

            animator.SetFloat("LeftHand_Closed", leftGripAlpha);
            animator.SetFloat("RightHand_Closed", rightGripAlpha);
            animator.SetFloat("LeftIndexFinger_Closed", leftTriggerAlpha);
            animator.SetFloat("RightIndexFinger_Closed", rightTriggerAlpha);

            //print("leftGripAlpha " + leftGripAlpha);
            //print("leftTriggerAlpha " + leftTriggerAlpha);


            //animator.SetFloat("LeftHand_Closed", Mathf.Round(Mathf.Sin(Time.time)));

            Quaternion lookRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(lookObj.forward, Vector3.up).normalized);
            Vector3 transformedCameraOffset = headReference.TransformDirection(cameraOffset);
            //Vector3 localHeadPosOffset = transform.InverseTransformPoint(animator.GetBoneTransform(HumanBodyBones.Head).position + transformedCameraOffset);
            Vector3 localHeadPosOffset = transform.InverseTransformPoint(lookRotation * Vector3.ProjectOnPlane(cameraOffset, Vector3.up) + headReference.TransformPoint(Vector3.Project(cameraOffset, Vector3.up)));
            //transform.position = lookObj.position - localHeadPosOffset;
            //transform.rotation = lookRotation;
            Vector3 averageLookVector = Vector3.ProjectOnPlane(lookObj.forward + (leftHandObj.up + rightHandObj.up)/3, Vector3.up) / 3;

            float angle = Vector3.Angle(lookObj.forward, lookRotation * Vector3.forward);
            neckReference.localRotation = defaultNeckRot;
            neckReference.rotation = Quaternion.Lerp(neckReference.rotation, lookObj.rotation * Quaternion.Euler(0, -180, 0), .5f);
            headReference.rotation = lookObj.rotation * Quaternion.Euler(0,-180,0);


            Transform headTransform = animator.GetBoneTransform(HumanBodyBones.Head);
            Vector3 viewPosition = headReference.TransformPoint(cameraOffset);
            Debug.DrawLine(viewPosition, viewPosition + Vector3.up * .1f, Color.red);
            transform.localPosition = -transform.InverseTransformPoint(viewPosition);
            transform.parent.position = lookObj.position;
            //transform.parent.rotation = lookRotation;
            transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation, Quaternion.LookRotation(averageLookVector.normalized), averageLookVector.magnitude * torsoTurnSpeedMultiplier * Time.smoothDeltaTime);
        }
    }

    void OnAnimatorIK() {
        if(animator) {

            //if the IK is active, set the position and rotation directly to the goal. 
            if(ikActive) {

                // Set the look target position, if one has been assigned
                if(lookObj != null) {
                    animator.SetLookAtWeight(1);
                    animator.SetLookAtPosition(animator.GetBoneTransform(HumanBodyBones.Head).position + lookObj.forward);
                }

                // Set the right hand target position and rotation, if one has been assigned
                if(rightHandObj != null) {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);

                    animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 1);
                    animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 1);
                    animator.SetIKHintPosition(AvatarIKHint.LeftElbow, leftHandObj.forward * -5);
                    animator.SetIKHintPosition(AvatarIKHint.RightElbow, rightHandObj.forward * -5);
                }
                if(leftHandObj != null) {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
                }

            }

            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else {
                animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 0);
                animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 0);
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetLookAtWeight(0);
            }
        }
    }
}
