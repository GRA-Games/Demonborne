using Platformer.Mechanics;
using UnityEngine;

public class AttackZoneController : MonoBehaviour
{

    public PlayerController player; // Reference to the player
    public Vector2 rightOffset; // Offset when player is facing right
    public Vector2 leftOffset;  // Offset when player is facing left


    private BoxCollider2D attackCollider;

    void Awake()
    {
        attackCollider = GetComponent<BoxCollider2D>();
        if (attackCollider != null)
        {
            attackCollider.enabled = false; // Ensure collider starts inactive
        }
    }

    void Update()
    {
        if (player != null)
        {
            // Update the attack zone position based on the player's position and direction
            if (player.isFacingRight)
            {
                transform.position = (Vector2)player.transform.position + rightOffset;
            }
            else
            {
                transform.position = (Vector2)player.transform.position + leftOffset;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            EnemyController enemyController = collider.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.TakeDamage(); // Deal damage to enemy
                Debug.Log("Enemy takes damage from AttackZone.");
            }
        }
    }

    public void EnableCollider()
    {
        if (attackCollider != null)
        {
            attackCollider.enabled = true;
        }
    }

    public void DisableCollider()
    {
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }
    }
}
