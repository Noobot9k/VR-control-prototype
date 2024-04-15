using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

class ActivePhysicsAnimationPlugin : EditorWindow {

    static public float TotalMass = 1.267658f;

    [MenuItem("Tools/ActivePhysics/Setup Rig", true), MenuItem("Tools/ActivePhysics/Select Configurable Joints", true)]
    static bool ValidateConvert() {
        return Selection.activeGameObject != null;
    }

    [MenuItem("Tools/ActivePhysics/Select Configurable Joints")]
    static void SelectConfigurableJoints() {

        GameObject selected = Selection.activeGameObject;
        if(selected == null) return;

        List<Object> foundObjects = new List<Object>();

        checkChild(selected);

        void checkChild(GameObject obj) {

            if(obj.GetComponent<ConfigurableJoint>()) {
                foundObjects.Add(obj);
            }

            foreach(Transform child in obj.transform) {
                checkChild(child.gameObject);
            }
        }
        Selection.objects = foundObjects.ToArray();
    }

    [MenuItem("Tools/ActivePhysics/Setup Rig")]
    static void Convert() {
        // retrieve and validate selection
        GameObject selected = Selection.activeGameObject;
        if(selected == null) return;

        List<Object> modifiedObjects = new List<Object>();

        void checkChild(GameObject obj) {
            // get first child
            Transform firstChild = null;
            if (obj.transform.childCount > 0)
                firstChild = obj.transform.GetChild(0);

            // check if this object and its children are valid to modify
            if(firstChild == null) return;
            if(obj.active == false) return;

            // check for existing capsule
            Collider collider = obj.GetComponent<Collider>();
            if(collider == null) {
                // apply

                // setup so undo will work
                //Undo.RecordObject(obj, "Create active physics rig");

                // create collider
                CapsuleCollider newCollider = Undo.AddComponent<CapsuleCollider>(obj);

                // set default values for if there is no first child
                Vector3 averagePosition = new Vector3(0, .05f, 0);
                float distance = .1f;
                int colliderDirection = 1;

                // set values for if there is a first child
                if(firstChild) {
                    averagePosition = firstChild.localPosition / 2;
                    distance = firstChild.localPosition.magnitude;
                    Vector3 firstChildDirection = firstChild.localPosition.normalized;
                    if(Mathf.Max(firstChildDirection.x, firstChildDirection.y, firstChildDirection.z) == firstChildDirection.x) colliderDirection = 0;
                    if(Mathf.Max(firstChildDirection.x, firstChildDirection.y, firstChildDirection.z) == firstChildDirection.y) colliderDirection = 1;
                    if(Mathf.Max(firstChildDirection.x, firstChildDirection.y, firstChildDirection.z) == firstChildDirection.z) colliderDirection = 2;
                }

                // apply values to collider
                newCollider.center = averagePosition;
                newCollider.direction = colliderDirection;
                newCollider.height = distance;
                newCollider.radius = .012f;

                // close undo setup
                //PrefabUtility.RecordPrefabInstancePropertyModifications(obj);

                // add to modified objects list
                modifiedObjects.Add(obj);
            }

            // check for rigidbody
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb == null) {

                // create Rigidbody
                Rigidbody newRb = Undo.AddComponent<Rigidbody>(obj);
                newRb.useGravity = false;

                if(modifiedObjects.Contains(obj) == false) modifiedObjects.Add(obj);
            }

            // check for configurable joint
            ConfigurableJoint joint = obj.GetComponent<ConfigurableJoint>();
            if (true){//(joint == null) {

                Rigidbody parentRB = null;
                if (obj.transform.parent) parentRB = obj.transform.parent.GetComponent<Rigidbody>();
                if(parentRB) {

                    // create new joint
                    ConfigurableJoint newJoint = joint;
                    if (!newJoint) newJoint = Undo.AddComponent<ConfigurableJoint>(obj);
                    Undo.RecordObject(newJoint, "update configurable joint");
                    newJoint.connectedBody = parentRB;
                    newJoint.secondaryAxis = Vector3.down;
                    newJoint.xMotion = ConfigurableJointMotion.Locked;
                    newJoint.yMotion = ConfigurableJointMotion.Locked;
                    newJoint.zMotion = ConfigurableJointMotion.Locked;
                    newJoint.angularXMotion = ConfigurableJointMotion.Limited;
                    //newJoint.angularYMotion = ConfigurableJointMotion.Locked;
                    //newJoint.angularZMotion = ConfigurableJointMotion.Locked;

                    SoftJointLimit lowX = new SoftJointLimit();
                    lowX.limit = -5;
                    newJoint.lowAngularXLimit = lowX;
                    SoftJointLimit highX = new SoftJointLimit();
                    highX.limit = 95;
                    newJoint.highAngularXLimit = highX;

                    SoftJointLimitSpring xSpring = new SoftJointLimitSpring();
                    xSpring.spring = 150;
                    xSpring.damper = 5;
                    newJoint.angularXLimitSpring = xSpring;
                    newJoint.angularYZLimitSpring = xSpring;
                    newJoint.rotationDriveMode = RotationDriveMode.Slerp;
                    JointDrive slerpDrive = new JointDrive();
                    slerpDrive.positionSpring = 125;
                    slerpDrive.positionDamper = 3;
                    newJoint.slerpDrive = slerpDrive;

                    if(modifiedObjects.Contains(obj) == false) modifiedObjects.Add(obj);

                }
            }

            // apply to children
            foreach(Transform child in obj.transform) {
                checkChild(child.gameObject);
            }
        }

        checkChild(selected);

        foreach(Object modifiedObj in modifiedObjects) {
            GameObject obj = (GameObject)modifiedObj;
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            rb.mass = TotalMass / modifiedObjects.Count;
        }

        Selection.objects = modifiedObjects.ToArray();

    }


    void Update() {

    }
}
