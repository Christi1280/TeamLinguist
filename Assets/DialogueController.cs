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
    public AudioSource phoneDialogueAudioSource;

    public GameObject tooltipPanel;
    public TMP_Text tooltipText;

    private NPCDialogue currentDialogueData;
    private NPC currentNPC;


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

    public void SetCurrentNPC(NPC npc)
    {
        currentNPC = npc;
    }

    public NPC GetCurrentNPC()
    {
        return currentNPC;
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
        foreach (Transform child in choiceContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public void CreateChoiceButton(
        string choiceText,
        UnityEngine.Events.UnityAction onClick)
    {
        GameObject choiceButton =
            Instantiate(choiceButtonPrefab, choiceContainer);

        choiceButton.GetComponentInChildren<TMP_Text>().text =
            choiceText;

        choiceButton.GetComponent<Button>()
            .onClick.AddListener(onClick);
    }

    // This should be called by the X / Close button.
    public void CloseDialogue()
    {
        if (currentNPC != null)
        {
            currentNPC.CancelDialogue();
        }
        else
        {
            ClearChoices();
            SetDialogueText("");
            ShowDialogueUI(false);
            PauseController.SetPause(false);
        }
    }

    public void ClearCurrentNPC()
    {
        currentNPC = null;
        currentDialogueData = null;
    }

    void CheckHoverDefinition()
    {
        if (tooltipPanel == null ||
            tooltipText == null ||
            dialogueText == null)
        {
            return;
        }

        if (!dialoguePanel.activeSelf ||
            currentDialogueData == null)
        {
            tooltipPanel.SetActive(false);
            return;
        }

        int linkIndex =
            TMP_TextUtilities.FindIntersectingLink(
                dialogueText,
                Input.mousePosition,
                null
            );

        if (linkIndex == -1)
        {
            tooltipPanel.SetActive(false);
            return;
        }

        TMP_LinkInfo linkInfo =
            dialogueText.textInfo.linkInfo[linkIndex];

        string key = linkInfo.GetLinkID();

        if (currentDialogueData.hoverDefinitions == null)
        {
            tooltipPanel.SetActive(false);
            return;
        }

        foreach (HoverDefinition def
                 in currentDialogueData.hoverDefinitions)
        {
            if (def.key == key)
            {
                tooltipText.text = def.definition;
                tooltipPanel.SetActive(true);

                tooltipPanel.transform.position =
                    dialoguePanel.transform.position +
                    new Vector3(0, 80f, 0);

                return;
            }
        }

        tooltipPanel.SetActive(false);
    }

    public void ShowCutsceneDialogue(
    NPCDialogue dialogueData,
    int dialogueIndex,
    bool usePhoneEffect = false)
    {
        Debug.Log($"CUTSCENE AUDIO: {dialogueData.npcName} | Phone Effect = {usePhoneEffect}");
        if (dialogueData == null ||
            dialogueData.dialogueLines == null ||
            dialogueIndex < 0 ||
            dialogueIndex >= dialogueData.dialogueLines.Length)
        {
            return;
        }

        // Cutscene dialogue does not belong to an interactable NPC.
        currentNPC = null;
        currentDialogueData = dialogueData;

        // Set speaker information.
        SetNPCInfo(
            dialogueData.npcName,
            dialogueData.npcPortrait
        );

        // Display the selected dialogue line.
        SetDialogueText(
            dialogueData.dialogueLines[dialogueIndex]
        );

        ShowDialogueUI(true);

        AudioSource audioSourceToUse = dialogueAudioSource;

        if (usePhoneEffect && phoneDialogueAudioSource != null)
        {
            audioSourceToUse = phoneDialogueAudioSource;
        }

        // Play the audio associated with this line.
        if (audioSourceToUse != null &&
            dialogueData.dialogueAudioClips != null &&
            dialogueIndex < dialogueData.dialogueAudioClips.Length)
        {
            AudioClip clip =
                dialogueData.dialogueAudioClips[dialogueIndex];

            if (clip != null)
            {
                if (dialogueAudioSource != null)
                {
                    dialogueAudioSource.Stop();
                }

                if (phoneDialogueAudioSource != null)
                {
                    phoneDialogueAudioSource.Stop();
                }

                audioSourceToUse.clip = clip;
                audioSourceToUse.Play();
            }
        }
    }

    public void HideCutsceneDialogue()
    {
        if (dialogueAudioSource != null)
        {
            dialogueAudioSource.Stop();
        }

        ClearChoices();
        SetDialogueText("");
        ShowDialogueUI(false);

        currentDialogueData = null;
        currentNPC = null;
    }
}