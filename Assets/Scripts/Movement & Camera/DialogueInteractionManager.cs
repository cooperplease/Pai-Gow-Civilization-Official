using UnityEngine;
using UnityEngine.InputSystem;
using Yarn;

public class DialogueInteractionManager : MonoBehaviour
{
    [SerializeField] private InputActionReference interactAction;
    //[SerializeField] private string startingYarnNode = "AngryChuddyStart";
    private bool canMove = true;

    private void OnEnable()
    {
        //DialogueEvents
        DialogueEvents.OnDialogueStart += LockMovement;
        DialogueEvents.OnDialogueEnd += UnlockMovement;

        //InputEvents
        interactAction.action.Enable();
        interactAction.action.performed += OnInteract;
    }

    private void OnDisable()
    {
        //DialogueEvents
        DialogueEvents.OnDialogueStart -= LockMovement;
        DialogueEvents.OnDialogueEnd -= UnlockMovement;

        //InputEvents
        interactAction.action.performed -= OnInteract;
        interactAction.action.Disable();
    }

    private void LockMovement() => canMove = false;
    private void UnlockMovement() => canMove = true;

    private void OnInteract(InputAction.CallbackContext context)
    {
        // if (!DialogueEvents.IsDialogueActive) DialogueEvents.RequestDialogue(startingYarnNode);
    }

    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.TryGetComponent<InteractDialogueData>(out InteractDialogueData data))
        {
            if (!DialogueEvents.IsDialogueActive) DialogueEvents.RequestDialogue(data.dialogueStartNode);
        }
    }

}
