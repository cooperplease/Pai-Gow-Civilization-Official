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
    private int startingLifeTotal = 5;
    List<CardData> collection = new List<CardData>();
    
    int lifeTotal;
    
    public CardZone hand;
    public CardZone inPlay;
    public CardZone graveyard;
    public CardPlayer opponent;

    public CardPlayer(CardPlayerData data)
    {
        collection = data.collection;
        name = data.name;
        startingLifeTotal = data.startingLifeTotal;
    }

    public async Task<Queue<List<CardData>>> CreateRandomDecks(int numDecks, int cardsPerDeck)
    {
        if (numDecks * cardsPerDeck > collection.Count) Debug.Log("not enough cards to make decks!");
        
        List<CardData> shuffled = new List<CardData>(collection);
        shuffled = shuffled.OrderBy(i => Guid.NewGuid()).ToList();
        
        Queue<List<CardData>> decks = new Queue<List<CardData>>();
        for (int i = 0; i < numDecks; i++)
        {
            List<CardData> deck = new List<CardData>(cardsPerDeck);
            for (int j = 0; j < cardsPerDeck; j++)
            {
                deck.Add(shuffled[i * cardsPerDeck + j]);
            }
            decks.Enqueue(deck);
        }
        await Task.Delay(1000);
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

    public void StartNewRound()
    {
        lifeTotal = startingLifeTotal;
    }

    public async Task<Ability> Choose(String question, List<String> options, List<Ability> results, EffectContext effectContext)
    {
        //TODO : let players actually choose
        await Task.Delay(1000);
        return results[0];
    }

    public async Task<Card> ChooseCardInPlay()
    {
        //todo: real choice
        List<Card> options = inPlay.cards.Concat(opponent.inPlay.cards).ToList();
        await Task.Delay(1000);
        if (options.Count<1) return null;
        return options[0];
    }
    
    public async Task<Card> ChooseCardInHand()
    {
        //todo: real choice
        List<Card> options = hand.cards;
        await Task.Delay(1000);
        if (options.Count<1) return null;
        return options[0];
    }
    
    public async Task<Card> ChooseCardFromList(List<Card> cards)
    {
        //todo: real choice
        return cards[0];
    }

    public async Task<List<Card>> ChooseCardsFromList(List<Card> cards)
    {
        //todo: real choice
        await Task.Delay(1000);
        return cards;
    }
    
    public async Task<List<Card>> ChooseXCardsFromList(int x, List<Card> cards)
    {
        //todo: real choice
        await Task.Delay(1000);
        return cards.GetRange(0,x);
    }
}

public class CardPlayerData
{
    public List<CardData> collection = new List<CardData>();
    public String name;
    public int startingLifeTotal;

    public CardPlayerData(List<CardData> collection, String name = "Chuddy Default Player",  int startingLifeTotal = 5)
    {
        this.collection = collection;
        this.name = name;
        this.startingLifeTotal = startingLifeTotal;
    }
}