using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;          // Reference to Animator for fade in/out
    public float defaultTransitionTime = 1f;  // Default time duration for the transition
    public GameObject image;

    [SerializeField] private AudioClip selectSound;
    private AudioSource audioSource;

    void Awake()
    {
        // Ensure audio source is initialized
        image.SetActive(true);
        audioSource = GetComponent<AudioSource>();
    }

    // Method to start loading the scene by name, with an optional custom transition time

    public void LoadSceneName(string sceneName)
    {
        StartCoroutine(LoadScene(sceneName, 1));
    }

    public void LoadSceneName(string sceneName, float? customTransitionTime = null)
    {
        float transitionTimeToUse = customTransitionTime ?? defaultTransitionTime;
        StartCoroutine(LoadScene(sceneName, transitionTimeToUse));
    }

    // Quit application, typically used for a quit button
    public void OnQuitButton()
    {
        Application.Quit();
    }

    // Coroutine to handle fade-in, wait, and then load scene with an optional custom transition time
    IEnumerator LoadScene(string sceneName, float transitionTimeToUse)
    {
        if (transition != null)
        {
            // Debugging step: Check if Animator is assigned a valid controller
            if (transition.runtimeAnimatorController != null)
            {
                Debug.Log("Starting fade animation.");
                transition.SetTrigger("StartFadeIn");  // Trigger the fade animation
            }
            else
            {
                Debug.LogError("Animator does not have an Animator Controller assigned. Please assign a valid controller.");
                yield break;  // Stop the coroutine if Animator Controller is missing
            }
        }
        else
        {
            Debug.LogError("Transition Animator is not assigned in LevelLoader script.");
            yield break;  // Stop the coroutine if Animator is not assigned
        }

        // Play select sound if applicable
        if (selectSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(selectSound);
        }

        // Wait for the fade animation to finish
        yield return new WaitForSeconds(transitionTimeToUse);

        // Load the new scene
        SceneManager.LoadScene(sceneName);
    }
}
