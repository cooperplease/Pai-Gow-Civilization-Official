using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using UnityEngine;

public class CardSystemTest : MonoBehaviour
{
    public PlayingAreaVisual playingArea;
    public CardGameManager cardGameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TriggerData UpkeepGain3life = new TriggerData("UpkeepStep",
            new Conditional(new InZone(new ThisCard(), CardZone.ZoneType.INPLAY),new Conditional(new OwnerIsActivePlayer(new ThisCard()),new ChangeLife(new Owner(), 3))));
        CardData chuddyCard = new CardData("chuddy","this card is a basic 1/1 chud nvm I have HASTE!","oops, I am a chud - chud", 1,1,null,new List<TriggerData>(){UpkeepGain3life},new List<Keywords>
            {
                Keywords.HASTE, Keywords.TRAMPLE
            });
        List<CardData> collection = new List<CardData>();
        for (int i = 0; i < 12; i++)
        {
            collection.Add(chuddyCard);
        }
        CardPlayerData playerData = new CardPlayerData(collection, "Human");
        CardPlayerData playerData2 = new CardPlayerData(collection, "chuddy");
        CardPlayer player1 = new HumanCardPlayer(playerData,playingArea);
        CardPlayer player2 = new CardPlayer(playerData2);
        cardGameManager = new CardGameManager(player1, player2,playingArea);
        StartTest();

    }
    
    async Task StartTest()
    {
        try
        {
            await cardGameManager.StartGame();
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}