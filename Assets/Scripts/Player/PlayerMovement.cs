using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementa : MonoBehaviour
{
    //Again this is from Dave/GameDevelopment's Video
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    private float horizontalInput;
    private float verticalInput;

    [Header("Jumping")]
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump = true;
    public float speedDampening = 0.9f;

    [Header("Scientifically Correct Jumping")]
    public float jumpHeight;
    public float gravity;
    public float jumpVelocity;
    public float fallMultiplier = 3.1f;
    public float riseMultiplier = 2.9f;
    
    Vector3 moveDirection;

    public Rigidbody rb;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDrag;
    public  float groundDistance = 0.4f;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    public MovementState movementState;

    [Header("Camera Settings")]
    public PlayerCam playerCam;

    [Header("Slope Movement")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    public float playerHeight = 2f;
    private bool exitingSlope;

    public enum MovementState{
        walking,
        sprinting,
        air
    }


    //----------------------------------------

    private void Start(){
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; 

        jumpHeight = 2f;
        gravity = Mathf.Abs(Physics.gravity.y);
        jumpVelocity = Mathf.Sqrt(2 * jumpHeight * gravity);
    }

    private void Update(){
        //Ground Check
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, whatIsGround);       

        MyInput();
        SpeedControl();
        StateHandler();
        
        

        //Handle Drag
        if(grounded){
            rb.linearDamping = groundDrag;
        } else {
            rb.linearDamping = 0f;
        }
    }

    private void FixedUpdate(){
        MovePlayer();
        ForceGravity();
    }



    private void MyInput(){
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKey(KeyCode.Space) && readyToJump && grounded){
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }



    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (grounded)
        {
             rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if(!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
       
       Debug.Log(moveDirection);


    }

    private void SpeedControl(){
        //Limit speed on slope
         if(rb.linearVelocity.magnitude > moveSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
            }

        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if(flatVel.magnitude > moveSpeed){
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    //--------------------------------------

    private void Jump(){
        exitingSlope = true;
        //Consistent Jump Height! and Speed Dampening when Jumping
        rb.linearVelocity = new Vector3(rb.linearVelocity.x * speedDampening, 0f, rb.linearVelocity.z * speedDampening);

        //Actual Jumping
        rb.AddForce(transform.up * jumpVelocity, ForceMode.Impulse);
        
    }

    private void ResetJump(){
        readyToJump = true;

        exitingSlope = false;
    }

    private void ForceGravity() //Move that bitch down
    {
        if(rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += (Vector3.down * gravity * Time.deltaTime * (fallMultiplier - 1));
        }
        else if(rb.linearVelocity.y > 0)
        {
            rb.linearVelocity += (Vector3.down * gravity * Time.deltaTime * (riseMultiplier - 1));
        }

    }


    //--------------------------------------
    private void StateHandler(){
        //Sprinting
        if(grounded && Input.GetKey(KeyCode.LeftShift)){
            movementState = MovementState.sprinting;
            Sprinting();
            //Walking
        } else if(grounded){
            movementState = MovementState.walking;
            Walking();
        } else {
            //Air
            movementState = MovementState.air;
            Air();
        }

    }

    private void Sprinting(){
        moveSpeed = sprintSpeed;
    }

    private void Walking(){
        moveSpeed = walkSpeed;
        
    }

    private void Air(){
       //MOVE THAT BITCH DOWN
       
    }

    //--------------------------------------


    //------------------------------------------
    //Momentum

    public void removeAllMomentum()
    {
        rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
    }

    //------------------------------

    public float getHorizontalInput(){
        return horizontalInput;
    }

    public float getVerticalInput(){
        return verticalInput;
    }

    




}
