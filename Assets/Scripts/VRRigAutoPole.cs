using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRRigAutoPole : MonoBehaviour {

    enum HandSide { Left, Right }

    public Vector3 EstimatedTorsoSize = new Vector3(.333f, 1.8f, .25f);
    public float PoleDistance = .5f;
    public float ShoulderOffsetFromHead = -.1f;
    public Vector3 SectorTrasnferRanges = new Vector3(.2f, .15f, .2f);
    public Vector3 DebugOffset = new Vector3(0,1.8f/2,0);

    Vector3 PoleLeftPos = new Vector3();
    Vector3 PoleRightPos = new Vector3();

    public Transform LeftPole;
    public Transform RightPole;
    public Transform LeftHand;
    public Transform RightHand;
    public Transform Head;

    void Start() {

    }
    void Update() {
        float ShoulderHight = transform.InverseTransformPoint(Head.position).y + ShoulderOffsetFromHead;

        Vector3 FrontTopLeft = new Vector3(-EstimatedTorsoSize.x / 2, ShoulderHight, EstimatedTorsoSize.z / 2);
        Vector3 FrontTopRight = new Vector3(EstimatedTorsoSize.x / 2, ShoulderHight, EstimatedTorsoSize.z / 2);
        Vector3 BackTopLeft = new Vector3(-EstimatedTorsoSize.x / 2, ShoulderHight, -EstimatedTorsoSize.z / 2);
        Vector3 BackTopRight = new Vector3(EstimatedTorsoSize.x / 2, ShoulderHight, -EstimatedTorsoSize.z / 2);
        
        Debug.DrawRay(transform.TransformPoint(FrontTopLeft), -transform.up * EstimatedTorsoSize.y, Color.green);
        Debug.DrawRay(transform.TransformPoint(FrontTopRight), -transform.up * EstimatedTorsoSize.y, Color.green);
        Debug.DrawRay(transform.TransformPoint(BackTopLeft), -transform.up * EstimatedTorsoSize.y, Color.green);
        Debug.DrawRay(transform.TransformPoint(BackTopRight), -transform.up * EstimatedTorsoSize.y, Color.green);

        //Vector3 leftHandLocalPos = transform.InverseTransformPoint(LeftHand.position);
        //Vector3 RightHandLocalPos = transform.InverseTransformPoint(RightHand.position);

        //float LeftHand_LeftToRightDistanceFromShoulder = Vector3.Project(leftHandLocalPos - FrontTopLeft, transform.right).x;
        //float LeftHand_FrontToBackDistanceFromShoulder = Vector3.Project(leftHandLocalPos - BackTopLeft, transform.forward).z;

        //float LeftHand_RightSectionAlpha = Mathf.Clamp01((leftHandLocalPos.x - FrontTopLeft.x) / SectorTrasnferRanges.x);
        //float LeftHand_UpperSectorAlpha = Mathf.Clamp01((leftHandLocalPos.y - ShoulderHight) / SectorTrasnferRanges.y);
        //float LeftHand_RearSectionAlpha = Mathf.Clamp01((leftHandLocalPos.z - BackTopLeft.z) / SectorTrasnferRanges.z);
        //float LeftHand_FrontRightSectionAlpha = 1 - LeftHand_RightSectionAlpha * LeftHand_RearSectionAlpha;

        //PoleLeftPos = new Vector3(
        //    LeftHand_LeftToRightDistanceFromShoulder / 2 - EstimatedTorsoSize.x / 2 - (PoleDistance * LeftHand_RearSectionAlpha) + (PoleDistance * (1 - LeftHand_RearSectionAlpha)),
        //    Mathf.Clamp(leftHandLocalPos.y, -10, ShoulderHight) - PoleDistance,
        //    LeftHand_FrontToBackDistanceFromShoulder / 2 - (PoleDistance * LeftHand_FrontRightSectionAlpha) //(PoleDistance * (1-LeftHand_UpperSectorAlpha))
        //);

        //LeftPole.position = transform.TransformPoint(PoleLeftPos);


        Vector3 GetPolePosition(HandSide handSide) {
            Transform Hand = handSide == HandSide.Left ? LeftHand : RightHand;

            Vector3 HandLocalPos = transform.InverseTransformPoint(Hand.position);

            float Hand_SideToSideDistanceFromShoulder = Vector3.Project(HandLocalPos - FrontTopLeft, transform.right).x;
            //if (handSide == HandSide.Right)
            //    Hand_SideToSideDistanceFromShoulder = Vector3.Project(FrontTopRight - HandLocalPos, transform.right).x;
            float Hand_FrontToBackDistanceFromShoulder = Vector3.Project(HandLocalPos - (handSide == HandSide.Left ? BackTopLeft : BackTopRight), transform.forward).z;

            float Hand_OppositeSideSectionAlpha = Mathf.Clamp01(((HandLocalPos.x - (handSide == HandSide.Left ? FrontTopLeft.x : FrontTopRight.x)) * (handSide == HandSide.Left ? 1 : -1)) / SectorTrasnferRanges.x);
            float Hand_UpperSectionAlpha = Mathf.Clamp01((HandLocalPos.y - ShoulderHight) / SectorTrasnferRanges.y);
            float Hand_RearSectionAlpha = Mathf.Clamp01((HandLocalPos.z - BackTopLeft.z) / SectorTrasnferRanges.z);
            float Hand_FrontOppositeSideSectionAlpha = 1 - Hand_OppositeSideSectionAlpha * Hand_RearSectionAlpha;
            float Hand_UpperRearSectionAlpha = (1-Hand_RearSectionAlpha) * Hand_UpperSectionAlpha;

            float PolePosX = 0;
            if(handSide == HandSide.Left)
                PolePosX = Hand_SideToSideDistanceFromShoulder / 2 - EstimatedTorsoSize.x / 2 - (PoleDistance * Hand_RearSectionAlpha) - (PoleDistance * Hand_UpperRearSectionAlpha);// + (PoleDistance * (1 - Hand_RearSectionAlpha));
            else
                PolePosX = Hand_SideToSideDistanceFromShoulder / 2 + EstimatedTorsoSize.x / 2 + (PoleDistance * Hand_RearSectionAlpha) + (PoleDistance * Hand_UpperRearSectionAlpha);// - (PoleDistance * (1 - Hand_RearSectionAlpha));
            Vector3 PolePosition = new Vector3(
                PolePosX,
                Mathf.Clamp(HandLocalPos.y, -10, ShoulderHight) - (PoleDistance * (Hand_RearSectionAlpha)),
                Hand_FrontToBackDistanceFromShoulder / 2 - (PoleDistance * Hand_FrontOppositeSideSectionAlpha * (1- Hand_UpperRearSectionAlpha)) + (PoleDistance * Hand_UpperRearSectionAlpha)
            );
            return PolePosition;

            //if (handSide == HandSide.Left)
            //    return new Vector3(
            //        Hand_SideToSideDistanceFromShoulder / 2 - EstimatedTorsoSize.x / 2 - (PoleDistance * Hand_RearSectionAlpha) + (PoleDistance * (1 - Hand_RearSectionAlpha)),
            //        Mathf.Clamp(HandLocalPos.y, -10, ShoulderHight) - PoleDistance,
            //        Hand_FrontToBackDistanceFromShoulder / 2 - (PoleDistance * Hand_FrontOppositeSideSectionAlpha)
            //    );
            //else {
            //    return new Vector3(
            //        Hand_SideToSideDistanceFromShoulder / 2 + EstimatedTorsoSize.x / 2 + (PoleDistance * Hand_RearSectionAlpha) - (PoleDistance * (1 - Hand_RearSectionAlpha)),
            //        Mathf.Clamp(HandLocalPos.y, -10, ShoulderHight) - PoleDistance,
            //        Hand_FrontToBackDistanceFromShoulder / 2 - (PoleDistance * Hand_FrontOppositeSideSectionAlpha)
            //    );
            //}
        }

        LeftPole.position = transform.TransformPoint(GetPolePosition(HandSide.Left));
        RightPole.position = transform.TransformPoint(GetPolePosition(HandSide.Right));

    }
}
