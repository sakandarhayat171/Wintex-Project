using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip clickSound;
    public AudioClip flipSound;
    public AudioClip matchSound;
    public AudioClip mismatchSound;
    public AudioClip gameoverSound;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayClickSound()
    {
        audioSource?.PlayOneShot(clickSound);
    }

    public void PlayFlipSound()
    {
        audioSource?.PlayOneShot(flipSound);
    }

    public void PlayMatchSound()
    {
        audioSource?.PlayOneShot(matchSound);
    }

    public void PlayMismatchSound()
    {
        audioSource?.PlayOneShot(mismatchSound);
    }

    public void PlayGameOverSound()
    {
        audioSource?.PlayOneShot(gameoverSound);
    }
}