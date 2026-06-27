using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraHeadBob : MonoBehaviour
{
    //If Grounded, and Not on Sloped, and holding WASD, Bob
    //IF ANY of those arent true, Lerp into Default

    //IF FALL AND HIT GROUND
    //CAMERA U      - Ate Rene, Ate Jill
    [Header("Conditions")]
    [SerializeField] private PlayerMovementStates playerMovementStatesScript;
    [SerializeField] private PlayerMovementPhysics playerMovementPhysicsScript;

    [Header("Camera Stuff")]
    [SerializeField] private float peakWalkBob = 0.1f; 
    [SerializeField] private float peakRunBob = 0.17f; 
    [SerializeField] private float walkSpeedBob = 12; // Speed of the wave
    [SerializeField] private float runSpeedBob = 16;
    [SerializeField] private float returnSpeedBob;
    [SerializeField] private float defaultY;
    [SerializeField] private float time;
    [SerializeField] private Vector3 defaultPosition;
    [SerializeField] private Vector3 newPosition;

    [Header("Landing")]
    [SerializeField] private float landingImpactSpeed;
    [SerializeField] private float landingImpactIntensity;
    [SerializeField] private bool recoveringFromLanding = false;
    [SerializeField] private float landingTime = 0;
    [SerializeField] private float landingCameraDuration;
    [SerializeField] private float landY = 0;
    [SerializeField] private float landYMax = 0;

    private void Start()
    {
        defaultY = transform.localPosition.y;
        defaultPosition = transform.localPosition;
    }

    private void Update()
    {
        bobbingCondition();
        findMaxNegativeVelocity();

        if (playerMovementStatesScript.getPlayerMovementState() == MovementState.Air && playerMovementStatesScript.getGroundedBool())
        {
            
            //Debug.Log("Landed!");
            StartCoroutine(landingBob(landYMax));
        }
    }

    private void bobbingCondition()
    {
         float horizontalInput = Input.GetAxisRaw("Horizontal");
         float verticalInput = Input.GetAxisRaw("Vertical");

        if((horizontalInput != 0 || verticalInput != 0) && playerMovementStatesScript.getGroundedBool() && !playerMovementStatesScript.getSlopeBool())
        {
            if(playerMovementStatesScript.getPlayerMovementState() == MovementState.Walking)
            {
                applyBobbing(walkSpeedBob, peakWalkBob);
            }
            else if(playerMovementStatesScript.getPlayerMovementState() == MovementState.Running)
            {
                applyBobbing(runSpeedBob, peakRunBob);
            }
            
            
        }
        else if(!recoveringFromLanding)
        {
           transform.localPosition = Vector3.Lerp(transform.localPosition, defaultPosition, Time.deltaTime * returnSpeedBob);
            time = 0;
        }
    
    }

    private void applyBobbing(float speedBob, float peakBob)
    {
        time += Time.deltaTime;
        float waveValue = Mathf.Sin(time * speedBob) * peakBob;
     
        if(waveValue == 0)
        {
            Debug.Log("Normal!");
        }
        //Debug.Log(waveValue);

        //I should Apply the offset?
        newPosition = transform.localPosition;
        newPosition.y = defaultY + waveValue;
        transform.localPosition = newPosition;
    }


    private float findMaxNegativeVelocity()
    {
        landY = playerMovementPhysicsScript.getRigidBody().linearVelocity.y;
        if(landYMax >= landY)
        {
            landYMax = landY;
        }

        return landYMax;

    }
    private IEnumerator landingBob(float fallingVelocity)
    {
        //When we LAND, we play a quick animation camera shake via changing a rotation of X.
        //We Play from wherever the X axis is to a *insert very smart formula that changes intensity of the drop*
        //Then we lerp it back to normal
        recoveringFromLanding = true;
        
        landingTime = 0; //So that the animation can play
        //Default is 1.6
        
        //Debug.Log(Mathf.Lerp(1.6f, 1.3f, Math.Abs(fallingVelocity/25)));

        Vector3 targetPosition = new Vector3(
            transform.localPosition.x, 
            Mathf.Lerp(1.6f, 1.3f, Math.Abs(fallingVelocity/25)),
            transform.localPosition.z
        );

        while (landingTime <= landingCameraDuration)
        {
            landingTime += Time.deltaTime; //Timer
            

            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * landingImpactSpeed);
            yield return null;
        }
        recoveringFromLanding = false;
        
        yield return null;
        landYMax = 0;
    }







}