using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class CutsceneDialogueClip : PlayableAsset
{
    public NPCDialogue dialogueData;

    [Min(0)]
    public int dialogueIndex;

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

        return playable;
    }
}