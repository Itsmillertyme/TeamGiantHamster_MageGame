using UnityEngine;

public class TeleportUtility : MonoBehaviour {
    [SerializeField] private GameObject thingToMove;
    [SerializeField] private GameObject location1;
    [SerializeField] private GameObject location2;
    [SerializeField] private GameObject location3;
    [SerializeField] private GameObject location4;
    [SerializeField] private Canvas canvas;

    bool isActive = true;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.I)) {
            isActive = !isActive;
            canvas.enabled = isActive;
        }
    }

    public void MoveToLocation1() {
        thingToMove.transform.position = location1.transform.position;
        thingToMove.transform.rotation = location1.transform.rotation;
        Debug.Log("Moving to location 1");
    }

    public void MoveToLocation2() {
        thingToMove.transform.position = location2.transform.position;
        thingToMove.transform.rotation = location2.transform.rotation;
        Debug.Log("Moving to location 2");
    }

    public void MoveToLocation3() {
        thingToMove.transform.position = location3.transform.position;
        thingToMove.transform.rotation = location3.transform.rotation;
        Debug.Log("Moving to location 3");
    }

    public void MoveToLocation4() {
        thingToMove.transform.position = location4.transform.position;
        thingToMove.transform.rotation = location4.transform.rotation;
        Debug.Log("Moving to location 4");
    }
}
