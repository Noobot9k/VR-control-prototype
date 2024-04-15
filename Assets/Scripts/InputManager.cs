using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class InputManager : MonoBehaviour {

    static InputManager Current;

    static public UnityEvent LeftGripPulled = new UnityEvent();
    static public UnityEvent LeftGripReleased = new UnityEvent();
    static public UnityEvent RightGripPulled = new UnityEvent();
    static public UnityEvent RightGripReleased = new UnityEvent();

    bool LeftGripState = false;
    bool rightGripState = false;

    private void OnEnable() {
        if(InputManager.Current)
            Debug.LogError("More than one instance of InputManager should not be running! Attached object: " + gameObject.name);
        else
            InputManager.Current = this;
    }
    private void OnDisable() {
        if(InputManager.Current == this)
            InputManager.Current = null;
    }

    void Start() {

    }
    void Update() {
        float leftGripAlpha = SteamVR_Input.GetSingleAction("default", "GripSqueeze").GetAxis(SteamVR_Input_Sources.LeftHand);
        float rightGripAlpha = SteamVR_Input.GetSingleAction("default", "GripSqueeze").GetAxis(SteamVR_Input_Sources.RightHand);
        float leftTriggerAlpha = SteamVR_Input.GetSingleAction("default", "TriggerSqueeze").GetAxis(SteamVR_Input_Sources.LeftHand);
        float rightTriggerAlpha = SteamVR_Input.GetSingleAction("default", "TriggerSqueeze").GetAxis(SteamVR_Input_Sources.RightHand);

        if(leftGripAlpha > .5f) {
            if(LeftGripState == false) {
                LeftGripState = true;
                if(LeftGripPulled != null) LeftGripPulled.Invoke();
            }
        } else {
            if(LeftGripState == true) {
                LeftGripState = false;
                if(LeftGripReleased != null) LeftGripReleased.Invoke();
            }
        }
        if(rightGripAlpha > .5f) {
            if(rightGripState == false) {
                rightGripState = true;
                if(RightGripPulled != null) RightGripPulled.Invoke();
            }
        } else {
            if(rightGripState == true) {
                rightGripState = false;
                if(RightGripReleased != null) RightGripReleased.Invoke();
            }
        }

    }
}
