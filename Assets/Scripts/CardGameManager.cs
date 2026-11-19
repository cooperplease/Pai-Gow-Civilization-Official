using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using UnityEngine;

public class CardGameManager
{
    private CardPlayer player1;
    private CardPlayer player2;
    
    int player1Score = 0;
    int player2Score = 0;
    
    CardPlayer firstPlayer;
    
    
    private int totalRounds = 3;
    private int numDecks = 3;
    private int cardsPerDeck = 4;
    
    PlayingAreaVisual playingArea;

    public CardGameManager(CardPlayer player1, CardPlayer player2, PlayingAreaVisual playingArea = null)
    {
        this.player1 = player1;
        this.player2 = player2;
        CardPlayer.SetOpponents(player1, player2);
        this.playingArea = playingArea;
    }

    public async Task StartGame()
    {
        Debug.Log("Starting game");
        
        //TODO: randomly choose starting player
        firstPlayer = player1;
        Debug.Log("First player chosen");
        
        Queue<List<CardData>> player1Decks = await player1.CreateDecks(numDecks, cardsPerDeck);
        Queue<List<CardData>> player2Decks = await player2.CreateDecks(numDecks, cardsPerDeck);
        
        for (int i = 0; i < totalRounds; i++)
        {
            //TODO: choose active decks
            List<CardData> player1Deck = player1Decks.Dequeue();
            List<CardData> player2Deck = player2Decks.Dequeue();
            
            Debug.Log("Starting Round " + (i+1));
            RoundManager roundManager;
            if (playingArea != null)
            {
                roundManager = new RoundManager(player1, player2, firstPlayer, player1Deck, player2Deck, playingArea);
            }
            else
            {
                roundManager = new RoundManager(player1, player2, firstPlayer, player1Deck, player2Deck);
            }
            CardPlayer winner = await roundManager.StartRound();
            if (winner == player1)
            {
                player1Score += 1;
            }
            if (winner == player2)
            {
                player2Score += 1;
            }
            Debug.Log("Round "+(i+1)+" Winner " + winner.name);
            firstPlayer = winner.opponent;
            if (player1Score > totalRounds / 2)
            {
                break;
            }
            if (player2Score > totalRounds / 2)
            {
                break;
            }
        }
        Debug.Log("Game over");
        Debug.Log(player1.name+" Score: " + player1Score);
        Debug.Log(player2.name+" Score: " + player2Score);
    }
}

public class RoundManager
{
    CardPlayer player1;
    CardPlayer player2;
    public CardPlayer activePlayer;
    
    List<CardData> player1Deck;
    List<CardData> player2Deck;
    
    TriggerEventSystem triggerEventSystem;
    PlayingAreaVisual playingArea;
    
    public RoundManager(CardPlayer player1, CardPlayer player2, CardPlayer activePlayer, List<CardData> player1Deck, List <CardData> player2Deck, PlayingAreaVisual playingArea = null)
    {
        this.player1 = player1;
        this.player2 = player2;
        this.activePlayer = activePlayer;
        this.player1Deck = player1Deck;
        this.player2Deck = player2Deck;
        triggerEventSystem = new TriggerEventSystem();
        this.playingArea = playingArea;
    }

    public async Task<CardPlayer> StartRound()
    {
        player1.StartNewRound();
        player2.StartNewRound();
        ReadyPlayers();
        List<Card> deck1 = CreateDeck(player1Deck,player1,player1.hand);
        List<Card> deck2 = CreateDeck(player2Deck,player2,player2.hand);
        player1.hand.AddCard(deck1);
        player2.hand.AddCard(deck2);
       
        foreach(Card card in deck1) card.RegisterTriggers(triggerEventSystem);
        foreach(Card card in deck2) card.RegisterTriggers(triggerEventSystem);
       
        await triggerEventSystem.TriggerEvent("RoundStart",new EffectContext(this));
        while (true)
        {
            for (int i = 0; i < player1Deck.Count; i++)
            {
                await triggerEventSystem.TriggerEvent("TurnStart", new EffectContext(this));
                await Untap();
                await Upkeep();
                await PreCombat();
                
                await DeclareAttackers();
                await DeclareBlockers(); 
                await Damage();
                
                await Endstep();
               
                /*
                
                PostCombat();
                
                */
                activePlayer = activePlayer.opponent;
            
                await triggerEventSystem.TriggerEvent("TurnEnd");
            }
            break;
        }
        CardPlayer winner = activePlayer;
        foreach(Card card in deck1) card.UnregisterTriggers(triggerEventSystem);
        foreach(Card card in deck2) card.UnregisterTriggers(triggerEventSystem);
        return winner;
    }

    public async Task TapCard(Card card)
    {
        card.Tap();
        await triggerEventSystem.TriggerEvent("TapCard", new EffectContext(card,this));
    }
    
    public async Task UntapCard(Card card)
    {
        card.Untap();
        await triggerEventSystem.TriggerEvent("UntapCard", new EffectContext(card,this));
    }

    private async Task Untap()
    {
        await triggerEventSystem.TriggerEvent("Untap", new EffectContext(this));
        TriggerData triggerData = new TriggerData("", new Untap(new AllMyCardsInPlay()));
        await new Trigger(triggerData).Activate(new EffectContext( this));
    }

    private async Task Upkeep()
    {
        await triggerEventSystem.TriggerEvent("Upkeep", new EffectContext(this));
    }

    private async Task PreCombat()
    {
        await triggerEventSystem.TriggerEvent("Precombat", new EffectContext(this));
        TriggerData triggerData = new TriggerData("", new PlayCard(new AnyCardInMyHand()));
        await new Trigger(triggerData).Activate(new EffectContext(this));
    }

    private async Task DeclareAttackers()
    {
        await triggerEventSystem.TriggerEvent("DeclareAttackers", new EffectContext(this));
        TriggerData triggerData = new TriggerData("", new DeclareAttacker(new ChooseAttackers()));
        await new Trigger(triggerData).Activate(new EffectContext(this));
    }

    private async Task DeclareBlockers()
    {
        await triggerEventSystem.TriggerEvent("DeclareBlockers", new EffectContext(this));
    }

    private async Task Damage()
    {
        await triggerEventSystem.TriggerEvent("Damage", new EffectContext(this));
        //TODO: real combat
    }

    private async Task Endstep()
    {
        await triggerEventSystem.TriggerEvent("Endstep", new EffectContext(this));
        TriggerData triggerData = new TriggerData("", new EndstepReset(new AllCardsInPlay()));
        await new Trigger(triggerData).Activate(new EffectContext(this));
    }

    private List<Card> CreateDeck(List<CardData> cards, CardPlayer player, CardZone hand)
    {
        List<Card> deck = new List<Card>();
        foreach(CardData cardData in cards)
        {
            Card newCard = new Card(cardData, player, hand);
            deck.Add(newCard);

            if (playingArea != null)
            {
                playingArea.SpawnCard(newCard);
            }
        }
        return deck;
    }

    private void ReadyPlayers()
    {
        if (playingArea!=null)
        {
            CardZone hand1 = new CardZone(CardZone.ZoneType.HAND);
            CardZone battlefield1 = new CardZone(CardZone.ZoneType.INPLAY);
            CardZone graveyard1 = new CardZone(CardZone.ZoneType.GRAVEYARD);
            player1.hand = hand1;
            player1.inPlay = battlefield1;
            player1.graveyard = graveyard1;
            
            CardZone hand2 = new CardZone(CardZone.ZoneType.HAND);
            CardZone battlefield2 = new CardZone(CardZone.ZoneType.INPLAY);
            CardZone graveyard2 = new CardZone(CardZone.ZoneType.GRAVEYARD);
            player2.hand = hand2;
            player2.inPlay = battlefield2;
            player2.graveyard = graveyard2;
        }
        else
        {
            player1.hand = new CardZone(CardZone.ZoneType.HAND);
            player1.graveyard = new CardZone(CardZone.ZoneType.GRAVEYARD);
            player1.inPlay = new CardZone(CardZone.ZoneType.INPLAY);
            player2.hand = new CardZone(CardZone.ZoneType.HAND);
            player2.graveyard = new CardZone(CardZone.ZoneType.GRAVEYARD);
            player2.inPlay = new CardZone(CardZone.ZoneType.INPLAY);
        }
    }

    public async Task DeclareAttacker(Card card)
    {
        card.state.DeclareAttacker();
        await triggerEventSystem.TriggerEvent("DeclareCardAttacker", new EffectContext(card, this));
    }
}