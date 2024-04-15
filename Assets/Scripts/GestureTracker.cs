using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureTracker : MonoBehaviour {

    public enum GestureHand { left, right, both, any};
    public enum GestureSpace { World, Head, LocalHand};
    public enum Hand { Left, Right};
    public Transform leftHand;
    public Transform rightHand;
    public Transform head;
    public float GestureVectorDecay = 5;
    public float GestureCooldown = .5f;
    public List<Gesture> Gestures = new List<Gesture>();

    Vector3 leftGestureOrigin;
    Vector3 rightGestureOrigin;
    Vector3 LeftGestureLook;
    Vector3 RightGestureLook;
    Vector3 LeftGestureVector;
    Vector3 RightGestureVector;

    void Start() {
        leftGestureOrigin = leftHand.position;
        rightGestureOrigin = rightHand.position;
    }

    Vector3 GetLocalGestureVector(GestureSpace gestureSpace, Hand hand, Vector3 gestureVector) {
        Vector3 localGestureVector = gestureVector;
        if(gestureSpace == GestureSpace.Head) {
            localGestureVector = head.InverseTransformDirection(gestureVector);
        } else if(gestureSpace == GestureSpace.LocalHand) {
            if(hand == Hand.Right) {
                localGestureVector = rightHand.InverseTransformDirection(gestureVector);
                localGestureVector.Scale(new Vector3(-1, 1, 1));
            } else {
                localGestureVector = leftHand.InverseTransformDirection(gestureVector);
            }
        }
        return localGestureVector;
    }
    // for internal use only.
    void checkGestureAgainstHand(Gesture gesture, Vector3 gestureVector, Vector3 gestureLook, Vector3 currentPosition, Hand hand) {
        Vector3 localGestureVector = GetLocalGestureVector(gesture.MoveRelitiveTo, hand, gestureVector);
        Vector3 localGestureLook = GetLocalGestureVector(gesture.MoveRelitiveTo, hand, gestureLook);
        
        if(localGestureVector.magnitude > gesture.MoveVector.magnitude
                && Vector3.Dot(localGestureVector.normalized, gesture.MoveVector.normalized) > gesture.travelStrictness
                && Vector3.Dot(localGestureLook, gesture.LookVector) > gesture.facingDirectionStrictness
                && Time.time - gesture.tickLastUsed > GestureCooldown) {
            // Gesture was performed.
            PerformedGestureDetails details = new PerformedGestureDetails(gesture, gestureVector, gestureLook, currentPosition, hand);
            //print("Gesture performed: " + details.name);
            gesture.tickLastUsed = Time.time;
            SendMessage("GesturePerformed", details, SendMessageOptions.DontRequireReceiver);
        }
    }

    void Update() {
        leftGestureOrigin = Vector3.Lerp(leftGestureOrigin, leftHand.position, GestureVectorDecay * Time.smoothDeltaTime);
        rightGestureOrigin = Vector3.Lerp(rightGestureOrigin, rightHand.position, GestureVectorDecay * Time.smoothDeltaTime);
        LeftGestureVector = leftHand.position - leftGestureOrigin;
        RightGestureVector = rightHand.position - rightGestureOrigin;
        Debug.DrawLine(leftGestureOrigin, leftHand.position, Color.green);
        Debug.DrawLine(rightGestureOrigin, rightHand.position, Color.green);

        LeftGestureLook = Vector3.Lerp(LeftGestureLook, leftHand.right, GestureVectorDecay * Time.smoothDeltaTime);
        RightGestureLook = Vector3.Lerp(RightGestureLook, -rightHand.right, GestureVectorDecay * Time.smoothDeltaTime);
        Debug.DrawRay(leftHand.position, LeftGestureLook * .1f, Color.blue);
        Debug.DrawRay(rightHand.position, RightGestureLook * .1f, Color.blue);

        foreach(Gesture gesture in Gestures) {
            if (gesture.gestureHand != GestureHand.right) checkGestureAgainstHand(gesture, LeftGestureVector, LeftGestureLook, leftHand.position, Hand.Left);
            if (gesture.gestureHand != GestureHand.left) checkGestureAgainstHand(gesture, RightGestureVector, RightGestureLook, rightHand.position, Hand.Right);
        }
    }
}

public class PerformedGestureDetails {
    public string name;
    public Gesture gesture;
    public Vector3 LookedVector;
    public Vector3 MovedVector;
    public float MovedVectorMagnitude;
    public Vector3 CompletionLocation;
    public GestureTracker.Hand hand;

    public PerformedGestureDetails(Gesture gest, Vector3 movedVector, Vector3 lookedVector, Vector3 completionLocation, GestureTracker.Hand _hand) {
        gesture = gest;
        name = gest.name;
        MovedVector = movedVector;
        MovedVectorMagnitude = MovedVector.magnitude;
        LookedVector = lookedVector;
        CompletionLocation = completionLocation;
        hand = _hand;
    }
}

[System.Serializable]
public class Gesture {
    public string name;
    [Tooltip("Which had must be used to perform gesture")]
    public GestureTracker.GestureHand gestureHand = GestureTracker.GestureHand.any;
    [Tooltip("How far and in what direction the controller must travel to activate gesture")]
    public Vector3 MoveVector = Vector3.up * .25f;
    [Tooltip("How precise the controller must be in traveling MoveVector (DOT product of MoveVector and the controller's traveled vector)")]
    public float travelStrictness = .666f;
    [Tooltip("What space the player's hand must be traveling in. if MoveVector = 0,1,0 and MoveRelitiveTo = world then the hand must move up from the ground but if MoveRelitiveTo = LocalHand then the hand must move along its local Y axis meaning this movement is dependent on the direction the hand is facing.")]
    public GestureTracker.GestureSpace MoveRelitiveTo = GestureTracker.GestureSpace.World;
    [Tooltip("What direction the player's PALM must be facing while traveling to activate gesture")]
    public Vector3 LookVector = Vector3.down;
    [Tooltip("How precise the controller must be in facing LookVector (DOT product of LookVector and the controller's left or right vector)")]
    public float facingDirectionStrictness = .666f;
    [Tooltip("Doesn't really work with LocalHand. LocalHand can be used to check if the hand rotated a lot of stayed facing mostly the same direction. See MoveRelitiveTo's tooltip for more info.")]
    public GestureTracker.GestureSpace LookRelitiveTo = GestureTracker.GestureSpace.World;

    [HideInInspector]
    public float tickLastUsed = -100;

    public Gesture() {
        MoveVector = Vector3.up * .25f;
        LookVector = Vector3.down;
    }
    public Gesture(Vector3 moveVector, Vector3 lookVector) {
        MoveVector = moveVector;
        LookVector = lookVector;
    }
}
