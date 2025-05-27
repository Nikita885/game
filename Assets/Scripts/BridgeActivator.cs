using UnityEngine;

public class BridgeActivator2D : MonoBehaviour
{
    public GameObject bridgesClose;
    public GameObject bridgesOpen;

    private bool isNearBridge = false;

    void Update()
    {
        if (isNearBridge && Input.GetKeyDown(KeyCode.E))
        {
            if (bridgesClose != null) bridgesClose.SetActive(true);
            if (bridgesOpen != null) bridgesOpen.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("bridgeTag"))
        {
            isNearBridge = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("bridgeTag"))
        {
            isNearBridge = false;
        }
    }
}
