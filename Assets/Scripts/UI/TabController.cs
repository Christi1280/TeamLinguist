using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    [Header("Tabs")]
    public Image[] tabImages;

    [Header("Pages")]
    public GameObject[] pages;

    private void Start()
    {
        ActivateTab(0);
    }

    public void ActivateTab(int tabNo)
    {
        if (tabImages == null || pages == null)
        {
            return;
        }

        // Turn off all pages.
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] != null)
            {
                pages[i].SetActive(false);
            }
        }

        // Set all tabs to inactive color.
        for (int i = 0; i < tabImages.Length; i++)
        {
            if (tabImages[i] != null)
            {
                tabImages[i].color = Color.grey;
            }
        }

        // Make sure this tab actually exists.
        if (tabNo < 0 ||
            tabNo >= pages.Length ||
            tabNo >= tabImages.Length)
        {
            Debug.LogWarning(
                $"Tab {tabNo} does not exist."
            );

            return;
        }

        // Activate selected page and tab.
        if (pages[tabNo] != null)
        {
            pages[tabNo].SetActive(true);
        }

        if (tabImages[tabNo] != null)
        {
            tabImages[tabNo].color = Color.white;
        }
    }
}