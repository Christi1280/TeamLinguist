using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCDialogue dialogueData;
    private DialogueController dialogueUI;
    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    private void Start()
    {
        dialogueUI = DialogueController.Instance;
    }
    public bool CanInteract()
    {
        return !isDialogueActive;
    }

    public void Interact()
    {
        if (dialogueData == null || (PauseController.IsGamePaused && !isDialogueActive))
            return;

        if (isDialogueActive)
        {
            NextLine();
        }
        else
        {
            StartDialogue();
        }
    }

    void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;

        dialogueUI.SetNPCInfo(dialogueData.npcName, dialogueData.npcPortrait);
        dialogueUI.ShowDialogueUI(true);
        PauseController.SetPause(true);

        DisplayCurrentLine();
    }

    void PlayDialogueAudio()
    {
        if (dialogueData.dialogueAudioClips == null) return;
        if (dialogueIndex < 0 || dialogueIndex >= dialogueData.dialogueAudioClips.Length) return;

        AudioClip clip = dialogueData.dialogueAudioClips[dialogueIndex];
        if (clip == null) return;

        dialogueUI.dialogueAudioSource.Stop();
        dialogueUI.dialogueAudioSource.clip = clip;
        dialogueUI.dialogueAudioSource.Play();
    }

    void NextLine()
    {
        if (isTyping)
        {
            //Skip typing animation and show full line
            StopAllCoroutines();
            dialogueUI.SetDialogueText(dialogueData.dialogueLines[dialogueIndex]);
            isTyping = false;
            return;
        }
        //Clear choices
        dialogueUI.ClearChoices();

        //Check endDialogueLines
        if (dialogueData.endDialogueLines.Length > dialogueIndex && dialogueData.endDialogueLines[dialogueIndex])
        {
            EndDialogue();
            return;
        }

        //Check if choices and display
        foreach (DialogueChoice dialogueChoice in dialogueData.choices)
        {
            if (dialogueChoice.dialogueIndex == dialogueIndex)
            {
                DisplayChoices(dialogueChoice);
                return;
            }
        }


        if (++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            DisplayCurrentLine();
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueUI.SetDialogueText("");

        foreach (char letter in dialogueData.dialogueLines[dialogueIndex])
        {
            dialogueUI.SetDialogueText(dialogueUI.dialogueText.text += letter);
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;
        if (dialogueData.autoProgressLines.Length > dialogueIndex && dialogueData.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            NextLine();
        }
    }

    void DisplayChoices(DialogueChoice choice)
    {

        for (int i = 0; i < choice.choices.Length; i++)
        {
            int choiceIndex = i;
            int nextIndex = choice.nextDialogueIndex[i];
            dialogueUI.CreateChoiceButton(choice.choices[i], () => ChooseOption(choice, choiceIndex, nextIndex));
        }
    }

    void ChooseOption(DialogueChoice choice, int choiceIndex, int nextIndex)
    {
        bool isCorrect = choice.correctChoice != null &&
                         choiceIndex < choice.correctChoice.Length &&
                         choice.correctChoice[choiceIndex];

        if (isCorrect)
        {
            if (FluencyPointsManager.Instance != null)
            {
                FluencyPointsManager.Instance.AddFluencyPoint();
            }

            dialogueIndex = nextIndex;
        }
        else
        {
            dialogueIndex = nextIndex;
        }

        dialogueUI.ClearChoices();
        DisplayCurrentLine();
    }

    void DisplayCurrentLine()
    {
        StopAllCoroutines();
        PlayDialogueAudio();
        StartCoroutine(TypeLine());
    }
    public void EndDialogue()
    {
        StopAllCoroutines();
        dialogueUI.dialogueAudioSource.Stop();
        isDialogueActive = false;
        dialogueUI.SetDialogueText("");
        dialogueUI.ShowDialogueUI(false);
        PauseController.SetPause(false);
    }
}