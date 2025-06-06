using UnityEngine;

public class AirZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OxygenSystem.Instance.AddRefillingCharacter();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OxygenSystem.Instance.RemoveRefillingCharacter();
        }
    }
}