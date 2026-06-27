using UnityEngine;

public class PlayerMovementInput : MonoBehaviour
{
    //This one stores the input of the player and is used to determine the movement of the player
    [Header("Input")]
    [SerializeField] private float horizontalInput;
    [SerializeField] private float verticalInput;
    [SerializeField] private bool  pressedJump;

   
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown("space"))
        {
            pressedJump = true;
        }
        if(Input.GetKeyUp("space"))
        {
            pressedJump = false;
        }
    }


    public float getHorizontalInput(){
        return horizontalInput;
    }
    public float getVerticalInput(){
        return verticalInput;
    }

    public bool getJumpBool()
    {
        return pressedJump;
    }
    
}
