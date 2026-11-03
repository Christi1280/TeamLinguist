
using UnityEngine;
using UnityEngine.UI;

public class SoundEffectManager : MonoBehaviour
{
    public static SoundEffectManager Instance { get; private set; }

    private AudioSource audioSource;
    private AudioSource randomPitchAudioSource;
    private SoundEffectLibrary soundEffectLibrary;

    [SerializeField] private Slider sfxSlider;

    private const string VolumeKey = "SFXVolume";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Keep this manager alive between scenes.
        DontDestroyOnLoad(gameObject);

        AudioSource[] audioSources = GetComponents<AudioSource>();

        if (audioSources.Length < 2)
        {
            Debug.LogError(
                "SoundEffectManager requires two AudioSource components."
            );
            return;
        }

        audioSource = audioSources[0];
        randomPitchAudioSource = audioSources[1];

        soundEffectLibrary = GetComponent<SoundEffectLibrary>();

        // Restore the player's saved sound effect volume.
        SetVolume(PlayerPrefs.GetFloat(VolumeKey, 1f));
    }

    private void Start()
    {
        if (sfxSlider != null)
        {
            sfxSlider.SetValueWithoutNotify(
                PlayerPrefs.GetFloat(VolumeKey, 1f)
            );

            sfxSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    public static void Play(string soundName, bool randomPitch = false)
    {
        if (Instance == null)
        {
            Debug.LogWarning(
                $"Cannot play '{soundName}': SoundEffectManager is missing."
            );
            return;
        }

        if (Instance.soundEffectLibrary == null)
            return;

        AudioClip clip =
            Instance.soundEffectLibrary.GetRandomClip(soundName);

        if (clip == null)
        {
            Debug.LogWarning(
                $"Sound effect '{soundName}' was not found."
            );
            return;
        }

        if (randomPitch)
        {
            if (Instance.randomPitchAudioSource == null)
                return;

            Instance.randomPitchAudioSource.pitch =
                Random.Range(0.9f, 1.1f);

            Instance.randomPitchAudioSource.PlayOneShot(clip);
        }
        else
        {
            if (Instance.audioSource == null)
                return;

            Instance.audioSource.PlayOneShot(clip);
        }
    }

    public static void SetVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);

        if (Instance == null)
            return;

        if (Instance.audioSource != null)
            Instance.audioSource.volume = volume;

        if (Instance.randomPitchAudioSource != null)
            Instance.randomPitchAudioSource.volume = volume;

        PlayerPrefs.SetFloat(VolumeKey, volume);
        PlayerPrefs.Save();
    }
}
