using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour {

    [SerializeField] Canvas hud;
    [SerializeField] RectTransform crosshair;
    [SerializeField] Transform projectileSpawn;
    [SerializeField] Transform player;

    //DEV ONLY - REMOVE BEFORE BUILD
    Transform debugObject;

    private void Awake() {
        //Hide mouse
        Cursor.visible = false;

        //DEV ONLY - REMOVE BEFORE BUILD - setup debug object
        debugObject = new GameObject().transform;
        debugObject.name = "DEBUG - OBJECT";
    }

    private void Update() {
        MoveProjectileSpawn();
    }

    private void OnApplicationFocus(bool focus) {
        Cursor.visible = false;
    }

    void MoveProjectileSpawn() {

        //get mouse input position
        Vector3 mousePos = Mouse.current.position.ReadValue();

        //convert mouse input to point in world 
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Mathf.Abs(Camera.main.transform.position.z)));

        //get position in center of player model
        Vector3 centerMass = new Vector3(player.position.x, player.position.y + 1.162f, 0);

        //setup ray 
        Ray ray = new Ray(centerMass, new Vector3(worldPos.x, worldPos.y, 0) - centerMass);

        //spell spawn point offset from centermass of player        
        float offset = 1.25f;//DEFAULT IS .783f ONCE SPELL COLLISION DONE

        //move projectile spawn point
        projectileSpawn.transform.position = ray.GetPoint(offset);

        //DEV ONLY - REMOVE BEFORE BUILD - set debug object to world pos of mouse
        //debugObject.position = new Vector3(worldPos.x, worldPos.y, 0);

        //DEV ONLY - REMOVE BEFORE BUILD - draw ray
        //Debug.DrawRay(centerMass, debugObject.position - centerMass, Color.red);
    }

}

