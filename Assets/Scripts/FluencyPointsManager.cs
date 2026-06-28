using TMPro;
using UnityEngine;

public class FluencyPointsManager : MonoBehaviour
{
    public static FluencyPointsManager Instance;

    public int fluencyPoints = 0;

    public TMP_Text fluencyPointsText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateFluencyUI();
    }

    public void AddFluencyPoint()
    {
        fluencyPoints++;
        UpdateFluencyUI();

        Debug.Log("Fluency Points: " + fluencyPoints);
    }

    private void UpdateFluencyUI()
    {
        if (fluencyPointsText != null)
        {
            fluencyPointsText.text = fluencyPoints.ToString();
        }
    }
}