using System.Collections.Generic;
using UnityEngine;

public class CardZone
{
    public enum ZoneType {HAND, GRAVEYARD, INPLAY}
    public ZoneType zoneType;
    public readonly CardZoneVisual visual;
    public readonly List<Card> cards = new List<Card>();

    public CardZone(ZoneType zoneType, CardZoneVisual visual = null)
    {
        this.zoneType =  zoneType;
        this.visual = visual;
    }

    public static void MoveCard(Card card, CardZone to)
    {
        card.state.zone.RemoveCard(card);
        to.AddCard(card);
    }

    public bool HasCard(Card card)
    {
        return cards.Contains(card);
    }

    public void AddCard(Card card)
    {
        card.state.ChangeZone(this);
        cards.Add(card);
    }

    public void AddCard(List<Card> card)
    {
        this.cards.AddRange(card);
        foreach (Card newCard in card)
        {
            newCard.state.ChangeZone(this);
        }
    }

    public void RemoveCard(Card card)
    {
        cards.Remove(card);
    }
}