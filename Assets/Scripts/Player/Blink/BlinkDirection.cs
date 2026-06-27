using UnityEngine;
//THIS SCRIPT WILL DETERMINE THE DIRECTION OF THE BLINK
//FROM THE PLAYER.
public class BlinkDirection : MonoBehaviour
{
    
    private float horizontalInput;
    private float verticalInput;
    [SerializeField] private Transform playerOrientation; //Orientation of the player, where they're looking
    [SerializeField] private PlayerMovementInput playerMovementInputScript; //Reference to the playerMovementInputScript script to access input values
    


    public enum BlinkDirections
    {
        FRONT,
        HINDQUARTERS //This is for the sides and the back :)
    }
 

    public BlinkDirections DetermineDirection()
    {
        horizontalInput = playerMovementInputScript.getHorizontalInput();
        verticalInput = playerMovementInputScript.getVerticalInput();

        Vector3 direction = (playerOrientation.forward * verticalInput) +
                            (playerOrientation.right * horizontalInput);

        if(direction.magnitude < 0.1f){ //Culprit for anything related tooooo air
            return BlinkDirections.FRONT;
            
        }else if(verticalInput > 0f){//If going forwards
          return BlinkDirections.FRONT;

        }else{ //If going backwards or sideways
            return BlinkDirections.HINDQUARTERS;
            
        }
    }
}
