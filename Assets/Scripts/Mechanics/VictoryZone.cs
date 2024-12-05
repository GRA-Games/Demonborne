using Platformer.Gameplay;
using UnityEngine;
using static Platformer.Core.Simulation;
using System.Collections;

namespace Platformer.Mechanics
{
    /// <summary>
    /// Marks a trigger as a VictoryZone, usually used to end the current game level.
    /// </summary>
    public class VictoryZone : MonoBehaviour
    {
        public LevelLoader levelLoader;
        public string creditsSceneName = "CreditsScene";
        public float customTransitionTime = 5f;  // Custom transition time for this scene load

        void OnTriggerEnter2D(Collider2D collider)
        {
            var p = collider.gameObject.GetComponent<PlayerController>();
            if (p != null)
            {
                var ev = Schedule<PlayerEnteredVictoryZone>();
                ev.victoryZone = this;

                StartCoroutine(HandleVictory());
            }
        }

        IEnumerator HandleVictory()
        {
            yield return new WaitForSeconds(3f);

            if (levelLoader != null)
            {
                levelLoader.LoadSceneName(creditsSceneName, customTransitionTime);
            }
            else
            {
                Debug.LogError("LevelLoader reference is missing in VictoryZone script.");
            }
        }
    }
}
