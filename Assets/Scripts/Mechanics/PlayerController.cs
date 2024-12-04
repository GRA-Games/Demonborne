using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;

namespace Platformer.Mechanics
{
    /// <summary>
    /// This is the main class used to implement control of the player.
    /// It is a superset of the AnimationController class, but is inlined to allow for any kind of customisation.
    /// </summary>
    public class PlayerController : KinematicObject
    {
        public AudioClip jumpAudio;
        public AudioClip respawnAudio;
        public AudioClip ouchAudio;

        /// <summary>
        /// Max horizontal speed of the player.
        /// </summary>
        public float maxSpeed = 7;
        /// <summary>
        /// Initial jump velocity at the start of a jump.
        /// </summary>
        public float jumpTakeOffSpeed = 7;

        public JumpState jumpState = JumpState.Grounded;
        private bool stopJump;
        public Collider2D collider2d;
        public AudioSource audioSource;
        public Health health;
        public bool controlEnabled = true;

        bool jump;
        Vector2 move;
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public Bounds Bounds => collider2d.bounds;

        [SerializeField] private float attackRange = 1.0f; // Attack range

        void Awake()
        {
            health = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        public GameObject attackHitbox; // Reference to the hitbox GameObject

        protected override void Update()
        {
            if (controlEnabled)
            {
                move.x = Input.GetAxis("Horizontal");

                if (jumpState == JumpState.Grounded && Input.GetButtonDown("Jump"))
                    jumpState = JumpState.PrepareToJump;
                else if (Input.GetButtonUp("Jump"))
                {
                    stopJump = true;
                    Schedule<PlayerStopJump>().player = this;
                }

                // Trigger attack animation on input
                if (Input.GetButtonDown("Fire1"))
                {
                    animator.SetTrigger("attack");
                    PerformAttack(); // Handle attack logic
                }
            }
            else
            {
                move.x = 0;
            }
            UpdateJumpState();
            base.Update();
        }


        void PerformAttack()
        {
            // Determine the player's facing direction
            float facingDirection = spriteRenderer.flipX ? -1 : 1;
            Vector3 attackPosition = transform.position + new Vector3(facingDirection * attackRange * 0.5f, 0, 0);

            LayerMask enemyLayer = LayerMask.GetMask("Enemy");
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPosition, attackRange, enemyLayer);

            Debug.Log($"Number of enemies detected: {hitEnemies.Length}");

            foreach (Collider2D collider in hitEnemies)
            {
                // Try to get the EnemyController directly from the detected collider
                var enemyController = collider.GetComponentInParent<EnemyController>();
                if (enemyController != null)
                {
                    Debug.Log($"EnemyController found: {enemyController.name}");

                    // Access the Health component through the EnemyController
                    var enemyHealth = enemyController.GetComponent<Health>();
                    if (enemyHealth != null)
                    {
                        Debug.Log($"Health found on enemy {enemyController.name}. Decreasing HP.");
                        enemyHealth.Decrement();

                        // Optional: Schedule the enemy's death if health reaches 0
                        if (!enemyHealth.IsAlive)
                        {
                            Debug.Log($"Enemy {enemyController.name} has died.");
                            Schedule<EnemyDeath>().enemy = enemyController;
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Enemy {enemyController.name} does not have a Health component!");
                    }
                }
                else
                {
                    Debug.LogWarning($"No EnemyController found for {collider.name}!");
                }
            }
        }

        // Optional: Visualize the attack range
        void OnDrawGizmosSelected()
        {
            float facingDirection = spriteRenderer != null && spriteRenderer.flipX ? -1 : 1;
            Vector3 attackPosition = transform.position + new Vector3(facingDirection * attackRange * 0.5f, 0, 0);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPosition, attackRange);
        }

        void UpdateJumpState()
        {

            jump = false;
            switch (jumpState)
            {
                case JumpState.PrepareToJump:
                    jumpState = JumpState.Jumping;
                    jump = true;
                    stopJump = false;
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded)
                    {
                        Schedule<PlayerJumped>().player = this;
                        jumpState = JumpState.InFlight;
                    }
                    break;
                case JumpState.InFlight:
                    // Set `isFalling` to true if the player is in flight and moving downward
                    animator.SetBool("isFalling", velocity.y < 0);
                    if (IsGrounded)
                    {
                        Schedule<PlayerLanded>().player = this;
                        jumpState = JumpState.Landed;
                        // Reset `isFalling` when the player lands
                        animator.SetBool("isFalling", false);
                    }
                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    break;
            }
        }

        protected override void ComputeVelocity()
        {
            if (jump && IsGrounded)
            {
                velocity.y = jumpTakeOffSpeed * model.jumpModifier;
                jump = false;
            }
            else if (stopJump)
            {
                stopJump = false;
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * model.jumpDeceleration;
                }
            }

            if (move.x > 0.01f)
                spriteRenderer.flipX = false;
            else if (move.x < -0.01f)
                spriteRenderer.flipX = true;

            animator.SetBool("grounded", IsGrounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

            targetVelocity = move * maxSpeed;
        }

        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            Landed
        }
    }
}