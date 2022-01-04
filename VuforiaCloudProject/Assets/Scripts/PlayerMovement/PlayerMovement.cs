using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

public class PlayerMovement : MonoBehaviour
{

    [Header("Camera")]
    [SerializeField] Camera mainCamera;
    [Header("PlayerController")]
    [SerializeField] private GameObject player;
    Rigidbody rb;
    [SerializeField] private CharacterController controller;
    private PlayerInput pI;
    private DefaultPlayerInput playerInputActionsAsset;
    private InputAction inputAction;//move
    [Header("Input Value")]
    [SerializeField] private float movementForce = 1f;
    [SerializeField] private float walkSpeed = 1f;//minSpeed
    [SerializeField] private float runSpeed = 5f;//maxSpeed
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float turnSmoothTime = .1f;
    [SerializeField] private float turnSmoothVelocity;
    private Vector3 forceDirection = Vector3.zero;


    private void Awake()
    {

        rb = player.GetComponent<Rigidbody>();
        pI = player.GetComponent<PlayerInput>();
        controller = player.GetComponent<CharacterController>();

        playerInputActionsAsset = new DefaultPlayerInput();

    }

    private void FixedUpdate()
    {

        //Player Controller and Movement (Old)
        //PlayerController(player, rb, controller, walkSpeed, runSpeed, jumpForce, turnSmoothTime, turnSmoothVelocity);
        //CameraMovement();

        //New Player Input Controller
        FixedUpdateExecutionFunction();
        CamControllerCinemaMachine();


    }



    #region Player_Movement_And_Controll_TPS

    //Old Controll System / Input System
    #region Old_PlayerControllerVersion
    //Need to add CharacterController at Inspector of the player
    void PlayerController(GameObject player, Rigidbody rb, CharacterController controller, float walkSpd, float runSpd, float jumpAmt, float turnSmoothTime, float turnSmoothVelocity)
    {

        rb = player.GetComponent<Rigidbody>();
        Transform cam = mainCamera.transform;

        float xDirection = Input.GetAxisRaw("Horizontal");
        float zDirection = Input.GetAxisRaw("Vertical");
        Vector3 moveDir = new Vector3(xDirection, 0.0f, zDirection).normalized;

        if (moveDir.magnitude >= 0.1f)
        {
            ///point the dir to the point that player moving at using Atan2 function
            ///it will give us the angle in radiance
            float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            //adding smoothness for the rotation of the player
            float angle = Mathf.SmoothDampAngle(player.transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            //now for rotation
            player.transform.rotation = Quaternion.Euler(0f, angle, 0f);
            //make player follow the camera direction
            Vector3 camMoveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(camMoveDir.normalized * walkSpd * Time.deltaTime);

        }
        else if (moveDir.magnitude >= 0.1f && Input.GetKeyDown(KeyCode.LeftShift))
        {

            float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(player.transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            player.transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 camMoveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(camMoveDir.normalized * runSpd * Time.deltaTime);

        }


    }

    void CameraMovement()
    {

        //U need to install the Package Cinemachine first and setting it for Following and Look at player
        //Setting the orbits including binding mode to world space
        //add extension at the bottom of the tps cinemamachine to CinemamachineCollider and set it to the ground layer
        //then hide the mouse at in game
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        //if isPaused == true then make cursor = true and lockstate = CursorLockMode.Confined

    }
    #endregion

    //New Controll / Input System

    #region NewControllerVersion

    //Player Movement

    private void FixedUpdateExecutionFunction()
    {
        
        //New Player Input Controller
        forceDirection += inputAction.ReadValue<Vector2>().x * GetCameraRight(mainCamera) * movementForce;
        forceDirection += inputAction.ReadValue<Vector2>().y * GetCameraForward(mainCamera) * movementForce;

        rb.AddForce(forceDirection, ForceMode.Impulse);
        forceDirection = Vector3.zero;

        //make your player fall faster so it won't be floating bugs errors
        if (rb.velocity.y < 0f)
        {
            rb.velocity -= Vector3.down * Physics.gravity.y * Time.fixedDeltaTime;
        }

        //for caping our player speed so our player speed not become extreamely fast (just horizontal speed)
        Vector3 horizontalVelocity = rb.velocity;
        horizontalVelocity.y = 0;
        if (horizontalVelocity.sqrMagnitude > runSpeed * runSpeed)
        {
            rb.velocity = horizontalVelocity.normalized * runSpeed + Vector3.up * rb.velocity.y;
        }

        LookAtController();

    }

    private void LookAtController()
    {

        //change the player rotation(z pos) to the direction of the movement (facing the dir of movement)
        Vector3 direction = rb.velocity;
        direction.y = 0f;//just want to rotate based on the y axis
        //as long as we're giving the input, we want to change the direction what we look
        if (inputAction.ReadValue<Vector2>().sqrMagnitude > 0.1f && direction.sqrMagnitude > 0.1f) 
        {
            this.rb.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
        else
        {
            rb.angularVelocity = Vector3.zero;
        }

    }

    private Vector3 GetCameraRight(Camera playerCam)
    {
        Vector3 right = playerCam.transform.right;
        right.y = 0f;
        return right.normalized;
    }
    private Vector3 GetCameraForward(Camera playerCam)
    {
        Vector3 forward = playerCam.transform.forward;
        forward.y = 0f;
        return forward.normalized;
    }

    private void OnEnable()
    {

        playerInputActionsAsset.Player.Jump.started += Jump_performed;
        inputAction = playerInputActionsAsset.Player.Move;
        playerInputActionsAsset.Player.Enable();

    }
    
    private void OnDisable()
    {

        playerInputActionsAsset.Player.Jump.started -= Jump_performed;
        playerInputActionsAsset.Player.Disable();

    }

    public void Jump_performed(InputAction.CallbackContext callback)
    {

        if (IsGrounded())
        {

            forceDirection += Vector3.up * jumpForce;

        }

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

    }

    private bool IsGrounded()
    {

        Ray ray = new Ray(this.transform.position + Vector3.up * 0.25f, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, 0.3f))
        {
            return true;
        }
        else
        {
            return false;
        }

    }


    private void CamControllerCinemaMachine()
    {
        
        //U need to install the Package Cinemachine first and setting it for Following and Look at player
        //Setting the orbits including binding mode to world space
        //add extension at the bottom of the tps cinemamachine to CinemamachineCollider and set it to the ground layer
        //add cinema machine collider at AddExtension
        //Collide Againist Layer that you want to collide and ignore player tag / layer
        //Damping and Damping when Occluded = 0.6f
        //Connect unity new input system with cinema machine with CinemaMachineInputProvider
        //then hide the mouse at in game
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }

    #endregion

    #endregion
}
