using Platformer.Mechanics;
using UnityEngine;
using UnityEngine.UI;  // Required to use the UI components

namespace Platformer.UI
{
    public class HealthUIController : MonoBehaviour
    {
        public PlayerController playerController;  // Reference to the player controller script
        public Image healthImage;                  // Reference to the UI Image displaying the hearts
        public Sprite threeHeartsSprite;           // Sprite for 3 hearts
        public Sprite twoHeartsSprite;             // Sprite for 2 hearts
        public Sprite oneHeartSprite;              // Sprite for 1 heart
        public Sprite zeroHeartsSprite;            // Sprite for 0 hearts

        void Start()
        {
            if (playerController == null)
            {
                Debug.LogError("HealthUIController: PlayerController reference is not assigned.");
            }
            if (healthImage == null)
            {
                Debug.LogError("HealthUIController: Health Image reference is not assigned.");
            }
        }

        void Update()
        {
            // Update the health UI based on the player's current health
            UpdateHealthUI();
        }

        void UpdateHealthUI()
        {
            if (playerController == null || healthImage == null) return;

            // Get the player's current health
            int currentHealth = playerController.health.currentHP;

            // Choose the appropriate sprite based on current health
            switch (currentHealth)
            {
                case 3:
                    healthImage.sprite = threeHeartsSprite;
                    break;
                case 2:
                    healthImage.sprite = twoHeartsSprite;
                    break;
                case 1:
                    healthImage.sprite = oneHeartSprite;
                    break;
                case 0:
                    healthImage.sprite = zeroHeartsSprite;
                    break;
                default:
                    Debug.LogWarning("HealthUIController: Unexpected health value: " + currentHealth);
                    break;
            }
        }
    }
}
