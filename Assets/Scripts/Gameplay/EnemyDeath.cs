using Platformer.Core;
using Platformer.Mechanics;
using UnityEngine;  // Ensure to include UnityEngine to use Debug.Log and other Unity utilities

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when the health component on an enemy has a hitpoint value of 0.
    /// </summary>
    /// <typeparam name="EnemyDeath"></typeparam>
    public class EnemyDeath : Simulation.Event<EnemyDeath>
    {
        public EnemyController enemy;

        public override void Execute()
        {
            if (enemy == null)
            {
                Debug.LogError("EnemyDeath.Execute: 'enemy' reference is null. Ensure the enemy is properly assigned before scheduling the death event.");
                return;
            }

            if (enemy._collider != null)
            {
                enemy._collider.enabled = false;
            }
            if (enemy.control != null)
            {
                enemy.control.enabled = false;
            }
            if (enemy._audio != null && enemy.ouch != null)
            {
                enemy._audio.PlayOneShot(enemy.ouch);
            }
        }
    }
}
