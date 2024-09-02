using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


/*
 * Author: Jacob M.
 * Date Created:8/27/2024
 * Version: V2
 */
public class PlayerController : MonoBehaviour {

    //Component references
    PlayerInput playerInput;
    CharacterController characterController;
    Animator animator;
    [SerializeField] GameObject playerModel; //SII

    //Input actions
    InputAction moveAction;
    InputAction jumpAction;

    //Animations variables
    int velocityHash;
    int turnHash;

    //Player Properties
    float velocity;
    bool isFacingBackwards = false;
    bool isGrounded;
    [SerializeField] float speed;
    [SerializeField] float acceleration;
    [SerializeField] float deceleration;
    [SerializeField] float jumpPower;

    //Miscellaneous
    float gravity = -9.81f;
    [SerializeField] float gravityMultiplier;

    //Coroutines
    Coroutine turnAnimation;


    // Start is called before the first frame update
    void Awake() {

        //Cache References
        playerInput = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        //Cache animator strings as hashes
        velocityHash = Animator.StringToHash("Velocity");
        turnHash = Animator.StringToHash("Walking Turn 180");

        //Initialize actions
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
    }

    // Update is called once per frame
    void FixedUpdate() {
        GroundCheck();
        //ApplyGravity();
        Move();
        Debug.Log(characterController.isGrounded);
    }

    //WIP
    //void ApplyGravity() {

    //    characterController.Move(new Vector3(0, -0.1f, 0));
    //}

    public void Move() {
        //Lock Z position to 0
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        //Read values from inputController for movement
        float movementInput = moveAction.ReadValue<float>();

        //Turn animation and flip model
        if ((movementInput > 0 && !isFacingBackwards) || (movementInput < 0 && isFacingBackwards)) {
            if (turnAnimation == null) {
                turnAnimation = StartCoroutine(TurnModel());
            }
        }

        //Negate movement while turning
        if (turnAnimation != null) {
            movementInput = 0;
        }

        //get magnitude of input
        float animationInput = System.Math.Abs(movementInput);

        //Process animation based on input
        if (animationInput > 0.05 && velocity < 1) {
            //Acceleration
            velocity += acceleration * animationInput * Time.deltaTime;
        }
        if (animationInput < 0.05 && velocity > 0) {
            //Deceleration
            velocity -= deceleration * Time.deltaTime;
        }
        if (animationInput < 0.05 && velocity < 0) {
            //Set to 0 (negate any fluttering of values)
            velocity = 0;
        }

        //set animator parameters
        animator.SetFloat(velocityHash, velocity);


        ////Implement Gravity WIP
        //if (!isGrounded) {
        //    characterController.Move(new Vector3(0, -9.81f, 0) * Time.deltaTime);
        //}

    }

    public void Jump(InputAction.CallbackContext context) {

        if (!context.started) return;

        if (!isGrounded) return;
        //characterController.Move(new Vector3(0, jumpPower, 0));


    }

    void GroundCheck() {
        isGrounded = Physics.Raycast(transform.position, -Vector3.up, 0.01f);
    }

    IEnumerator TurnModel() {

        //Crossfade into animation
        animator.CrossFade(turnHash, 0.1f);

        //wait for it to play
        yield return new WaitForSeconds(0.5f);

        //set new rotation
        Quaternion newRotation;
        if (isFacingBackwards) {
            isFacingBackwards = false;
            newRotation = Quaternion.Euler(0, -90, 0);
        }
        else {
            isFacingBackwards = true;
            newRotation = Quaternion.Euler(0, 90, 0);
        }
        playerModel.transform.localRotation = newRotation;

        //Clear coroutine object
        turnAnimation = null;
    }

}
