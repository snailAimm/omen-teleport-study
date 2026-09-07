using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** This script is inspired by "Dave/GameDevelopment" on youtube.
 * Massive props to him
 */

/**<summar>

This is Summary!

**/

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;
    private float mouseX;
    private float mouseY;

    public Transform orientation;

    float xRotation;
    float yRotation;

    Camera cam;
    public float normalFOV = 60f;
    private float targetFOV = 60f;
    private float smoothSprintTime = 0.2f;
    private float smoothFallTime = 0.9f;
    private float currentVelocity = 0.0f;

    public PlayerMovementStates playerMovementStatesScript;
    public PlayerMovementPhysics playerMovementPhysicsScript;

    [SerializeField] private bool playerIsChangingFOV = false;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        //Mouse inputs
        CameraRotation();

        if (!playerIsChangingFOV)
        {
            if (playerMovementStatesScript.getPlayerMovementState() == MovementState.Air)
            {
                applyFallingFOV();
            }
            else
            {
                applySprintFOV();
            }
        }
    }


    private void Awake()
    {
        cam = GetComponent<Camera>(); //Get Camera
    }


    //CAMERA ROTATIONS
    private void CameraRotation()
    {
        mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); //Stop the camera from flipping over

        //Rotate cam and orientation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    private void applySprintFOV()
    {
        if (Input.GetKey(KeyCode.LeftShift) && playerMovementStatesScript.getGroundedBool())
        {
            targetFOV = 73f;
        }
        else
        {
            targetFOV = 60;
        }

        cam.fieldOfView = Mathf.SmoothDamp(cam.fieldOfView, targetFOV, ref currentVelocity, smoothSprintTime);
    }

    private void applyFallingFOV()
    {
        if (playerMovementPhysicsScript.getRigidBody().linearVelocity.y < 0)
        {
            if (80f + Math.Abs(playerMovementPhysicsScript.getRigidBody().linearVelocity.y) < 120)
            {
                targetFOV = 70f + Math.Abs(playerMovementPhysicsScript.getRigidBody().linearVelocity.y);
            }
        }
        else
        {
            targetFOV = 60;
        }

        cam.fieldOfView = Mathf.SmoothDamp(cam.fieldOfView, targetFOV, ref currentVelocity, smoothFallTime);
    }






    public IEnumerator zoomOutFOV(float targetFOV, float speed)
    {
        playerIsChangingFOV = true;
       

        yield return null;

        while (cam.fieldOfView <= targetFOV - 2)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * speed);
            yield return null;
        }

        

        playerIsChangingFOV = false;
       
        yield return null;
    }

    public IEnumerator zoomInFOV(float targetFOV, float speed)
    {
        playerIsChangingFOV = true;
       

        yield return null;

        while (cam.fieldOfView >= targetFOV + 2) 
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * speed);

            yield return null;
        }

        
        playerIsChangingFOV = false;
       
        yield return null;
    }


}
