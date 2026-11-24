using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HumanCardPlayer : CardPlayer
{
    PlayingAreaVisual visual;
    public HumanCardPlayer(CardPlayerData data, PlayingAreaVisual visual) : base(data)
    {
        this.visual = visual;
    }

    public override async Task<List<Card>> ChooseUpToXCardsFromList(int x, List<Card> cards)
    {
        return await visual.ChooseRangeOfCardsFromList(0,x, cards);
    }
}
