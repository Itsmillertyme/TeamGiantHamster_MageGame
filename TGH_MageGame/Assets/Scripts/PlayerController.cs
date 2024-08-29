using UnityEngine;
using UnityEngine.InputSystem;


/*
 * Author: Jacob M.
 * Date Created:8/27/2024
 * Version: V1
 */
public class PlayerController : MonoBehaviour {

    //Component references
    PlayerInput playerInput;
    CharacterController characterController;
    InputAction moveAction;
    Animator animator;
    [SerializeField] GameObject playerModel; //SII

    //Animations variables
    int velocityHash;
    //
    //Vector2 animationBlendVector; //Current animation blend vector
    //Vector2 animationVelocity;
    //float animationSmoothTime = 0.1f;

    //Player Properties
    float velocity;
    [SerializeField] float speed; //SII
    [SerializeField] float acceleration; //SII
    [SerializeField] float deceleration; //SII

    // Start is called before the first frame update
    void Awake() {

        //Cache References
        playerInput = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        //Cache animator strings as hashes
        velocityHash = Animator.StringToHash("Velocity");

        //Initialize actions
        moveAction = playerInput.actions["Move"];
    }

    // Update is called once per frame
    void FixedUpdate() {
        Move();

        //do gravity

    }

    void Move() {
        //Lock Z position top 0
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        //Read values from inputController for movement
        float movementInput = moveAction.ReadValue<float>();
        float animationInput = -movementInput;

        //Process animation based on inputs
        if (animationInput > 0.05 && velocity < 1) {
            velocity += acceleration * animationInput * Time.deltaTime;
        }
        if (animationInput < 0.05 && velocity > 0) {
            velocity -= deceleration * Time.deltaTime;
        }
        if (animationInput < 0.05 && velocity < 0) {
            velocity = 0;
        }

        //set animator parameters
        animator.SetFloat(velocityHash, velocity);
    }
}
