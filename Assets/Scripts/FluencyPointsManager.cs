using TMPro;
using UnityEngine;

public class FluencyPointsManager : MonoBehaviour
{
    public static FluencyPointsManager Instance;

    [Header("UI")]
    public TMP_Text fluencyPointsText;
    public TMP_Text playerPageFluencyPointsText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        UpdateFluencyUI();
    }

    public void AddFluencyPoint()
    {
        if (GameProgressManager.Instance == null)
        {
            Debug.LogWarning(
                "FluencyPointsManager: GameProgressManager was not found."
            );

            return;
        }

        GameProgressManager.Instance.AddFluencyPoint();

        UpdateFluencyUI();
    }

    public void UpdateFluencyUI()
    {
        if (GameProgressManager.Instance == null)
        {
            Debug.LogWarning(
                "FluencyPointsManager: GameProgressManager was not found."
            );

            return;
        }

        string points =
            GameProgressManager.Instance.FluencyPoints.ToString();

        // Existing Fluency Points display.
        if (fluencyPointsText != null)
        {
            fluencyPointsText.text = points;
        }

        // Player tab Fluency Points display.
        if (playerPageFluencyPointsText != null)
        {
            playerPageFluencyPointsText.text = points;
        }
    }
}