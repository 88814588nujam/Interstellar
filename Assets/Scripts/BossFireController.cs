using UnityEngine;

public class BossFireController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerController playerController = FindFirstObjectByType<PlayerController>();
            if (playerController != null) playerController.TakeDamage();
        }
    }
}
