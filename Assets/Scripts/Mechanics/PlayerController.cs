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

        public bool isInvincible;

        public bool isFacingRight;

        private bool isAttackCoroutineRunning = false;

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

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
            
                if (!isAttacking)
                {
                    TakeDamage();
                }
            }
        }

        void PerformAttack()
        {
            if (!isAttackCoroutineRunning)
            {
                StartCoroutine(ActivateAttackZone());
            }
        }

        IEnumerator ActivateAttackZone()
        {
            isAttacking = true;
            controlEnabled = false;
            isAttackCoroutineRunning = true;

            

            // Trigger attack animation
            animator.SetTrigger("attack");

            yield return new WaitForSeconds(0.1f);

            // Play attack sound effect
            if (audioSource && attackAudio)
            {
                audioSource.PlayOneShot(attackAudio);
            }

            // Notify the AttackZone to enable its collider
            if (attackZone != null)
            {
                AttackZoneController attackZoneController = attackZone.GetComponent<AttackZoneController>();
                if (attackZoneController != null)
                {
                    attackZoneController.EnableCollider();
                    Debug.Log("AttackZone activated.");
                }
            }

            yield return new WaitForSeconds(0.3f);

            // Notify the AttackZone to disable its collider
            if (attackZone != null)
            {
                AttackZoneController attackZoneController = attackZone.GetComponent<AttackZoneController>();
                if (attackZoneController != null)
                {
                    attackZoneController.DisableCollider();
                    Debug.Log("AttackZone deactivated.");
                }
            }

            isAttacking = false;
            controlEnabled = true;
            isAttackCoroutineRunning = false;

            yield return null;
        }
        public void TakeDamage()
        {
            // Check if the player is invincible; if so, do not apply damage
            if (isInvincible)
            {
                Debug.Log("Player is invincible and cannot take damage.");
                return;
            }

            // Decrement player health through Health component
            health.Decrement();
            Debug.Log("Player takes damage, health remaining: " + health.currentHP);

            // If health is still above 0, trigger invincibility frames
            if (health.currentHP > 0)
            {
                StartCoroutine(ActivateIframes());
            }

            // Optional: Play damage sound
            if (audioSource && ouchAudio)
            {
                audioSource.PlayOneShot(ouchAudio);
            }
        }

        public Material originalMaterial;      // Store the original material
        public Material whiteMaterial;

        IEnumerator ActivateIframes()
        {
            originalMaterial = spriteRenderer.material;

            isInvincible = true;
            Debug.Log("Player is now invincible.");

            // Optional: Add visual feedback (e.g., blinking)
            for (int i = 0; i < 5; i++)
            {
                spriteRenderer.material = whiteMaterial;
                yield return new WaitForSeconds(0.1f);
                spriteRenderer.material = originalMaterial;
                yield return new WaitForSeconds(0.1f);
            }

            spriteRenderer.material = originalMaterial;

            // Iframes duration
            yield return new WaitForSeconds(1f);
            isInvincible = false;

            Debug.Log("Player invincibility ended.");
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


            if (move.x > 0.01f)
            {
                spriteRenderer.flipX = false;
                isFacingRight = true;

            }
            else if (move.x < -0.01f)
            {
                spriteRenderer.flipX = true;
                isFacingRight = false;
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