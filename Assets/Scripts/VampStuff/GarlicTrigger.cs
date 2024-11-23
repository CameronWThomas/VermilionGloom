using UnityEngine;

public class GarlicTrigger : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        CamsPlayerController playerController = other.GetComponent<CamsPlayerController>();
        if (playerController != null)
        {
            playerController.SetGarlic(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CamsPlayerController playerController = other.GetComponent<CamsPlayerController>();
        if (playerController != null)
        {
            playerController.SetGarlic(null);
        }
    }

}
