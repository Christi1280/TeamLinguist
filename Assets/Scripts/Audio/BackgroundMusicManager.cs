
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager Instance { get; private set; }

    [SerializeField] private AudioClip startingMusic;

    private AudioSource audioSource;

    private const string VolumeKey = "MusicVolume";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.spatialBlend = 0f;

        SetVolume(PlayerPrefs.GetFloat(VolumeKey, 1f));
    }

    private void Start()
    {
        if (startingMusic != null)
        {
            PlayMusic(startingMusic);
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null)
            return;

        if (audioSource.clip == clip)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();

            return;
        }

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }

    public void SetVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);

        audioSource.volume = volume;

        PlayerPrefs.SetFloat(VolumeKey, volume);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return audioSource.volume;
    }
}
