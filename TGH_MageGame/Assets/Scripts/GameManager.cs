using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager_EMPTY : MonoBehaviour {

    [SerializeField] Canvas hud;
    [SerializeField] RectTransform crosshair;

    private void Awake() {
        //Hide mouse
        Cursor.visible = false;
    }

    private void Update() {

        //set crosshair position
        crosshair.anchoredPosition = Mouse.current.position.ReadValue();
    }

    private void OnApplicationFocus(bool focus) {
        Cursor.visible = false;
    }

    //public void SetCameraDistance(int location) {
    //    Debug.Log("Setting camera distance to position " + location);
    //    switch (location) {
    //        case 1:
    //            //Most zoomed in
    //            cam.transform.localPosition = new Vector3(0, 2.5f, 5);
    //            break;
    //        case 2:
    //            cam.transform.localPosition = new Vector3(0, 2.5f, 6.5f);
    //            break;
    //        case 3:
    //            cam.transform.localPosition = new Vector3(0, 2.5f, 8);
    //            //Most zoomed out
    //            break;
    //        default:
    //            //Most zoomed in
    //            cam.transform.localPosition = new Vector3(0, 2.5f, 5);
    //            break;

    //    }
}

