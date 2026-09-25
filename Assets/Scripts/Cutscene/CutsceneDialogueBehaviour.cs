using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// Controls dialogue playback for an individual dialogue clip on a Timeline.
/// Displays a selected dialogue line when the clip begins and can optionally
/// close the dialogue UI when the clip ends.
/// </summary>
public class CutsceneDialogueBehaviour : PlayableBehaviour
{
    // Dialogue asset and the specific line this clip should display.
    public NPCDialogue dialogueData;
    public int dialogueIndex;

    // If enabled, the dialogue UI closes when this clip finishes.
    public bool closeDialogue;

    // Checks if dialogue was already triggered
    private bool hasStarted;

    // If enabled, applies the phone audio effect to this dialogue.
    public bool usePhoneEffect;

    public override void OnBehaviourPlay(
        Playable playable,
        FrameData info)
    {
        if (hasStarted)
        {
            return;
        }

        hasStarted = true;

        if (DialogueController.Instance == null)
        {
            return;
        }

        if (dialogueData == null)
        {
            return;
        }

        DialogueController.Instance.ShowCutsceneDialogue(
            dialogueData,
            dialogueIndex,
            usePhoneEffect
        );
    }

    public override void OnBehaviourPause(
        Playable playable,
        FrameData info)
    {
        // Close the dialogue UI at the end of this clip if requested.
        if (closeDialogue && DialogueController.Instance != null)
        {
            DialogueController.Instance.HideCutsceneDialogue();
        }

        // Allows the dialogue to trigger again if the Timeline is replayed.
        hasStarted = false;
    }
}