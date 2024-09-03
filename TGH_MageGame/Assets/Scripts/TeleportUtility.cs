using UnityEngine;

public class TeleportUtility : MonoBehaviour {
    [SerializeField] private CharacterController thingToMove;
    // [SerializeField] private GameObject thingToMove; // IF NOT PLAYER USE THIS INSTEAD
    [SerializeField] private GameObject location1;
    [SerializeField] private GameObject location2;
    [SerializeField] private GameObject location3;
    [SerializeField] private GameObject location4;
    [SerializeField] private Canvas canvas;

    bool isActive = true;

    private void Update() {
        // 'I' TO TOGGLE UI CANVAS
        if (Input.GetKeyDown(KeyCode.I)) {
            isActive = !isActive;
            canvas.enabled = !isActive;
        }
    }

    public void MoveToLocation1() {
        thingToMove.enabled = false;
        thingToMove.transform.position = location1.transform.position;
        thingToMove.transform.rotation = location1.transform.rotation;
        thingToMove.enabled = true;
    }

    public void MoveToLocation2() {
        thingToMove.enabled = false;
        thingToMove.transform.position = location2.transform.position;
        thingToMove.transform.rotation = location2.transform.rotation;
        thingToMove.enabled = true;
    }

    public void MoveToLocation3() {
        thingToMove.enabled = false;
        thingToMove.transform.position = location3.transform.position;
        thingToMove.transform.rotation = location3.transform.rotation;
        thingToMove.enabled = true;
    }

    public void MoveToLocation4() {
        thingToMove.enabled = false;
        thingToMove.transform.position = location4.transform.position;
        thingToMove.transform.rotation = location4.transform.rotation;
        thingToMove.enabled = true;
    }
}