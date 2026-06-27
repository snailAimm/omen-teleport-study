using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlinkTier
{
    Close,
    //Medium,
    Far
}

public class PlayerBlinkTiers : MonoBehaviour
{
    //So this one, will have 3 Tiers, Close, Medium, Far. And I want to work with ENUMS so then I can....
    //SEND THIS to the PlayerBlink File, But also Send THAT to Crosshair Script that will change depending on it.
    public BlinkTier currentBlinkTier = BlinkTier.Close;

    public void Update()
    {
        SetBlinkTier();
        
    }

    private void SetBlinkTier()
    {
        //This is where we will set the Blink Tier, depending on the distance of the Blink, we will set it to Close, Medium, or Far.
        //We can use the blinkDistance variable from the PlayerBlink script to determine this. //Using Scrollwheel
        if(Input.GetAxis("Mouse ScrollWheel") > 0f) // Scroll up
        {
            if(currentBlinkTier == BlinkTier.Close)
            {
                currentBlinkTier = BlinkTier.Far;
            }
        }
        else if(Input.GetAxis("Mouse ScrollWheel") < 0f) // Scroll down
        {
            if(currentBlinkTier == BlinkTier.Far)
            {
                currentBlinkTier = BlinkTier.Close;
            }
        }
    }

    public float GetBlinkDistance()
    {
        //This is where we will return the Blink Distance, depending on the current Blink Tier.
        switch(currentBlinkTier)
        {
            case BlinkTier.Close:
                return 11f; // Example distance for Close tier
            case BlinkTier.Far:
                return 25f; // Example distance for Far tier
            default:
                return 11f; // Default to Close tier distance
        }
    }

    public float GetBlinkMomentum()
    {
        //Yes its the same but for naming conventions
        switch(currentBlinkTier)
        {
            case BlinkTier.Close:
                return 8f; // Example momentum for Close tier
            case BlinkTier.Far:
                return 15f; // Example momentum for Far tier
            default:
                return 8f; // Default to Close tier momentum
        }
    }

    public BlinkTier GetCurrentBlinkTier()
    {
        return currentBlinkTier;
    }
}
