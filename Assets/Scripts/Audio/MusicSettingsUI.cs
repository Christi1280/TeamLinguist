
using UnityEngine;
using UnityEngine.UI;

public class MusicSettingsUI : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;

    private void Start()
    {
        if (BackgroundMusicManager.Instance == null)
            return;

        musicSlider.minValue = 0f;
        musicSlider.maxValue = 1f;

        musicSlider.SetValueWithoutNotify(
            BackgroundMusicManager.Instance.GetVolume()
        );

        musicSlider.onValueChanged.AddListener(
            BackgroundMusicManager.Instance.SetVolume
        );
    }

    private void OnDestroy()
    {
        if (musicSlider != null &&
            BackgroundMusicManager.Instance != null)
        {
            musicSlider.onValueChanged.RemoveListener(
                BackgroundMusicManager.Instance.SetVolume
            );
        }
    }
}
