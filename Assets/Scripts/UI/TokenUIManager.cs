using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Platformer.UI
{
    public class TokenUIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI tokenText;
        private int tokenCount = 0;

        public Animator animator;


        // Singleton pattern for easy access
        public static TokenUIManager Instance { get; private set; }

        private void Awake()
        {
            // Ensure only one instance exists
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void CollectToken()
        {
            tokenCount++;
            UpdateTokenText();
        }

        private void UpdateTokenText()
        {
            if (tokenText != null)
            {
                tokenText.text = $"x{tokenCount}";
                animator.SetTrigger("TokenGained");

            }
        }
    }
}