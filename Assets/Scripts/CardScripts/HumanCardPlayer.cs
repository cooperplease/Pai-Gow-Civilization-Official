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
        if(cards.Count<1 || x<1) return new List<Card>();
        return await visual.ChooseRangeOfCardsFromList(0,x, cards);
    }

    public override async Task<Card> HighlightFromList(List<Card> cards)
    {
        if (cards.Count<1) return null;
        return await visual.HighlightCardFromList(cards);
    }

    public override async Task<Dictionary<Card, Card>> AssignBlockers(List<Card> attackers, List<Card> defenders)
    {
        if (attackers.Count<1 || defenders.Count<1) return new Dictionary<Card, Card>();
        Dictionary<Card, Card> Blocks = new Dictionary<Card, Card>();
        while (true)
        {
            Card defender = await HighlightFromList(defenders);
            if (defender == null) break;
            
            List<Card> EligibleAttackers = new List<Card>();
            foreach (Card attacker in attackers)
            {
                if(!(attacker.cardData.keywords.Contains(Keywords.TUNNEL)&&!defender.cardData.keywords.Contains(Keywords.TUNNEL))) EligibleAttackers.Add(attacker);
            }

            Card blockee = await HighlightFromList(EligibleAttackers);
            if (blockee == null) continue;
            if (Blocks.ContainsKey(defender))
            {
                if (Blocks[defender] != blockee) Blocks[defender] = blockee;
                else Blocks.Remove(defender);
            }
            else
            {
                Blocks.Add(defender, blockee);
            }
        }
        return Blocks;
    }
}
