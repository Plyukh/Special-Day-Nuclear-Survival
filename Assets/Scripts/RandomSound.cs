using UnityEngine;

public class RandomSound : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField] private AudioClip[] audioClips;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
        audioSource.Play();
    }
}