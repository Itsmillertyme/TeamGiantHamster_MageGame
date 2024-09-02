using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {

    ActionAsset actionAsset;
    CharacterController characterController;
    Animator animator;

    float currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    Vector3 currentCrouchMovement;
    bool isMovementPressed;
    bool isRunPressed;
    bool isCrouchPressed;
    bool isFacingBackwards;
    [SerializeField] GameObject playerModel;
    [SerializeField] float movementSpeed = 1.0f;
    [SerializeField] float sprintSpeed = 3.0f;
    [SerializeField] float jumpPower = 1.0f;
    [SerializeField][Range(0.1f, 9.8f)] float gravity;
    //
    Coroutine turnAnimation;

    //Animation Variables
    int isWalkingHash;
    int isRunningHash;
    int isCrouchingHash;
    int isCrouchWalkingHash;
    int jumpHash;
    int turnHash;

    //**Unity Methods    
    void Awake() {
        //Initialize
        actionAsset = new ActionAsset();
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        //
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isCrouchingHash = Animator.StringToHash("isCrouching");
        isCrouchWalkingHash = Animator.StringToHash("isCrouchWalking");
        jumpHash = Animator.StringToHash("jump");
        turnHash = Animator.StringToHash("Turn");

        //Define callbacks
        actionAsset.Player.Move.started += OnMovementInput;
        actionAsset.Player.Move.canceled += OnMovementInput;
        //
        actionAsset.Player.Run.started += OnRun;
        actionAsset.Player.Run.canceled += OnRun;
        //
        actionAsset.Player.Crouch.started += OnCrouch;
        actionAsset.Player.Crouch.canceled += OnCrouch;
        //
        actionAsset.Player.Jump.started += OnJump;


    }
    //
    void Update() {
        HandleGravity();
        //HandleRotation();
        HandleAnimation();

        //Hacky solution to keep gravity from becoming insane
        if (currentMovement.y < -gravity) {
            currentMovement.y = -gravity;
        }

        //do controller move with updated movement vector
        if (isRunPressed) {
            characterController.Move(currentRunMovement * Time.deltaTime);
        }
        else if (isCrouchPressed) {
            characterController.Move(currentCrouchMovement * Time.deltaTime);
        }
        else {
            characterController.Move(currentMovement * Time.deltaTime);
        }

        //Snap Z coord to 0
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
    //
    private void OnEnable() {
        //Turn on action assets
        actionAsset.Player.Enable();
    }
    //
    private void OnDisable() {
        //Turn off action assets
        actionAsset.Player.Disable();
    }

    //**Utility Methods
    //Wrapper for movement input callbacks
    void OnMovementInput(InputAction.CallbackContext context) {
        //Read values from Input System
        currentMovementInput = -1 * context.ReadValue<float>();

        //Turn animation and flip model
        if ((currentMovementInput > 0 && !isFacingBackwards) || (currentMovementInput < 0 && isFacingBackwards)) {
            if (turnAnimation == null) {
                turnAnimation = StartCoroutine(TurnModel(context));
            }
        }

        //Negate movement while turning
        if (turnAnimation != null) {
            currentMovementInput = 0;
        }

        //Setup movement vectors, part by part
        currentMovement.x = currentMovementInput * movementSpeed;
        currentRunMovement.x = currentMovementInput * sprintSpeed * movementSpeed;
        currentCrouchMovement.x = currentMovementInput * 0.5f * movementSpeed;

        //Set flag
        isMovementPressed = currentMovementInput != 0;
    }
    //
    //Wrapper for run input callbacks
    public void OnRun(InputAction.CallbackContext context) {
        isRunPressed = context.ReadValueAsButton();
    }
    //
    //Wrapper for crouch input callbacks
    public void OnCrouch(InputAction.CallbackContext context) {
        isCrouchPressed = context.ReadValueAsButton();
    }
    //   
    //Wrapper for jump input callbacks
    public void OnJump(InputAction.CallbackContext context) {
        animator.SetTrigger(jumpHash);

        if (characterController.isGrounded) {
            currentMovement.y += jumpPower;
            currentRunMovement.y += jumpPower;
            currentCrouchMovement.y += jumpPower;
        }
    }
    //
    void HandleAnimation() {
        //get param values from animator
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);
        bool isCrouching = animator.GetBool(isCrouchingHash);
        bool isCrouchWalking = animator.GetBool(isCrouchWalkingHash);

        //start crouch if crouch is pressed while not crouched
        if (isCrouchPressed && !isCrouching) {
            animator.SetBool(isCrouchingHash, true);
        }
        //Stop crouching if crouching not pressed while already crouching
        else if (!isCrouchPressed && isCrouching) {
            animator.SetBool(isCrouchingHash, false);
        }

        //Start crouch-walk if movement and crouch pressed while not crouch-walking
        if ((isMovementPressed && isCrouchPressed) && !isCrouchWalking) {
            animator.SetBool(isCrouchWalkingHash, true);
        }
        //Stop crouch-walk if no movement and crouch pressed while crouch-walking
        else if ((!isMovementPressed || !isCrouchPressed) && isCrouchWalking) {
            animator.SetBool(isCrouchWalkingHash, false);
            //animator.SetBool(isCrouchingHash, true);
        }
        //else if ((isMovementPressed && !isCrouchPressed) && isCrouchWalking) {
        //    animator.SetBool(isCrouchWalkingHash, false);
        //    animator.SetBool(isCrouchingHash, false);
        //}

        //Start walking if movement pressed while not walking
        if (isMovementPressed && !isWalking) {
            animator.SetBool(isWalkingHash, true);

        }
        //Stop walking is movement not pressed while already wlaking
        else if (!isMovementPressed && isWalking) {
            animator.SetBool(isWalkingHash, false);
        }

        //Start run if movement and run pressed while not currently runnning
        if ((isMovementPressed && isRunPressed) && !isRunning) {
            animator.SetBool(isRunningHash, true);
        }
        //Stop run if movement or run are not pressed while currently running
        else if ((!isMovementPressed || !isRunPressed) && isRunning) {
            animator.SetBool(isRunningHash, false);
        }

    }
    //
    void HandleGravity() {
        float grav = -gravity;
        if (characterController.isGrounded) {
            grav = -0.05f;
        }
        currentMovement.y += grav;
        currentCrouchMovement.y += grav;
        currentRunMovement.y += grav;

        //Clamp y movement
        Mathf.Clamp(currentMovement.y, -9.8f, -.01f);

    }

    //**Coroutines**
    IEnumerator TurnModel(InputAction.CallbackContext context) {

        //Crossfade into animation
        animator.CrossFade(turnHash, 0.01f);

        //wait for it to play
        yield return new WaitForSeconds(.25f);

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

        //Pass input callback context from original button press back into method
        OnMovementInput(context);
    }


}
