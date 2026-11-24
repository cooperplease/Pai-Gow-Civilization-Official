using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Android;

public class CardPlayer
{
    //player stats
    public readonly String name;
    public int life { get; private set; }
    List<CardData> collection = new List<CardData>();
    
    public CardZone hand;
    public CardZone inPlay;
    public CardZone graveyard;
    public CardPlayer opponent;

    public CardPlayer(CardPlayerData data)
    {
        collection = data.collection;
        name = data.name;
    }

    public void SetLife(int value)
    {
        life = value;
    }
    
    public void ChangeLife(int delta)
    {
        life += delta;
    }

    public async Task<Queue<List<CardData>>> CreateRandomDecks(int numDecks, int cardsPerDeck)
    {
        if (numDecks * cardsPerDeck > collection.Count) Debug.Log("not enough cards to make decks!");
        
        List<CardData> shuffled = new List<CardData>(collection);
        shuffled = shuffled.OrderBy(i => Guid.NewGuid()).ToList();
        
        Queue<List<CardData>> decks = new Queue<List<CardData>>();
        for (int i = 0; i < numDecks; i++)
        {
            List<CardData> deck = new List<CardData>();
            for (int j = 0; j < cardsPerDeck; j++)
            {
                deck.Add(shuffled[i * cardsPerDeck + j]);
            }
            decks.Enqueue(deck);
        }
        await Awaitable.WaitForSecondsAsync(1f);
        return decks;
    }

    public async Task<Queue<List<CardData>>> CreateDecks(int numDecks, int cardsPerDeck)
    {
        //TODO: make real choices
        return await CreateRandomDecks(numDecks, cardsPerDeck);
    }
    
    public static void SetOpponents(CardPlayer a, CardPlayer b)
    {
        a.opponent = b;
        b.opponent = a;
    }

    public async Task<Ability> Choose(String question, List<String> options, List<Ability> results, EffectContext effectContext)
    {
        //TODO : let players actually choose
        await Task.Delay(1000);
        return results[0];
    }

    public async Task<List<Card>> ChooseCardInPlay()
    {
        //todo: real choice
        List<Card> options = inPlay.cards.Concat(opponent.inPlay.cards).ToList();
        if (options.Count<1) return new List<Card>();
        return await ChooseUpToXCardsFromList(1, options);
    }
    
    public async Task<List<Card>> ChooseCardInHand()
    {
        //todo: real choice
        List<Card> options = hand.cards;
        if (options.Count<1) return new List<Card>();
        return await ChooseUpToXCardsFromList(1, options);
    }

    public async Task<List<Card>> ChooseCardsFromList(List<Card> cards)
    {
        if(cards.Count<1) return new List<Card>();
        //todo: real choice
        return await ChooseUpToXCardsFromList(cards.Count, cards);
        return cards;
    }
    
    public virtual async Task<List<Card>> ChooseUpToXCardsFromList(int x, List<Card> cards)
    {
        if(cards.Count<1) return new List<Card>();
        //todo: real choice
        await Awaitable.WaitForSecondsAsync(1f);
        return cards.GetRange(0,x);
    }
}

public class CardPlayerData
{
    public List<CardData> collection = new List<CardData>();
    public String name;

    public CardPlayerData(List<CardData> collection, String name = "Chuddy Default Player")
    {
        this.collection = collection;
        this.name = name;
    }
}