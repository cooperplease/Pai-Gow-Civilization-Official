using UnityEngine;
using Yarn.Unity;

public class InteractDialogueData : MonoBehaviour
{
    public string dialogueStartNode;
    public bool active = true;

    [YarnCommand("deactivate")]
    public void Deactivate()
    {
        active = false;
    }
}
