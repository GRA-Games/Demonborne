using System;
using Platformer.Gameplay;
using UnityEngine;
using static Platformer.Core.Simulation;

public class HealthBat : MonoBehaviour
{
    /// <summary>
    /// Represebts the current vital statistics of some game entity.
    /// </summary>
    /// <summary>
        /// The maximum hit points for the entity.
        /// </summary>
        public int maxHP = 3;

        /// <summary>
        /// Indicates if the entity should be considered 'alive'.
        /// </summary>
        public bool IsAlive => currentHP > 0;

        public int currentHP;

        /// <summary>
        /// Increment the HP of the entity.
        /// </summary>
        public void Increment()
        {
            currentHP = Mathf.Clamp(currentHP + 1, 0, maxHP);
        }

        /// <summary>
        /// Decrement the HP of the entity. Will trigger a HealthIsZero event when
        /// current HP reaches 0.
        /// </summary>
        public void Decrement()
        {
            Debug.Log(this + " Current HP: " + currentHP);
            Debug.Log(this + "Health -1");
            currentHP = Mathf.Clamp(currentHP - 1, 0, maxHP);
            if (currentHP == 0)
            {
                
            }
        }

        /// <summary>
        /// Decrement the HP of the entitiy until HP reaches 0.
        /// </summary>


        void Awake()
        {
            currentHP = maxHP;
        }
    }
