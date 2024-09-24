using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {

    //Component references
    ActionAsset actionAsset;
    CharacterController characterController;
    Animator animator;
    //AnimatorOverrideController animatorOverrideController;
    [SerializeField] MousePositionTracking mousePositionTracker;
    [SerializeField] GameObject playerModel;
    [SerializeField] Transform projectileSpawn;

    //Player variables
    float currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    Vector3 currentCrouchMovement;
    Vector3 appliedMovement;
    bool isMovementPressed;
    bool isRunPressed;
    bool isCrouchPressed;
    bool isBlockPressed;
    bool isFacingLeft;
    bool wasFlippedLastFrame;
    bool isJumpPressed = false;

    //Jump Varbiables
    [SerializeField] float maxJumpHeight; //SII
    [SerializeField] float maxJumpTime; //SII
    float initJumpVelocity;
    bool isJumping = false;

    //Movement Varbiables
    [SerializeField] float movementSpeed;
    [SerializeField] float sprintMultiplier;
    [SerializeField] float dashForce;

    //Gravity Variables
    [SerializeField][Range(-0.1f, -20f)] float gravity = -9.8f; //SII
    float groundedGravity = -0.05f;

    //Animation Variables
    int isWalkingHash;
    int isRunningHash;
    int isCrouchingHash;
    int isBackwardWalkingHash;
    int isBlockingHash;
    int landedHash;
    int jumpHash;
    int fallHash;
    int turnHash;
    int walkHash;
    int blockHash;
    int meleeHash;
    int castHash;
    int dashForwardHash;
    int dashbackwardHash;
    //
    Coroutine turnAnimation;
    Coroutine jumpAnimation;
    Coroutine dashAnimation;
    Coroutine castAnimation;

    //**Unity Methods    
    void Awake() {
        //Initialize
        actionAsset = new ActionAsset();
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        //animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        //animator.runtimeAnimatorController = animatorOverrideController;
        //
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isCrouchingHash = Animator.StringToHash("isCrouching");
        isBackwardWalkingHash = Animator.StringToHash("isBackwardWalking");
        isBlockingHash = Animator.StringToHash("isBlocking");
        landedHash = Animator.StringToHash("isLanded");
        jumpHash = Animator.StringToHash("Jump");
        fallHash = Animator.StringToHash("Fall");
        turnHash = Animator.StringToHash("Turn");
        walkHash = Animator.StringToHash("Walk");
        blockHash = Animator.StringToHash("Block");
        meleeHash = Animator.StringToHash("Melee");
        castHash = Animator.StringToHash("Cast");
        dashForwardHash = Animator.StringToHash("Dash_Forward");
        dashbackwardHash = Animator.StringToHash("Dash_Backward");
        //
        animator.SetBool(landedHash, true);


        //Define callbacks
        actionAsset.Player.Move.started += OnMovementInput;
        actionAsset.Player.Move.canceled += OnMovementInput;
        actionAsset.Player.Move.performed += OnMovementInput;
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
        actionAsset.Player.Block.started += OnBlock;
        actionAsset.Player.Block.canceled += OnBlock;
        //
        actionAsset.Player.Melee.started += OnMelee;
        //
        actionAsset.Player.Cast.started += OnCast;
        //
        actionAsset.Player.Dash.started += OnDash;
        //
        actionAsset.Player.MoveCamera.performed += Camera.main.GetComponent<CameraController>().CycleCameraPosition;
        //
        actionAsset.Player.DEVBREAK.performed += Devbreak;


        SetupJumpVariables();
    }
    //
    void Update() {

        //DEV ONLY
        SetupJumpVariables();
        //Debug.Log(isFacingLeft);



        HandleAnimation();
        mousePositionTracker.GetMousePosition();

        CollisionFlags collisionFlags;

        if (isRunPressed) {
            appliedMovement.x = currentRunMovement.x;
            appliedMovement.z = currentRunMovement.z;
        }
        else if (isCrouchPressed) {
            appliedMovement.x = currentCrouchMovement.x;
            appliedMovement.z = currentCrouchMovement.z;
        }
        else {
            appliedMovement.x = currentMovement.x;
            appliedMovement.z = currentMovement.z;
        }

        //stop movement if turning
        if (turnAnimation != null) {
            //stop movement of player controller
            appliedMovement.x = 0;
        }

        collisionFlags = characterController.Move(appliedMovement * Time.deltaTime);

        if (wasFlippedLastFrame) {
            wasFlippedLastFrame = false;
        }
        //Facing player based on Mouse position
        if ((!isFacingLeft && Input.mousePosition.x < Screen.width / 2f) || (isFacingLeft && Input.mousePosition.x > Screen.width / 2f)) {
            if (turnAnimation == null) {
                turnAnimation = StartCoroutine(TurnAnim());
                wasFlippedLastFrame = true;
            }
        }

        HandleGravity();
        HandleJump();

        if (jumpAnimation == null && !characterController.isGrounded) {
            //not landed
            animator.SetBool(landedHash, false);
            //crossfade into falling
            animator.CrossFade(fallHash, 0.01f);

        }
        else if (characterController.isGrounded && !animator.GetBool(landedHash)) {
            if (isMovementPressed) {
                animator.CrossFade(walkHash, 0.01f);
            }

            //not landed
            animator.SetBool(landedHash, true);
        }

        //test for vertical collisions when jumping (BINARY COMPARE bit mask and above flag, make sure they match (i.e. equal 1 because they are the same))
        if ((collisionFlags & CollisionFlags.Above) != 0) {
            currentMovement.y = 0;
            currentRunMovement.y = 0;
            currentCrouchMovement.y = 0;

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
    public void OnMovementInput(InputAction.CallbackContext context) {


        //Read values from Input System
        currentMovementInput = -1 * context.ReadValue<float>();


        //Negate movement while turning or blocking
        if (turnAnimation != null || isBlockPressed) {
            currentMovementInput = 0;
        }

        //Setup movement vectors, part by part
        currentMovement.x = currentMovementInput * movementSpeed;
        currentRunMovement.x = currentMovementInput * sprintMultiplier * movementSpeed;
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
    public void OnBlock(InputAction.CallbackContext context) {
        isBlockPressed = context.ReadValueAsButton();
    }
    //
    public void OnMelee(InputAction.CallbackContext context) {
        animator.CrossFade(meleeHash, 0.01f);
    }
    //
    public void OnDash(InputAction.CallbackContext context) {

        if (dashAnimation == null) {
            dashAnimation = StartCoroutine(DashAnim());
        }

    }
    //
    public void OnCast(InputAction.CallbackContext context) {

        if (castAnimation == null) {
            castAnimation = StartCoroutine(CastAnim());
        }

    }
    //
    void HandleAnimation() {
        //get param values from animator
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);
        bool isCrouching = animator.GetBool(isCrouchingHash);
        bool isBackwardWalking = animator.GetBool(isBackwardWalkingHash);
        bool isBlocking = animator.GetBool(isBlockingHash);
        //get current movement direction (Right is "forward" and stored here as a positive 1)
        float movementDir = currentMovementInput * -1;


        if (isBlockPressed && !isBlocking) {
            animator.SetBool(isBlockingHash, true);
            animator.CrossFade(blockHash, 0.1f);
        }
        else if (!isBlockPressed && isBlocking) {
            animator.SetBool(isBlockingHash, false);
        }


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

        //Start walking if movement pressed while not walking
        if (isMovementPressed && !isWalking) {

            //Test direction
            if (movementDir > 0) {
                if (!isFacingLeft) {
                    //Forward towards the right
                    animator.SetBool(isWalkingHash, true);
                }
                else {
                    //backward towards the right
                    animator.SetBool(isBackwardWalkingHash, true);
                }


            }
            else {
                if (!isFacingLeft) {
                    //backward towards the left
                    animator.SetBool(isBackwardWalkingHash, true);
                }
                else {
                    //Forward towards the left
                    animator.SetBool(isWalkingHash, true);
                }
            }

        }
        //Stop walking in either direction if movement not pressed while already walking
        else if (!isMovementPressed && (isWalking || isBackwardWalking)) {
            animator.SetBool(isWalkingHash, false);
            animator.SetBool(isBackwardWalkingHash, false);
        }


        //Start run if movement and run pressed while not currently runnning and facing direction of run
        if ((isMovementPressed && isRunPressed) && !isRunning) {
            animator.SetBool(isRunningHash, true);
        }
        //Stop run if movement or run are not pressed while currently running
        else if ((!isMovementPressed || !isRunPressed) && isRunning) {
            animator.SetBool(isRunningHash, false);
        }

        //flip run and backward run
        if (wasFlippedLastFrame) {

        }

    }
    //
    void HandleGravity() {
        bool isFalling = currentMovement.y <= 0f || !isJumpPressed;
        float fallMultiplier = 2f;

        //Sets gravity every frame
        if (characterController.isGrounded) {
            currentMovement.y = groundedGravity;
            currentCrouchMovement.y = groundedGravity;
            currentRunMovement.y = groundedGravity;
            appliedMovement.y = groundedGravity;
        }
        else if (isFalling) {
            //applies gravity when falling scaled by mult
            float prevYVelocity = currentMovement.y;
            currentMovement.y = currentMovement.y + (gravity * fallMultiplier * Time.deltaTime);
            appliedMovement.y = (prevYVelocity + currentMovement.y) * .5f;
        }
        //applies gravity every frame
        else {
            //apply velocity verlet intergration
            float prevYVelocity = currentMovement.y;
            currentMovement.y = currentMovement.y + (gravity * Time.deltaTime);
            appliedMovement.y = (prevYVelocity + currentMovement.y) * .5f;


            //else if (isFalling) {
            //    //applies gravity when falling scaled by mult
            //    float prevYVelocity = currentMovement.y;
            //    float newYVelocity = currentMovement.y + (gravity * fallMultiplier * Time.deltaTime);
            //    float nextYVelocity = (prevYVelocity + newYVelocity) * .5f;
            //    currentMovement.y = nextYVelocity;
            //    currentCrouchMovement.y = nextYVelocity;
            //    currentRunMovement.y = nextYVelocity;
            //}
            ////applies gravity every frame
            //else {
            //    //apply velocity verlet intergration
            //    float prevYVelocity = currentMovement.y;
            //    float newYVelocity = currentMovement.y + (gravity * Time.deltaTime);
            //    float nextYVelocity = (prevYVelocity + newYVelocity) * .5f;
            //    currentMovement.y = nextYVelocity;
            //    currentCrouchMovement.y = nextYVelocity;
            //    currentRunMovement.y = nextYVelocity;
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

            if (isMovementPressed) {
                animator.CrossFade(walkHash, 0.01f);
            }
            animator.SetBool(landedHash, true);

        }
    }
    //
    //DEV ONLY - DELETE BEFORE FINAL BUILD
    void Devbreak(InputAction.CallbackContext context) {
        Debug.Break();
        if (!Application.isEditor) {
            if (Time.timeScale != 0) {
                Time.timeScale = 0;
            }
            else {
                Time.timeScale = 1;
            }
        }


    }

    //**Coroutines**
    IEnumerator TurnAnim() {

        //set new rotation
        Quaternion newRotation;

        //Turn left to right
        if (isFacingLeft) {
            isFacingLeft = false;
            newRotation = Quaternion.Euler(0, -90, 0);
            //test if walking right
            if (isMovementPressed && appliedMovement.x < 0) {

                animator.SetBool(isBackwardWalkingHash, false);
                animator.SetBool(isWalkingHash, true);
            }
            //test if walking left
            else if (isMovementPressed && appliedMovement.x > 0) {
                animator.SetBool(isWalkingHash, false);
                animator.SetBool(isBackwardWalkingHash, true);
            }

        }
        //Turn right to left
        else {
            isFacingLeft = true;
            newRotation = Quaternion.Euler(0, 90, 0);
            //test if walking right
            if (isMovementPressed && appliedMovement.x < 0) {
                animator.SetBool(isWalkingHash, false);
                animator.SetBool(isBackwardWalkingHash, true);
            }
            //test if walking left
            else if (isMovementPressed && appliedMovement.x > 0) {
                animator.SetBool(isBackwardWalkingHash, false);
                animator.SetBool(isWalkingHash, true);
            }
        }

        //Crossfade into animation
        animator.CrossFade(turnHash, 0.01f);

        //wait for it to play
        yield return new WaitForSeconds(.2f);

        playerModel.transform.localRotation = newRotation;

        //Clear coroutine object
        turnAnimation = null;

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
        appliedMovement.y = initJumpVelocity;

        //set animation bool
        animator.SetBool(landedHash, false);

        //reset player model
        playerModel.transform.localPosition = temp;

        //clear coroutine variables
        jumpAnimation = null;
    }

    IEnumerator DashAnim() {
        float dashX = dashForce;
        animator.CrossFade(dashForwardHash, 0.1f);

        //test direction and flip force
        if (!isFacingLeft) {
            dashX *= -1;
        }

        //apply dash force        
        currentMovement.x = dashX;
        currentRunMovement.x = dashX;
        currentCrouchMovement.x = dashX;

        //wait for animation
        yield return new WaitForSeconds(0.4f);

        //reset movement i X dimension
        currentMovement.x = 0;
        currentRunMovement.x = 0;
        currentCrouchMovement.x = 0;

        //reset flipped force
        if (!isFacingLeft) {
            dashX *= -1;
        }

        dashAnimation = null;
    }

    IEnumerator CastAnim() {

        //get reference to spellbook
        SpellBook spellBook = GetComponent<SpellBook>();

        //get active spell anim clip
        AnimationClip clip = spellBook.GetSpellAnimation();

        //create Animaotr overide controller - basically a new animation controller for the overrides
        AnimatorOverrideController aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);
        //Set animator controller to new override controller
        animator.runtimeAnimatorController = aoc;

        //Loop through every clip in animator
        foreach (AnimationClip racClip in animator.runtimeAnimatorController.animationClips) {
            //test if clip is attack clip
            if (racClip.name.Contains("Mage@Attack")) {
                //replace it in override controller with proper clip from spellbook
                aoc[racClip.name] = clip;
            }
        }

        //play animation state
        animator.CrossFade(castHash, 0.01f);

        //Set anim delay
        int delayFrame = 0;
        switch (spellBook.ActiveSpell) {
            case 0:
                delayFrame = 15;
                break;
            case 1:
                delayFrame = 11;
                break;
            case 2:
                delayFrame = 9;
                break;
        }

        //scale delay based on speed of animation
        float delay = (delayFrame / 30f) / animator.GetCurrentAnimatorStateInfo(0).speed;

        //wait for delay
        yield return new WaitForSeconds(delay);

        //FIRE FROM SPELL BOOK
        GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        //test if cursor is not between player and spawn point
        if (Vector3.Distance(gm.GetPlayerPivot(), gm.GetMousePositionInWorldSpace()) > Vector3.Distance(gm.GetPlayerPivot(), projectileSpawn.position)) {
            spellBook.Cast();
        }

        //Test for spell 2 (DOUBLE FIRE)
        if (spellBook.ActiveSpell == 1) {
            yield return new WaitForSeconds(6f / 30f);
            if (Vector3.Distance(gm.GetPlayerPivot(), gm.GetMousePositionInWorldSpace()) > Vector3.Distance(gm.GetPlayerPivot(), projectileSpawn.position)) {
                spellBook.Cast();
            }
        }

        //Wait for animation to finish
        yield return new WaitUntil(() => !animator.GetCurrentAnimatorStateInfo(0).IsName("Cast"));

        //reset cast coroutine variable
        castAnimation = null;
    }
}
