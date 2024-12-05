using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when a Player collides with an Enemy.
    /// </summary>
    /// <typeparam name="EnemyCollision"></typeparam>
    public class PlayerEnemyCollision : Simulation.Event<PlayerEnemyCollision>
    {
        public EnemyController enemy;
        public PlayerController player;

        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {
            // Ensure this event only processes if the player is not attacking
            if (player.isAttacking)
            {
                // Player is attacking; no damage to player
                // Handle enemy damage instead
                var enemyHealth = enemy.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.Decrement();
                    if (!enemyHealth.IsAlive)
                    {
                        Schedule<EnemyDeath>().enemy = enemy;
                    }
                }
                else
                {
                    Schedule<EnemyDeath>().enemy = enemy;
                }
                return; // Stop further processing since the attack is handled
            }

            // Check if the player is actually colliding with the enemy and not the attack zone
            if (player.attackZone != null && player.attackZone.activeSelf && enemy.GetComponent<Collider2D>().IsTouching(player.attackZone.GetComponent<Collider2D>()))
            {
                // The collision is between the enemy and attack zone; do nothing further
                return;
            }

            // If the player collides with the enemy and is not attacking, the player takes damage
            Schedule<PlayerDeath>();
        }
    }
}
