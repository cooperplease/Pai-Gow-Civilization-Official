using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CPUCardPlayer : CardPlayer
{
    Personality personality;
    public Queue<List<CardData>> decks;
    
    public CPUCardPlayer(CPUCardPlayerData cpuData) : base(cpuData.data)
    {
        this.personality = cpuData.personality;
        this.decks = cpuData.decks;
    }

    public override async Task<Queue<List<CardData>>> CreateDecks(int numDecks, int cardsPerDeck)
    {
        await Task.Yield();
        return decks;
    }
}

public class CPUCardPlayerData
{
    public Personality personality;
    public Queue<List<CardData>> decks;
    public CardPlayerData data;

    public CPUCardPlayerData(String name, Personality personality, Queue<List<CardData>> decks)
    {
        this.personality = personality;
        this.decks = decks;
        List<CardData> collection = new List<CardData>();
        foreach (List<CardData> cards in decks)
        {
            collection.AddRange(cards);
        }
        data = new CardPlayerData(collection, name);
    }
}

public enum Personality
{
    CONCIETED,
    ANGRY,
    SAD,
    AIRHEAD,
    COMPETITIVE,
    COLLECTOR
}

