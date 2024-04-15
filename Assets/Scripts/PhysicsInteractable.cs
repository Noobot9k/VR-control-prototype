using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsInteractable : MonoBehaviour {

    [Tooltip("If this value is true, the player can grab this object anywhere regardless of grip points. If this value is false, this item can only be held from a grip point.")]
    public bool AllowGenericGrabbing = true;

    public List<GripPointData> GripPoints = new List<GripPointData>();

    
    public void OnPickedUp(GripPointData GripPoint = null) {

    }
    public void OnReleased(GripPointData GripPoint = null) {

    }

    private void OnDrawGizmosSelected() {
        foreach(GripPointData gripPoint in GripPoints) {
            if (gripPoint.GripType == GripPointData.GripTypeENUM.Line) {
                Debug.DrawLine(transform.TransformPoint(gripPoint.LinePoint0), transform.TransformPoint(gripPoint.LinePoint1), Color.red, 0, false);
            } else if (gripPoint.GripType == GripPointData.GripTypeENUM.Point) {
                Vector3 pointOriginWorld = transform.TransformPoint(gripPoint.PointOrigin);
                float debugSize = .01f;
                Debug.DrawRay(pointOriginWorld + gripPoint.PointRotation * transform.up      * debugSize, gripPoint.PointRotation * transform.up.normalized       * -debugSize * 2, Color.red, 0, false);
                Debug.DrawRay(pointOriginWorld + gripPoint.PointRotation * transform.right   * debugSize, gripPoint.PointRotation * transform.right.normalized    * -debugSize * 2, Color.red, 0, false);
                Debug.DrawRay(pointOriginWorld + gripPoint.PointRotation * transform.forward * debugSize, gripPoint.PointRotation * transform.forward.normalized  * -debugSize * 2, Color.red, 0, false);

            }
        }
    }
}

[System.Serializable]
public class GripPointData {
    public enum GripTypeENUM { Line, Disc, Point }
    public GripTypeENUM GripType;
    [Tooltip("If this value is true, only one hand can hold this grip.")]
    public bool GripHolderLimit = false;
    public float GripRadius = .1f;

    [Header("GripType Line:")]
    public Vector3 LinePoint0;
    public Vector3 LinePoint1;
    [Header("GripType Disc:")]
    public Vector3 DiscOrigin;
    public Quaternion DiscRotation;
    public float DiscRadius;
    [Header("GripType Point:")]
    public Vector3 PointOrigin;
    public Quaternion PointRotation;

    public GripPointData() {
        GripType = GripTypeENUM.Line;
        GripRadius = .1f;
    }
    public GripPointData(GripTypeENUM type) {
        GripType = type;
        GripRadius = .1f;
    }
    public GripPointData(float lineRadius, Vector3 linePoint0, Vector3 linePoint1) {
        GripType = GripTypeENUM.Line;
        GripRadius = lineRadius;
        LinePoint0 = linePoint0;
        LinePoint1 = linePoint1;
    }
    public GripPointData(float gripRadius, float discRadius, Vector3 discOriginPosition, Quaternion discRotation) {
        GripType = GripTypeENUM.Disc;
        GripRadius = gripRadius;
        DiscRadius = discRadius;
        DiscOrigin = discOriginPosition;
        DiscRotation = discRotation;
    }
    public GripPointData(float gripRadius, Vector3 pointOrigin, Quaternion pointRotation) {
        GripType = GripTypeENUM.Point;
        GripRadius = gripRadius;
        PointOrigin = pointOrigin;
        PointRotation = pointRotation;
    }

}