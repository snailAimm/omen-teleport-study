using UnityEngine;

public class FrontBlink : MonoBehaviour
{
    [SerializeField] private PlayerBlinkTiers blinkTiers;
    [SerializeField] private PlayerMovementInput playerMovement;
    [SerializeField] private PlayerMovementPhysics playerMovementPhysicsScript;
    [SerializeField] private FrontBlinkIndicator indicatorScript;
    [SerializeField] private Transform playerCam;

    [SerializeField] private Vector3 indicatorTargetPosition;

    public void ShowIndicator()
    {
        Vector3 candidatePosition = GetFrontBlinkCandidate();

        if (TryGetGround(candidatePosition, out Vector3 groundedPosition))
        {
            float heightDifference = groundedPosition.y - playerMovementPhysicsScript.getRigidBody().position.y;

            if (heightDifference > 0.2f)
            {
                if (TryLandOnEdge(groundedPosition, out Vector3 edgePosition))
                {
                    indicatorTargetPosition = edgePosition;
                }
                else
                {
                    indicatorScript.indicator.SetActive(false);
                    return;
                }
            }
            else
            {
                indicatorTargetPosition = groundedPosition;
            }

            indicatorScript.indicator.transform.position = indicatorTargetPosition;
            indicatorScript.indicator.SetActive(true);
        }
        else
        {
            indicatorScript.indicator.SetActive(false);
        }
    }

    private Vector3 GetFrontBlinkCandidate()
    {
        Vector3 cameraDirection = playerCam.forward;
        Vector3 playerPosition = playerMovementPhysicsScript.getRigidBody().position + Vector3.up * 1.2f;
        float blinkDistance = blinkTiers.GetBlinkDistance();

        Debug.DrawRay(playerPosition, cameraDirection * blinkDistance, Color.red);

        if (Physics.Raycast(playerPosition, cameraDirection, out RaycastHit hit, blinkDistance, ~LayerMask.GetMask("Player")))
        {
            return hit.point + hit.normal * 0.5f; //This is an offset so its safe
        }

        return playerPosition + cameraDirection * blinkDistance; //This is Max
    }

    private bool TryGetGround(Vector3 testPosition, out Vector3 groundedPosition)
    {
        float groundCheckDistance = 10f; //u renamed my max distance hmph

        Debug.DrawRay(testPosition, Vector3.down * groundCheckDistance, Color.blue);

        if (Physics.Raycast(testPosition, Vector3.down, out RaycastHit hit, groundCheckDistance, ~LayerMask.GetMask("Player")))
        {
            groundedPosition = hit.point; //Similar to mine, but you actually export this, instead of mutating something else.
            return true;
        }

        groundedPosition = Vector3.zero; //Ah safely say No ok
        return false;
    }

    private bool TryLandOnEdge(Vector3 startingGroundPosition, out Vector3 edgePosition)
    {
        Vector3 pullDirection = playerMovementPhysicsScript.getRigidBody().position - startingGroundPosition;
        pullDirection.y = 0f;
        pullDirection.Normalize(); //Still the same as you thought me, yeah if we subtract those two Vectors we get the direction. 
        //Normalise since its a direction

        float originalY = startingGroundPosition.y;
        float yTolerance = 0.1f;
        float stepDistance = 0.2f;
        //This one... ok need clearer stuff here

        int attempts = 0;
        int maxAttempts = 40;

        Vector3 testPosition = startingGroundPosition;  //This feels to be the one getting changed?
        Vector3 lastValidPosition = startingGroundPosition; //Then this one feels like yeah the last valid spot, like my psuedo code
        //Those are better names, but it seems hm, startingGroundPosition seems to be where the player was
        //Then Edge Position is well the edge duh

        while (attempts < maxAttempts)
        {
            testPosition += pullDirection * stepDistance; //oh step distance is right right ok gets

            if (!TryGetGround(testPosition + Vector3.up * 1f, out Vector3 groundedPosition)) //Ok this one I need explanation
            {
                break;
            }

            if (Mathf.Abs(groundedPosition.y - originalY) > yTolerance) //Ok this one is y axis stuff, im guessing once the original Y of the indicator is different from the new Yaxis then we break ok
            {
                break;
            }

            lastValidPosition = groundedPosition;
            attempts++;

            //Debug.Log("Pulling edge target");
        }

        edgePosition = lastValidPosition;
        return true;
    }

    public Vector3 PreciseBlinkDestination()
    {
        return indicatorTargetPosition;
    }

    public void HideIndicator()
    {
        indicatorScript.indicator.SetActive(false);
    }

    public void BlinkToDestination()
    {
        playerMovementPhysicsScript.setRigidBodyPosition(indicatorTargetPosition);
    }
}