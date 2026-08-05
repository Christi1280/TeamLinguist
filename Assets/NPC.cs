using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCDialogue dialogueData;

    [Header("NPC Events")]
    public UnityEvent onDialogueEnded;

    private DialogueController dialogueUI;
    private int dialogueIndex;
    private bool isTyping;
    private bool isDialogueActive;

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
        if (dialogueData == null ||
            (PauseController.IsGamePaused && !isDialogueActive))
        {
            return;
        }

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

        // Tell the DialogueController which NPC owns this conversation.
        dialogueUI.SetCurrentNPC(this);

        dialogueUI.SetNPCInfo(
            dialogueData.npcName,
            dialogueData.npcPortrait
        );

        dialogueUI.SetCurrentDialogueData(dialogueData);
        dialogueUI.ShowDialogueUI(true);

        PauseController.SetPause(true);

        DisplayCurrentLine();
    }

    void PlayDialogueAudio()
    {
        if (dialogueData.dialogueAudioClips == null)
        {
            return;
        }

        if (dialogueIndex < 0 ||
            dialogueIndex >= dialogueData.dialogueAudioClips.Length)
        {
            return;
        }

        AudioClip clip =
            dialogueData.dialogueAudioClips[dialogueIndex];

        if (clip == null)
        {
            return;
        }

        dialogueUI.dialogueAudioSource.Stop();
        dialogueUI.dialogueAudioSource.clip = clip;
        dialogueUI.dialogueAudioSource.Play();
    }

    void NextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();

            dialogueUI.SetDialogueText(
                dialogueData.dialogueLines[dialogueIndex]
            );

            isTyping = false;
            return;
        }

        dialogueUI.ClearChoices();

        if (dialogueData.endDialogueLines.Length > dialogueIndex &&
            dialogueData.endDialogueLines[dialogueIndex])
        {
            EndDialogue();
            return;
        }

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

        string line = dialogueData.dialogueLines[dialogueIndex];
        string displayedText = "";

        for (int i = 0; i < line.Length; i++)
        {
            if (line[i] == '<')
            {
                int closingIndex = line.IndexOf('>', i);

                if (closingIndex != -1)
                {
                    displayedText += line.Substring(
                        i,
                        closingIndex - i + 1
                    );

                    i = closingIndex;

                    dialogueUI.SetDialogueText(displayedText);

                    continue;
                }
            }

            displayedText += line[i];

            dialogueUI.SetDialogueText(displayedText);

            yield return new WaitForSeconds(
                dialogueData.typingSpeed
            );
        }

        isTyping = false;

        if (dialogueData.autoProgressLines.Length > dialogueIndex &&
            dialogueData.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(
                dialogueData.autoProgressDelay
            );

            NextLine();
        }
    }

    void DisplayChoices(DialogueChoice choice)
    {
        for (int i = 0; i < choice.choices.Length; i++)
        {
            int choiceIndex = i;
            int nextIndex = choice.nextDialogueIndex[i];

            dialogueUI.CreateChoiceButton(
                choice.choices[i],
                () => ChooseOption(
                    choice,
                    choiceIndex,
                    nextIndex
                )
            );
        }
    }

    void ChooseOption(
        DialogueChoice choice,
        int choiceIndex,
        int nextIndex)
    {
        bool isCorrect =
            choice.correctChoice != null &&
            choiceIndex < choice.correctChoice.Length &&
            choice.correctChoice[choiceIndex];

        if (isCorrect)
        {
            if (FluencyPointsManager.Instance != null)
            {
                FluencyPointsManager.Instance.AddFluencyPoint();
            }
        }

        dialogueIndex = nextIndex;

        dialogueUI.ClearChoices();

        DisplayCurrentLine();
    }

    void DisplayCurrentLine()
    {
        StopAllCoroutines();

        PlayDialogueAudio();

        StartCoroutine(TypeLine());
    }

    // Normal completion of the conversation.
    public void EndDialogue()
    {
        StopAllCoroutines();

        if (dialogueUI.dialogueAudioSource != null)
        {
            dialogueUI.dialogueAudioSource.Stop();
        }

        isTyping = false;
        isDialogueActive = false;

        dialogueUI.ClearChoices();
        dialogueUI.SetDialogueText("");
        dialogueUI.ShowDialogueUI(false);

        PauseController.SetPause(false);

        // Clear active dialogue ownership.
        dialogueUI.ClearCurrentNPC();

        // Only fire this when the player actually completes the dialogue.
        onDialogueEnded?.Invoke();
    }

    // Used when the player manually closes the dialogue early.
    public void CancelDialogue()
    {
        StopAllCoroutines();

        if (dialogueUI.dialogueAudioSource != null)
        {
            dialogueUI.dialogueAudioSource.Stop();
        }

        isTyping = false;
        isDialogueActive = false;

        // Reset so the conversation starts from the beginning next time.
        dialogueIndex = 0;

        dialogueUI.ClearChoices();
        dialogueUI.SetDialogueText("");
        dialogueUI.ShowDialogueUI(false);

        PauseController.SetPause(false);

        dialogueUI.ClearCurrentNPC();

        // Do not call onDialogueEnded here.
    }
}