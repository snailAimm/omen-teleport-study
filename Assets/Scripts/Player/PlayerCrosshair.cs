using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerCrosshair : MonoBehaviour
{
    public PlayerBlinkTiers playerBlinkTiers;
    public Image crosshairImage;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        changeCrosshairColour();
    }

    private void changeCrosshairColour()
    {
        switch(playerBlinkTiers.currentBlinkTier)
        {
            case BlinkTier.Close:
                crosshairImage.color = Color.green; // Example color for Close tier
                break;
            case BlinkTier.Far:
                crosshairImage.color = Color.red; // Example color for Far tier
                break;
        }
    }
}
