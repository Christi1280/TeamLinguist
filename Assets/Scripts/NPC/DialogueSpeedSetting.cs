using TMPro;
using UnityEngine;

public class DialogueSpeedSetting : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown speedDropdown;

    private const string SpeedKey = "DialogueSpeed";

    private void OnEnable()
    {
        if (speedDropdown == null)
            return;

        int savedSpeed = PlayerPrefs.GetInt(SpeedKey, 0);

        speedDropdown.SetValueWithoutNotify(savedSpeed);
        speedDropdown.RefreshShownValue();

        speedDropdown.onValueChanged.RemoveListener(ChangeSpeed);
        speedDropdown.onValueChanged.AddListener(ChangeSpeed);
    }

    private void OnDisable()
    {
        if (speedDropdown != null)
        {
            speedDropdown.onValueChanged.RemoveListener(ChangeSpeed);
        }
    }

    private void ChangeSpeed(int index)
    {
        PlayerPrefs.SetInt(SpeedKey, index);
        PlayerPrefs.Save();

        Debug.Log("Dialogue Speed: " +
            (index == 0 ? "Normal" : "Slowed"));
    }
}