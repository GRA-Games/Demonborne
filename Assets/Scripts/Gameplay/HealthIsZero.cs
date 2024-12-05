using Platformer.Core;
using Platformer.Mechanics;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when the health reaches 0. This usually results in either a PlayerDeath or EnemyDeath event.
    /// </summary>
    /// <typeparam name="HealthIsZero"></typeparam>
    public class HealthIsZero : Simulation.Event<HealthIsZero>
    {
        public Health health;

        public override void Execute()
        {
            // Log that the HealthIsZero event has started executing
            Debug.Log("HealthIsZero.Execute: Executing health zero logic for GameObject: " + health.gameObject.name);

            // Check if the health component belongs to a player
            if (health.gameObject.CompareTag("Player"))
            {
                Debug.Log("HealthIsZero.Execute: Player health reached zero. Scheduling PlayerDeath event for: " + health.gameObject.name);
                Schedule<PlayerDeath>();
            }
            // Check if the health component belongs to an enemy
            else if (health.gameObject.CompareTag("Enemy"))
            {
                Debug.Log("HealthIsZero.Execute: Enemy health reached zero. Scheduling EnemyDeath event for: " + health.gameObject.name);

                var enemyDeathEvent = Schedule<EnemyDeath>();
                var enemyController = health.gameObject.GetComponent<EnemyController>();
                if (enemyController != null)
                {
                    enemyDeathEvent.enemy = enemyController;
                    Debug.Log("HealthIsZero.Execute: Successfully assigned EnemyController to EnemyDeath event for: " + health.gameObject.name);
                }
                else
                {
                    Debug.LogError("HealthIsZero.Execute: Failed to assign EnemyController to EnemyDeath event. EnemyController is missing from GameObject: " + health.gameObject.name);
                }
            }
            else
            {
                Debug.LogWarning("HealthIsZero.Execute: Unrecognized tag on GameObject: " + health.gameObject.name + ". Expected 'Player' or 'Enemy'.");
            }
        }
    }
}
