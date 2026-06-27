using System.Collections;
using System.Data;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerMovementPhysics : MonoBehaviour
{
    [Header("Objects/Scripts")]
    [SerializeField] private PlayerMovementProcessor playerMovementProcessorScript;
    [SerializeField] private PlayerMovementStates playerMovementStateScript;
    [SerializeField] private Rigidbody rb;

    [Header("Gravity/Drag")]
    [SerializeField] private float maxGravity = 50f;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float groundDrag;
    [SerializeField] private float airDrag;
    [SerializeField] private float gravityScale;
    [SerializeField] private bool atApex = false;
    [SerializeField] private float hangTimeDrag;
    [SerializeField] private float hangTimePeriod;

    public void Start()
    {
        rb.freezeRotation = true;
    }

    public void Update()
    {
        applyDrag();
        
    }

    public void FixedUpdate()
    {
        
        applyMovementForce();
        applyJump();
        applyGravity();
        applySlopedGravity();
        
    }

    public void applyMovementForce()
    {
        Vector3 moveDirection = playerMovementProcessorScript.getMoveDirection();

        if (playerMovementStateScript.getSlopeBool())
        {
            applySlopeMovementForce();
        }
        else
        {
            if(playerMovementStateScript.getPlayerMovementState() == MovementState.Walking){
            rb.AddForce(moveDirection.normalized * playerMovementProcessorScript.getWalkingSpeed() * 5f, ForceMode.Force);
            }
            else if(playerMovementStateScript.getPlayerMovementState() == MovementState.Running)
            {
            rb.AddForce(moveDirection.normalized * playerMovementProcessorScript.getRunningSpeed() * 5f, ForceMode.Force);
            }
        }
        
    }

    public void applySlopeMovementForce()
    {
        Vector3 slopeDirection = playerMovementProcessorScript.getSlopeDirection();
        if(rb.linearVelocity.y > 0)//UP
            if(playerMovementStateScript.getPlayerMovementState() == MovementState.Running){
                rb.AddForce(slopeDirection * playerMovementProcessorScript.getSlopeUpSpeed() * 10f, ForceMode.Force);
                //Debug.Log("Im Faster Up!");
            }
            else{
                rb.AddForce(slopeDirection * playerMovementProcessorScript.getSlopeUpSpeed() * 8f, ForceMode.Force);
            }
        else if(rb.linearVelocity.y < 0) //DOWN
        {
                if(playerMovementStateScript.getPlayerMovementState() == MovementState.Running){
                    rb.AddForce(slopeDirection * playerMovementProcessorScript.getSlopeDownSpeed() * 10f, ForceMode.Acceleration);
                    //Debug.Log("Im Sliding");
                }
                else{
                    rb.AddForce(slopeDirection * playerMovementProcessorScript.getSlopeDownSpeed() * 4f, ForceMode.Force);
                }
        }
        else{
            rb.AddForce(slopeDirection * playerMovementProcessorScript.getSlopeUpSpeed() * 8f, ForceMode.Force);
        }
    }

    public void applyDrag()
    {
        rb.linearDamping = groundDrag;
        if(playerMovementStateScript.getPlayerMovementState() == MovementState.Walking || playerMovementStateScript.getPlayerMovementState() == MovementState.Idle 
        || playerMovementStateScript.getPlayerMovementState() == MovementState.Running)
        {
            rb.linearDamping = groundDrag;
        }
        else //Air
        {
            rb.linearDamping = airDrag;
        }

    }

    //ALL AIR STUFF

    public void applySlopedGravity()
    {
        //If Sloped, and then Velocity is Down? Y Velocity
        //If Sloped, then Velocity is going up
        
        if (playerMovementStateScript.getSlopeBool() && !playerMovementStateScript.getJumpingBool())
        {
            rb.useGravity = false;
            if(rb.linearVelocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f,ForceMode.Force);
                //Debug.Log("Going Up!");
            }
            else if(rb.linearVelocity.y < 0)
            {
                rb.AddForce(Vector3.down * 80f,ForceMode.Force);
                //Debug.Log("Going Down");
            }
            else
            {
                //Debug.Log("Still Ground");
            }
        }
        

    }
    
    public void applyGravity()
    {
        if (playerMovementStateScript.getGroundedBool())
        {
            rb.useGravity = true;
        }
        else if(playerMovementStateScript.getPlayerMovementState() == MovementState.Air)
        {
            if(rb.linearVelocity.y >= -2f && rb.linearVelocity.y <= 2f && playerMovementStateScript.getJumpingBool() && !atApex ) //Jumping
            {
                StartCoroutine(stopHangTime());
            }
            else if(rb.linearVelocity.y < maxGravity && rb.linearVelocity.y < 1 && playerMovementStateScript.getPlayerMovementState() == MovementState.Air) //Falling
            {
                rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
                
                Debug.Log("Im falling");
            }
           /* else if(rb.linearVelocity.y >= -2f && rb.linearVelocity.y <= 2f && playerMovementStateScript.getJumpingBool() && !atApex ) //Jumping
            {
                StartCoroutine(stopHangTime());
            }*/
        }
    }

    private void applyHangTime()
    {
        //Debug.Log("Stop Gravity");
        rb.useGravity = false;
        atApex = true;
        rb.linearDamping = hangTimeDrag;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

    }

    public IEnumerator stopHangTime()
    {
        float delayBeforeDrop = 0.5f;

        applyHangTime();
        yield return new WaitForSeconds(hangTimePeriod);
        rb.useGravity = true;
        
        

        yield return new WaitForSeconds(delayBeforeDrop);
        atApex = false;

        yield return null;
    }

    private void applyJump()
    {
        if(playerMovementProcessorScript.getJumpHeight() > 0 && playerMovementStateScript.getCanJumpBool() && !playerMovementStateScript.getJumpingBool() && (playerMovementProcessorScript.getCoyoteTime() > 0))
        {
            float jumpHeight = playerMovementProcessorScript.getJumpHeight();

            
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            
            rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
            

            playerMovementStateScript.setCanJumpBool(false);
            playerMovementStateScript.setJumpingBool(true);
            //Debug.Log("Jumping");
        }
    }

    //------------------------------------------------------GETTER,SETTER
    public Rigidbody getRigidBody()
    {
        return rb;

    }

    public void setRigidBodyPosition(Vector3 newPosition)
    {
        rb.position = newPosition;
    }

    public void resetMomentum()
    {
        rb.linearVelocity = playerMovementProcessorScript.getMoveDirection() * playerMovementProcessorScript.getWalkingSpeed();
    }
}
