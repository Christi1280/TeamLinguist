using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance { get; private set; }
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage;
    public Transform choiceContainer;
    public GameObject choiceButtonPrefab;
    public AudioSource dialogueAudioSource;

    public GameObject tooltipPanel;
    public TMP_Text tooltipText;

    private NPCDialogue currentDialogueData;

    void Awake()
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

    void Update()
    {
        CheckHoverDefinition();
    }

    public void ShowDialogueUI(bool show)
    {
        dialoguePanel.SetActive(show);

        if (!show && tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }

    public void SetCurrentDialogueData(NPCDialogue dialogueData)
    {
        currentDialogueData = dialogueData;
    }

    public void SetNPCInfo(string npcName, Sprite portrait)
    {
        nameText.text = npcName;
        portraitImage.sprite = portrait;
    }

    public void SetDialogueText(string text)
    {
        dialogueText.text = text;
    }

    public void ClearChoices()
    {
        foreach (Transform child in choiceContainer) Destroy(child.gameObject);
    }

    public void CreateChoiceButton(string choiceText, UnityEngine.Events.UnityAction onClick)
    {
        GameObject choiceButton = Instantiate(choiceButtonPrefab, choiceContainer);
        choiceButton.GetComponentInChildren<TMP_Text>().text = choiceText;
        choiceButton.GetComponent<Button>().onClick.AddListener(onClick);
    }

    void CheckHoverDefinition()
    {
        if (tooltipPanel == null || tooltipText == null || dialogueText == null)
            return;

        if (!dialoguePanel.activeSelf || currentDialogueData == null)
        {
            tooltipPanel.SetActive(false);
            return;
        }

        int linkIndex = TMP_TextUtilities.FindIntersectingLink(dialogueText, Input.mousePosition, null);

        if (linkIndex == -1)
        {
            tooltipPanel.SetActive(false);
            return;
        }

        TMP_LinkInfo linkInfo = dialogueText.textInfo.linkInfo[linkIndex];
        string key = linkInfo.GetLinkID();

        foreach (HoverDefinition def in currentDialogueData.hoverDefinitions)
        {
            if (def.key == key)
            {
                tooltipText.text = def.definition;
                tooltipPanel.SetActive(true);
                tooltipPanel.transform.position = dialoguePanel.transform.position + new Vector3(0, 80f, 0);
                return;
            }
        }

        tooltipPanel.SetActive(false);
    }
}