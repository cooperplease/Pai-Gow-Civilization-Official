using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.UI;
using Yarn.Unity;

public class PortraitManager : DialoguePresenterBase
{
    [System.Serializable]
    public struct CharacterPortrait
    {
        public string characterName;
        public Sprite portrait;
    }

    [SerializeField] private Image portraitImage;
    [SerializeField] private List<CharacterPortrait> portraits;

    public override YarnTask RunLineAsync(LocalizedLine line, LineCancellationToken token)
    {
        string speaker = line.CharacterName;
        var match = portraits.Find(p => p.characterName == speaker);

        if (!string.IsNullOrEmpty(speaker) && match.portrait != null)
        {
            portraitImage.sprite = match.portrait;
            portraitImage.enabled = true;
        }
        else
        {
            portraitImage.enabled = false;
        }

        return YarnTask.CompletedTask;
    }
    
    public override YarnTask OnDialogueStartedAsync()
    {
        portraitImage.enabled = false;
        return YarnTask.CompletedTask;
    }

    public override YarnTask OnDialogueCompleteAsync()
    {
        portraitImage.enabled = false;
        return YarnTask.CompletedTask;
    }
}
