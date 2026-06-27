using UnityEngine;

public class FrontBlinkIndicator : MonoBehaviour
{
    public GameObject indicator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        indicator.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
