using UnityEngine;

public sealed class AudioManager : MonoBehaviour
{
    public static AudioManager _Instance;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip[] audioClips;

    private void Awake()
    {
        if (_Instance != null && _Instance != this)
            Destroy(gameObject);
        else if (_Instance == null)
            _Instance = this;
    }

    public void PlayAudio(int clipToPlay)
    {
        audioSource.PlayOneShot(audioClips[clipToPlay]);
    }

    public void PlayAudio(int clipToPlay, float volume)
    {
        audioSource.PlayOneShot(audioClips[clipToPlay], volume);
    }
}
