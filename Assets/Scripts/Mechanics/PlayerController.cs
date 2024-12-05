using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;
using UnityEngine.WSA;

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
        public AudioClip attackAudio;

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
        /*internal new*/ public Collider2D collider2d;
        /*internal new*/ public AudioSource audioSource;
        public Health health;
        public bool controlEnabled = true;

        public GameObject attackZone;

        public bool isAttacking;

        bool jump;
        Vector2 move;
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public Bounds Bounds => collider2d.bounds;

        void Awake()
        {
            health = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

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

                if (Input.GetButtonDown("Fire1")) // Assuming "Fire1" is mapped to the attack key
                {
                    PerformAttack();
                }
            }
            else
            {
                move.x = 0;
            }
            UpdateJumpState();
            base.Update();
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            // Ensure this script is attached to the `AttackZone`
            if (collision.gameObject.CompareTag("Enemy"))
            {
                var enemy = collision.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    enemy.TakeDamage(); // Trigger damage on the enemy
                }
            }
        }

        void PerformAttack()
        {
            // Trigger attack animation
            animator.SetTrigger("attack");

            StartCoroutine(ActivateAttackZone());

            // Play audio (optional)
            if (audioSource && attackAudio)
            {
                audioSource.PlayOneShot(attackAudio);
            }

            // Handle attack logic (e.g., Raycast or collider interaction)
            Vector2 attackPosition = transform.position + (spriteRenderer.flipX ? Vector3.left : Vector3.right);
            RaycastHit2D hit = Physics2D.Raycast(attackPosition, spriteRenderer.flipX ? Vector2.left : Vector2.right, 1f);

            if (hit.collider != null)
            {
                var enemy = hit.collider.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    enemy.TakeDamage();
                }
            }
        }

        IEnumerator ActivateAttackZone()
        {
            yield return new WaitForSeconds(0.3f);
            // Activate the attack collider for a short duration
            isAttacking = true;
            attackZone.SetActive(true);

            // TODO Wait for the duration of the attack animation (adjust the timing as needed)
            yield return new WaitForSeconds(0.3f);

            // Disable the attack collider after the attack
            attackZone.SetActive(false);
            isAttacking = false;

            yield return new WaitForSeconds(0.1f);
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
                    if (IsGrounded)
                    {
                        Schedule<PlayerLanded>().player = this;
                        jumpState = JumpState.Landed;
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

            // Access the BoxCollider2D component of attackZone
            BoxCollider2D attackZoneCollider = attackZone.GetComponent<BoxCollider2D>();

            if (move.x > 0.01f)
            {
                spriteRenderer.flipX = false;

                // Set the attackZone offset to -11.6 when moving right
                if (attackZoneCollider != null)
                {
                    attackZoneCollider.offset = new Vector2(-11.6f, attackZoneCollider.offset.y);
                }
            }
            else if (move.x < -0.01f)
            {
                spriteRenderer.flipX = true;

                // Set the attackZone offset to -13.7 when moving left
                if (attackZoneCollider != null)
                {
                    attackZoneCollider.offset = new Vector2(-13.88f, attackZoneCollider.offset.y);
                }
            }

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