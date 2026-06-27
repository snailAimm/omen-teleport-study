using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlink : MonoBehaviour
{
    //This should only have Scripts, and no other variables
    //It should only ORCHESTRATE the functions
    //This Determines which blinkscript to run
    
    //There wwill be more, such as in the air, or falling.

    [SerializeField] private BlinkDirection blinkDirection; //Reference to the BlinkDirection script to determine the direction of the blink
    [SerializeField] private FrontBlink frontBlink; //Reference to the FrontBlink script to
    [SerializeField] private NonPreciseBlink nonPreciseBlink; //Reference to the nonPreciseBlink script to
    [SerializeField] private PlayerMovementPhysics playerMovementPhysicsScript; //Reference to the PlayerMovement script to access input values
    
    [SerializeField] private PlayerCam playerCam; //Reference to the PlayerCam script to access FOV functions

    private void Update()
    {
        preciseBlink();
        quickBlink();
    }

    private void preciseBlink()
    {
        if(Input.GetMouseButton(2)){
            frontBlink.ShowIndicator();
        }
        else if(Input.GetMouseButtonUp(2)){
             frontBlink.BlinkToDestination();
             frontBlink.HideIndicator();

             
             StartCoroutine(playerCam.zoomInFOV(45, 40));


                    //playerMovement.removeAllMomentum(); //Remove momentum after blinking   
        }
    }

    private void quickBlink()
    {
        
        if(Input.GetKeyDown("e")){
            Vector3 targetPosition = nonPreciseBlink.NonPreciseBlinkDestination();
            playerMovementPhysicsScript.setRigidBodyPosition(targetPosition); // Move the player to the target position
            
            StartCoroutine(playerCam.zoomOutFOV(90, 20));
        }
    }


}


