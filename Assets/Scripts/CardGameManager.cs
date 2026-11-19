using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using UnityEditor;
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
    bool roundOver = false;
    CardPlayer winner;
    CardPlayer player1;
    CardPlayer player2;
    
    public CardPlayer activePlayer;
    
    List<CardData> player1Deck;
    List<CardData> player2Deck;
    
    public TriggerEventSystem triggerEventSystem { get; private set; }
    PlayingAreaVisual playingArea;
    
    public RoundManager(CardPlayer player1, CardPlayer player2, CardPlayer activePlayer, List<CardData> player1Deck, List <CardData> player2Deck, PlayingAreaVisual playingArea = null)
    {
        this.player1 = player1;
        this.player2 = player2;
        this.activePlayer = activePlayer;
        this.player1Deck = player1Deck;
        this.player2Deck = player2Deck;
        triggerEventSystem = new TriggerEventSystem(CheckStateBasedActions);
        this.playingArea = playingArea;
    }

    public async Task<CardPlayer> StartRound()
    {
        ReadyPlayers();
        List<Card> deck1 = CreateDeck(player1Deck,player1,player1.hand);
        List<Card> deck2 = CreateDeck(player2Deck,player2,player2.hand);
        player1.hand.AddCard(deck1);
        player2.hand.AddCard(deck2);
        foreach(Card card in deck1) card.RegisterTriggers(this);
        foreach(Card card in deck2) card.RegisterTriggers(this);
       
        await triggerEventSystem.TriggerEvent(new GameEvent("RoundStart"));
        while (true)
        {
            await triggerEventSystem.TriggerEvent(new GameEvent("TurnStart"));
            await PlayStep(Untap);
            await PlayStep(Upkeep);
            
            await PlayStep(PreCombat);
            
            await PlayStep(DeclareAttackers);
            await PlayStep(DeclareBlockers); 
            await PlayStep(Damage);

            await PlayStep(PostCombat);
            
            await PlayStep(Endstep);
            
            if (roundOver) break;
            
            activePlayer = activePlayer.opponent;
        }
        foreach(Card card in deck1) card.UnregisterTriggers(this);
        foreach(Card card in deck2) card.UnregisterTriggers(this);
        return winner;
    }

    public async Task PlayStep(Func<Task> step)
    {
        if(roundOver) return;
        await step();
    }

    public async Task CheckStateBasedActions()
    {
        bool x = true;
        while (x)
        {
            x = await KillCreaturesWithLethalDamage();
        }
        CheckIfRoundOver();
    }

    public async Task<bool> KillCreaturesWithLethalDamage()
    {
        List<Card> cards = new List<Card>();
        cards.AddRange(player1.inPlay.cards);
        cards.AddRange(player2.inPlay.cards);
        List<Card> cardsToKill = new List<Card>();
        foreach (Card card in cards)
        {
            if (card.state.damage >= card.cardData.toughness && !card.cardData.keywords.Contains(Keywords.INDESTRUCTABLE))
            {
                cardsToKill.Add(card);
            }
        }
        await KillCards(cardsToKill);
        if (cardsToKill.Count == 0)
        {
            return false;
        }
        return true;
    }
    
    public void CheckIfRoundOver()
    {
        if (player1.life == 0 || player2.life == 0)
        {
            roundOver = true;
            if(player1.life > 0) winner = player1;
            else if (player2.life > 0) winner = player2;
            else winner = null;
        }
    }

    public async Task KillCard(Card card)
    {
        await KillCards(new List<Card>() { card });
    }

    public async Task KillCards(List<Card> cards)
    {
        foreach (Card card in cards)
        {
            card.state.Reset();
            await MoveCard(card,card.state.owner.graveyard);
        }
        foreach (Card card in cards)
        {
            await triggerEventSystem.TriggerEvent(new GameEvent("CardDied", card));
        }
    }

    public async Task TapCard(Card card)
    {
        card.Tap();
        await triggerEventSystem.TriggerEvent(new GameEvent("CardTap", card));
    }
    
    public async Task UntapCard(Card card)
    {
        card.Untap();
        await triggerEventSystem.TriggerEvent(new GameEvent("CardUntap", card));
    }
    
    public async Task DeclareAttacker(Card card)
    {
        card.state.DeclareAttacker();
        await triggerEventSystem.TriggerEvent(new  GameEvent("CardDeclaredAttacker", card));
    }
    
    public async Task ChangeLife(CardPlayer player, int delta)
    {
        player.ChangeLife(delta);
        if (delta < 0)
        {
            await triggerEventSystem.TriggerEvent(new GameEvent("PlayerLostLife", player, Math.Abs(delta)));
        }

        if (delta > 0)
        {
            await triggerEventSystem.TriggerEvent(new GameEvent("PlayerGainedLife", player, Math.Abs(delta)));
        }
    }

    public async Task MoveCard(Card card, CardZone to)
    {
        CardZone oldZone = card.state.zone;
        card.state.ChangeZone(to);
        if(oldZone!=null) await triggerEventSystem.TriggerEvent(new GameEvent("CardLeftZone", card, from: oldZone));
        await triggerEventSystem.TriggerEvent(new GameEvent("CardEnteredZone", card, to: to));
    }

    private async Task Untap()
    {
        await triggerEventSystem.TriggerEvent(new GameEvent("UntapStep"));
        TriggerData triggerData = new TriggerData("", new Untap(new AllMyCardsInPlay(new ActivePlayer())));
        await new Trigger(triggerData,this).Activate();
    }

    private async Task Upkeep()
    {
        await triggerEventSystem.TriggerEvent(new GameEvent("UpkeepStep"));
    }

    private async Task PreCombat()
    {
        await triggerEventSystem.TriggerEvent(new  GameEvent("PreCombatStep"));
        TriggerData triggerData = new TriggerData("", new PlayCard(new AnyCardInMyHand()));
        await new Trigger(triggerData, this).Activate();
    }

    private async Task DeclareAttackers()
    {
        await triggerEventSystem.TriggerEvent(new  GameEvent("DeclareAttackersStep"));
        TriggerData triggerData = new TriggerData("", new DeclareAttacker(new ChooseAttackers()));
        await new Trigger(triggerData,this).Activate();
    }

    private async Task DeclareBlockers()
    {
        await triggerEventSystem.TriggerEvent(new  GameEvent("DeclareBlockersStep"));
    }

    private async Task Damage()
    {
        await triggerEventSystem.TriggerEvent(new GameEvent("DamageStep"));
        //TODO: real combat
    }
    
    private async Task PostCombat()
    {
        await triggerEventSystem.TriggerEvent(new  GameEvent("PostCombatStep"));
        TriggerData triggerData = new TriggerData("", new PlayCard(new AnyCardInMyHand()));
        await new Trigger(triggerData, this).Activate();
    }

    private async Task Endstep()
    {
        await triggerEventSystem.TriggerEvent(new  GameEvent("EndStep"));
        TriggerData triggerData = new TriggerData("", new EndstepReset(new AllCardsInPlay()));
        await new Trigger(triggerData,this).Activate();
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
            player1.SetLife(5);
            
            CardZone hand2 = new CardZone(CardZone.ZoneType.HAND);
            CardZone battlefield2 = new CardZone(CardZone.ZoneType.INPLAY);
            CardZone graveyard2 = new CardZone(CardZone.ZoneType.GRAVEYARD);
            player2.hand = hand2;
            player2.inPlay = battlefield2;
            player2.graveyard = graveyard2;
            player2.SetLife(5);
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
}