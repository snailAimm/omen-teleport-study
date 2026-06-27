using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovementProcessor : MonoBehaviour
{
    [Header("Scripts and Objects")]
    [SerializeField] private PlayerMovementInput playerMovementInputScript;
    [SerializeField] private Transform orientation;
    [SerializeField] private PlayerMovementStates playerMovementStateScript;

    [Header("Calculated Numbers")]
    [SerializeField] private Vector3 moveDirection;
    [SerializeField] private Vector3 slopeDirection;

    [Header("Player Speed")]
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float runningSpeed;
    [SerializeField] private float slopeUpSpeed;
    [SerializeField] private float slopeDownSpeed;

    [Header("Player Jump")]
    [SerializeField] private float jumpHeight;
    [SerializeField] private float coyoteTimeLeft = 2f;
    


    public void Update()
    {
        
        moveDirection = orientation.forward * playerMovementInputScript.getVerticalInput() +
        orientation.right * playerMovementInputScript.getHorizontalInput();

        slopeDirection = Vector3.ProjectOnPlane(moveDirection, playerMovementStateScript.getSlopeRaycastHit().normal).normalized;

        changeJumpHeight();
        startCoyoteTime();
    }

    private void changeJumpHeight()
    {
        if (playerMovementInputScript.getJumpBool())
        {
            jumpHeight = 6.4f;
        }
        else
        {
            jumpHeight = 0f;
        }
    }

    public void startCoyoteTime()
    {
        if (playerMovementStateScript.getGroundedBool())
        {
            coyoteTimeLeft = 0.3f;
        }
        else
        {
            if(coyoteTimeLeft >= 0)
            {
                coyoteTimeLeft -= Time.deltaTime;
                
            }

        }
    }

    public Vector3 getMoveDirection()
    {
        return moveDirection;
    }

    public Vector3 getSlopeDirection()
    {
        return slopeDirection;
    }

    public float getWalkingSpeed()
    {
        return walkingSpeed;
    }

    public float getRunningSpeed()
    {
        return runningSpeed;
    }

    public float getSlopeUpSpeed()
    {
        return slopeUpSpeed;
    }

    public float getSlopeDownSpeed()
    {
        return slopeDownSpeed;
    }
    public float getJumpHeight()
    {
        return jumpHeight;
    }
    public float getCoyoteTime()
    {
        return coyoteTimeLeft;
    }
}
