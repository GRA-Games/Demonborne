using System.Collections;
using System.Collections.Generic;
using Platformer.Gameplay;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Mechanics
{
    /// <summary>
    /// A simple controller for enemies. Provides movement control over a patrol path and manages health and damage interactions.
    /// </summary>
    [RequireComponent(typeof(AnimationController), typeof(Collider2D), typeof(Health))]
    public class EnemyController : MonoBehaviour
    {
        public PatrolPath path;
        public AudioClip ouch;

        internal PatrolPath.Mover mover;
        internal AnimationController control;
        internal Collider2D _collider;
        internal AudioSource _audio;
        internal Health health;
        SpriteRenderer spriteRenderer;

        public Bounds Bounds => _collider.bounds;

        void Awake()
        {
            control = GetComponent<AnimationController>();
            _collider = GetComponent<Collider2D>();
            _audio = GetComponent<AudioSource>();
            health = GetComponent<Health>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }


        void Update()
        {
            if (path != null)
            {
                if (mover == null) mover = path.CreateMover(control.maxSpeed * 0.5f);
                control.move.x = Mathf.Clamp(mover.Position.x - transform.position.x, -1, 1);
            }
        }

        public void TakeDamage()
        {
            var healthComponent = GetComponent<Health>();
            if (healthComponent != null)
            {
                healthComponent.Decrement();
                StartCoroutine(FlashWhite());
                Debug.Log("Enemy takes damage. Remaining health: " + healthComponent.currentHP);
            }
        }

        public Material originalMaterial;      // Store the original material
        public Material whiteMaterial;

        IEnumerator FlashWhite()
        {

            // Store the original color of the sprite
            originalMaterial = spriteRenderer.material;

            // Set the color to white to indicate taking damage
            spriteRenderer.material = whiteMaterial;

            // Wait for a short duration to create the flashing effect
            yield return new WaitForSeconds(0.3f);

            // Revert back to the original color
            spriteRenderer.material = originalMaterial;
        }

    }
}
