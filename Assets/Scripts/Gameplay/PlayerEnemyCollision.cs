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
            var willHurtEnemy = player.Bounds.center.y >= enemy.Bounds.max.y;
            var isAttacking = player.animator.GetCurrentAnimatorStateInfo(0).IsName("Player-Attack");

            if (willHurtEnemy || isAttacking)
            {
                var enemyHealth = enemy.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.Decrement();
                    if (!enemyHealth.IsAlive)
                    {
                        Schedule<EnemyDeath>().enemy = enemy;
                        if (willHurtEnemy)
                        {
                            player.Bounce(2); // Bounce only if jumping
                        }
                    }
                }
                else
                {
                    Schedule<EnemyDeath>().enemy = enemy;
                    if (willHurtEnemy)
                    {
                        player.Bounce(2); // Bounce only if jumping
                    }
                }
            }
            else
            {
                Schedule<PlayerDeath>();
            }
        }
    }
}