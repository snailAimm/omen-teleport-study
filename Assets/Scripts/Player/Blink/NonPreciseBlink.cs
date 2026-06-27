using UnityEditor.Callbacks;
using UnityEngine;

public class NonPreciseBlink : MonoBehaviour
{
       //This one shouldnt have any hard calculations, its only directional, no fancy stuff
       //First is to figure out which side it is.
       
    [SerializeField] private BlinkDirection blinkDirection; //Reference to the BlinkDirection script to determine the direction of the blink
    [SerializeField] private PlayerBlinkTiers blinkTiers;
    [SerializeField] private PlayerMovementInput playerMovementInputScript;
    [SerializeField] private PlayerMovementPhysics playerMovementPhysicsScript;
    [SerializeField] private Transform playerOrientation;
    [SerializeField] private Transform playerCam;

    public Vector3 NonPreciseBlinkDestination()
    {
        Vector3 targetPosition = CalculateBlinkDestination();

        if (CapsuleCheck(targetPosition))
        {
            return targetPosition;
        }
        else
        {
           targetPosition = DrawLineCheck(targetPosition);
        }


        return targetPosition;
    }

    public Vector3 CalculateBlinkDestination()
    {
        float blinkDistance = blinkTiers.GetBlinkDistance();
        float horizontalInput = playerMovementInputScript.getHorizontalInput();
        float verticalInput = playerMovementInputScript.getVerticalInput();
        Vector3 finalTargetPosition = new Vector3(0,0,0);
        
        BlinkDirection.BlinkDirections direction = blinkDirection.DetermineDirection();

        switch(direction)
        {
            case BlinkDirection.BlinkDirections.FRONT:
                Debug.Log("Camera Blink");
                Vector3 cameraDirection = playerCam.forward;
                finalTargetPosition = cameraDirection * blinkDistance;
                
                playerMovementPhysicsScript.resetMomentum();
                
                

                break;
            case BlinkDirection.BlinkDirections.HINDQUARTERS:
                Debug.Log("Side Blink");
                finalTargetPosition = new Vector3(horizontalInput, 0f, verticalInput) * blinkDistance;
                 finalTargetPosition = playerOrientation.TransformDirection(finalTargetPosition); 
                //Changes the direction to the oritentation of the player
                //Without it, it will only work on one direction
                break;
            default:
                break; 
        }
        finalTargetPosition += playerMovementPhysicsScript.getRigidBody().position;
        return finalTargetPosition;
    }


    public bool CapsuleCheck(Vector3 finalDestination)
    {
        
        float playerHeight = 1.8f;
        float radius = 0.3f;
        float ankleHeight = 0.15f;

        Vector3 capsuleEnd = finalDestination + Vector3.up * (ankleHeight + radius);
        Vector3 capsuleStart = finalDestination + Vector3.up * (ankleHeight + playerHeight - radius);

        bool isBlocked = Physics.CheckCapsule(capsuleStart, capsuleEnd, radius);

        if (isBlocked)
        {
            Debug.Log("Cant Teleport");
            return false;
        }

        Debug.Log("Can Teleport");
        return true;
    }

    public Vector3 DrawLineCheck(Vector3 finalDestination)
    {
        RaycastHit hit;

        float playerHeight = 1.8f;
        float radius = 0.4f;
        float offset = 0.1f;
        
        

        Vector3 startPosition, origin, finalPosition;
        startPosition = playerMovementPhysicsScript.getRigidBody().position;
        origin = startPosition + Vector3.up * (playerHeight/2);

        Vector3 direction = (finalDestination - startPosition).normalized;

        Debug.DrawRay(startPosition, finalDestination, Color.red, 5f);

        bool hasHit = Physics.SphereCast(origin, radius, direction,
        out hit, blinkTiers.GetBlinkDistance());

        if (hasHit)
        {
            Debug.Log("Ray hit asomething");
            float safeDistance = Mathf.Max(hit.distance - offset, 0f);
            finalPosition = startPosition + direction * safeDistance;
            return finalPosition;
        }
        Debug.Log("Ray missed");
        return finalDestination;
    }

    private void OnDrawGizmos()
    {
        float playerHeight = 1.8f;
        float radius = 0.3f;

        float blinkDistance = blinkTiers.GetBlinkDistance();
        float horizontalInput = playerMovementInputScript.getHorizontalInput();
        float verticalInput = playerMovementInputScript.getVerticalInput();

        Vector3 targetPosition = new Vector3(horizontalInput, 0f, verticalInput) * blinkDistance;
        targetPosition = playerOrientation.TransformDirection(targetPosition); 

        // Change this to targetPosition when testing blink destination
        Vector3 feetPosition = targetPosition + playerMovementPhysicsScript.getRigidBody().position;

        Vector3 capsuleBottom = feetPosition + Vector3.up * radius;
        Vector3 capsuleTop = feetPosition + Vector3.up * (playerHeight - radius);

        bool isBlocked = Physics.CheckCapsule(capsuleBottom, capsuleTop, radius);

        Gizmos.color = isBlocked ? Color.red : Color.green;

        Gizmos.DrawWireSphere(capsuleBottom, radius);
        Gizmos.DrawWireSphere(capsuleTop, radius);

        Gizmos.DrawLine(capsuleBottom + Vector3.right * radius, capsuleTop + Vector3.right * radius);
        Gizmos.DrawLine(capsuleBottom - Vector3.right * radius, capsuleTop - Vector3.right * radius);
        Gizmos.DrawLine(capsuleBottom + Vector3.forward * radius, capsuleTop + Vector3.forward * radius);
        Gizmos.DrawLine(capsuleBottom - Vector3.forward * radius, capsuleTop - Vector3.forward * radius);
    }
}
