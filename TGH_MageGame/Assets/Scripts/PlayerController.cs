using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {

    //base  y = .91, h = 1.8
    //crouch y = ..71, h = 1.4

    ActionAsset actionAsset;
    CharacterController characterController;
    [SerializeField] MousePositionTracking mousePositionTracker;
    Animator animator;
    [SerializeField] GameObject playerModel;

    float currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    Vector3 currentCrouchMovement;
    bool isMovementPressed;
    bool isRunPressed;
    bool isCrouchPressed;
    bool isFacingBackwards;
    bool isJumpPressed = false;

    //Jump Varbiables
    [SerializeField] float maxJumpHeight; //SII
    [SerializeField] float maxJumpTime; //SII
    float initJumpVelocity;
    bool isJumping = false;

    //Movement Varbiables
    [SerializeField] float movementSpeed = 1.0f;
    [SerializeField] float sprintSpeed = 3.0f;

    //Gravity Variables
    [SerializeField][Range(-0.1f, -20f)] float gravity = -9.8f; //SII
    float groundedGravity = -0.05f;


    //Animation Variables
    int isWalkingHash;
    int isRunningHash;
    int isCrouchingHash;
    int isCrouchWalkingHash;
    int landedHash;
    int jumpHash;
    int turnHash;
    //
    Coroutine turnAnimation;
    Coroutine jumpAnimation;

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
        landedHash = Animator.StringToHash("landed");
        jumpHash = Animator.StringToHash("Jump");
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
        actionAsset.Player.Jump.canceled += OnJump;
        //
        actionAsset.Player.MoveCamera.performed += Camera.main.GetComponent<CameraController>().CycleCameraPosition;
        //
        actionAsset.Player.DEVBREAK.performed += Devbreak;


        SetupJumpVariables();
    }
    //
    void Update() {

        HandleAnimation();
        mousePositionTracker.GetMousePosition();

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

        HandleGravity();
        HandleJump();

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
    public void OnMovementInput(InputAction.CallbackContext context) {
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
        isJumpPressed = context.ReadValueAsButton();
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
            //set player collider smaller
            characterController.height = 1.4f;
            characterController.center = new Vector3(-.06f, .65f, 0);
        }
        //Stop crouching if crouching not pressed while already crouching
        else if (!isCrouchPressed && isCrouching) {
            animator.SetBool(isCrouchingHash, false);
            //reset player collider
            characterController.height = 1.8f;
            characterController.center = new Vector3(0, .91f, 0);
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
        //Sets gravity every frame
        if (characterController.isGrounded) {
            currentMovement.y = groundedGravity;
            currentCrouchMovement.y = groundedGravity;
            currentRunMovement.y = groundedGravity;
        }
        //applies gravity every frame
        else {
            currentMovement.y += gravity * Time.deltaTime;
            currentCrouchMovement.y += gravity * Time.deltaTime;
            currentRunMovement.y += gravity * Time.deltaTime;
        }
    }
    //
    void SetupJumpVariables() {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }
    //
    void HandleJump() {
        if (!isJumping && characterController.isGrounded && isJumpPressed) {
            isJumping = true;
            jumpAnimation = StartCoroutine(JumpAnim());
        }
        else if (!isJumpPressed && characterController.isGrounded && isJumping) {
            isJumping = false;
            animator.SetBool(landedHash, true);
        }
    }
    //
    //DEV ONLY - DELETE BEFORE FINAL BUILD
    void Devbreak(InputAction.CallbackContext context) {
        Debug.Break();
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

    IEnumerator JumpAnim() {
        //play animation at 10th frame
        animator.Play(jumpHash, 0, 10 / 71f);

        //drop player model slightly
        Vector3 temp = playerModel.transform.localPosition;
        playerModel.transform.localPosition = new Vector3(temp.x, -0.237f, temp.z);

        //wait for animation 4 frames
        yield return new WaitForSeconds(2 / 30f);

        //apply upward force
        currentMovement.y = initJumpVelocity;
        currentRunMovement.y = initJumpVelocity;
        currentCrouchMovement.y = initJumpVelocity;

        //set animation bool
        animator.SetBool(landedHash, false);

        //reset player model
        playerModel.transform.localPosition = temp;

        //clear coroutine variables
        jumpAnimation = null;
    }
}
