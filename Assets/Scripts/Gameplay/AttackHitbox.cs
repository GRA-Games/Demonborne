using UnityEngine;

using Platformer.Mechanics;

public class AttackHitbox : MonoBehaviour
{
    public int damage = 10; // Damage dealt to enemies
    private bool isActive = false; // Whether the hitbox is active

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActive)
        {
            // Check if the collider belongs to an enemy
            var enemy = collision.GetComponent<EnemyController>();
            if (enemy != null)
            {
                var health = enemy.GetComponent<Health>();
                if (health != null)
                {
                    health.Decrement(); // Apply damage
                }
            }
        }
    }

    public void ActivateHitbox()
    {
        isActive = true;
    }

    public void DeactivateHitbox()
    {
        isActive = false;
    }
}