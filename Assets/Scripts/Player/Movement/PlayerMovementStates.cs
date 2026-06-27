using UnityEngine;
public enum MovementState{
    Idle,
    Walking,
    Running,
    Air
     }

public class PlayerMovementStates : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private PlayerMovementInput playerMovementInput;
    [SerializeField] private MovementState movementState;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private RaycastHit slopeHit;
    [SerializeField] private LayerMask groundLayer;

    [Header("Number Variables")]
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private float maxSlopeAngle = 40f;


    [Header("Boolean Varialbes")]
    [SerializeField] private bool grounded = false;
    [SerializeField] private bool sloped = false;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool jumping = false;
    [SerializeField] private bool landed = false;
    //Scope is to determine the state of the player
    public void determineMovementState()
    {
        if (grounded)
        {
            if(Input.GetKey(KeyCode.LeftShift) && (playerMovementInput.getVerticalInput() != 0 || playerMovementInput.getHorizontalInput() != 0)){
            movementState = MovementState.Running;
            }
            else if(playerMovementInput.getVerticalInput() != 0 || playerMovementInput.getHorizontalInput() != 0){
            movementState = MovementState.Walking;
            }
            else{
            movementState = MovementState.Idle;
            }
        }
        else
        {
            movementState = MovementState.Air;

        }


    }
    public void Update()
    {
        
        determineMovementState();
        
        //Debug.Log(movementState);
    }

    public void FixedUpdate()
    {
        determineGrounded();
        determineSlope();
    }

    public void determineGrounded()
    {
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);
        if (grounded)
        {
            canJump = true;
            jumping = false;
        }
        
        //Debug.Log(Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer));
    }

    private void determineSlope()
    {
        Debug.DrawRay(transform.position, Vector3.down * (playerHeight * 0.5f + 0.3f), Color.red , 20 );

        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f) && grounded)
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            sloped = angle < maxSlopeAngle && angle != 0;
            //Debug.Log("Angle " + angle);
        }
        else
        {
            sloped = false;
        }


        //Debug.Log("Am I on Slope?: " + sloped);
    }



    //-----------------------------------
    public MovementState getPlayerMovementState()
    {
        return movementState;
    }
    public bool getSlopeBool()
    {
        return sloped;
    }
    public RaycastHit getSlopeRaycastHit()
    {
        return slopeHit;
    }
    public bool getGroundedBool()
    {
        return grounded;
    }
    public bool getCanJumpBool()
    {
        return canJump;
    }
    public void setCanJumpBool(bool canJump)
    {
        this.canJump = canJump;
    }
    public bool getJumpingBool()
    {
        return jumping;
    }
    public void setJumpingBool(bool jumping)
    {
        this.jumping = jumping;
    }
    public bool getLandingBool()
    {
        return landed;
    }
}
