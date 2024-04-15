using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Movement : MonoBehaviour {

    public Transform TeleportIndicatorPrefab;
    public LineRenderer TeleportLineRenderer;
    public Transform LeftHandTransform;
    public float SnapTurnDegrees = 45;
    Transform cameraTransform;
    Transform currentTeleportIndicator;
    LineRenderer currentTeleportLineRenderer;

    bool calculateTeleportDestination(Vector3 originPos, Vector3 originLookVector, out Vector3 hitPoint, out List<Vector3> linePoints) {
        bool foundGround = false;
        int itterations = 20;
        float distancePerItterationMultiplier = .333f;
        float gravityPerItteration = .333f; // this is scaled with DistancePerItterationMultiplier so lowering said multiplier will still teleport you to the same location but with more checks. Gravity does not need to be adjusted.
        Vector3 currentPos = originPos;
        Vector3 currentLookVector = originLookVector;
        linePoints = new List<Vector3>() {originPos};
        for(int i = 0; i < itterations; i++) {
            Vector3 calculatedLookVector = currentLookVector * distancePerItterationMultiplier;
            bool didHit = Physics.Raycast(currentPos, calculatedLookVector, out RaycastHit hitInfo, calculatedLookVector.magnitude);
            if(didHit) {
                Debug.DrawLine(currentPos, hitInfo.point, Color.yellow);
                currentPos = hitInfo.point;
                foundGround = true;
            } else {
                Debug.DrawRay(currentPos, calculatedLookVector, Color.blue);
                currentPos += calculatedLookVector;
                currentLookVector += (Vector3.down * gravityPerItteration * distancePerItterationMultiplier);
                //currentLookVector.Normalize();
            }
            linePoints.Add(currentPos);

            if(didHit)
                break;
        }
        hitPoint = currentPos;
        return foundGround;
    }

    // Start is called before the first frame update
    void Start() {
        Camera cam = GetComponentInChildren<Camera>();
        if(cam)
            cameraTransform = cam.transform;
        else
            Debug.LogError("Unable to find camera within SteamVR camera rig.", gameObject);
    }

    // Update is called once per frame
    void Update() {
        bool snapLeft = SteamVR_Input.GetBooleanAction("SnapTurnLeft").GetStateDown(SteamVR_Input_Sources.RightHand);
        bool snapRight = SteamVR_Input.GetBooleanAction("SnapTurnRight").GetStateDown(SteamVR_Input_Sources.RightHand);

        if(snapLeft) {
            transform.RotateAround(cameraTransform.position, Vector3.up, -SnapTurnDegrees);
        } else if(snapRight) {
            transform.RotateAround(cameraTransform.position, Vector3.up, SnapTurnDegrees);
        }

        bool TeleportHeld = SteamVR_Input.GetBooleanAction("Teleport").GetState(SteamVR_Input_Sources.LeftHand);
        bool TeleportFinished = SteamVR_Input.GetBooleanAction("Teleport").GetStateUp(SteamVR_Input_Sources.LeftHand);
        if(TeleportHeld || TeleportFinished) {
            if(!currentTeleportIndicator) currentTeleportIndicator = Instantiate<Transform>(TeleportIndicatorPrefab) as Transform;
            if(!currentTeleportLineRenderer) currentTeleportLineRenderer = Instantiate<LineRenderer>(TeleportLineRenderer) as LineRenderer;

            bool teleportCast = calculateTeleportDestination(LeftHandTransform.position + (LeftHandTransform.forward * .1f), LeftHandTransform.forward, out Vector3 hitPoint, out List<Vector3> linePoints);
            if(teleportCast) {
                currentTeleportIndicator.gameObject.SetActive(true);
                currentTeleportIndicator.position = hitPoint;
            } else {
                currentTeleportIndicator.gameObject.SetActive(false);
            }

            currentTeleportLineRenderer.positionCount = linePoints.Count;
            for(int i = 0; i < linePoints.Count; i++) {
                currentTeleportLineRenderer.SetPosition(i, linePoints[i]);
            }

            if(TeleportFinished) {
                GameObject.Destroy(currentTeleportIndicator.gameObject);
                GameObject.Destroy(currentTeleportLineRenderer.gameObject);
                if (teleportCast)
                    transform.position = hitPoint - Vector3.ProjectOnPlane(cameraTransform.position - transform.position, Vector3.up);
            }
        }
    }
}
