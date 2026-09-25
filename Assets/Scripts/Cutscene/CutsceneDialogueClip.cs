using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// Stores the settings for an individual dialogue clip on a Timeline
/// and passes those settings to CutsceneDialogueBehaviour at runtime.
/// </summary>
[System.Serializable]
public class CutsceneDialogueClip : PlayableAsset
{
    public NPCDialogue dialogueData;

    [Min(0)]
    public int dialogueIndex;

    public bool closeDialogue;

    public bool usePhoneEffect;

    ///Creates the runtime behaviour for this Timeline clip and gives 
    ///it the settings configured in the Inspector.
    public override Playable CreatePlayable(
        PlayableGraph graph,
        GameObject owner)
    {
        ScriptPlayable<CutsceneDialogueBehaviour> playable =
            ScriptPlayable<CutsceneDialogueBehaviour>.Create(graph);

        CutsceneDialogueBehaviour behaviour =
            playable.GetBehaviour();

        behaviour.dialogueData = dialogueData;
        behaviour.dialogueIndex = dialogueIndex;
        behaviour.closeDialogue = closeDialogue;
        behaviour.usePhoneEffect = usePhoneEffect;

        return playable;
    }
}