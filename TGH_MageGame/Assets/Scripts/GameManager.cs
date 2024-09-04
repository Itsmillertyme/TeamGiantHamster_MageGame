using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    Camera cam;
    [SerializeField] Canvas hud;
    [SerializeField] RectTransform crosshair;

    private void Awake() {
        //Initialize
        cam = Camera.main;

        //Add listeners for camera button distance using delegates to pass in button index as parameter
        foreach (Button btn in hud.GetComponentsInChildren<Button>()) {
            int temp = int.Parse(btn.GetComponentInChildren<TextMeshProUGUI>().text);
            btn.onClick.AddListener(delegate { SetCameraDistance(temp); });
        }

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

    public void SetCameraDistance(int location) {
        Debug.Log("Setting camera distance to position " + location);
        switch (location) {
            case 1:
                //Most zoomed in
                cam.transform.localPosition = new Vector3(0, 2.5f, 5);
                break;
            case 2:
                cam.transform.localPosition = new Vector3(0, 2.5f, 6.5f);
                break;
            case 3:
                cam.transform.localPosition = new Vector3(0, 2.5f, 8);
                //Most zoomed out
                break;
            default:
                //Most zoomed in
                cam.transform.localPosition = new Vector3(0, 2.5f, 5);
                break;

        }
    }
}
