using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TriggerEventSystem
{
    Dictionary<String, List<Trigger>> triggers;
    private Func<Task> StateBasedActions;
    
    public TriggerEventSystem(Func<Task> stateBasedActions)
    {
        StateBasedActions = stateBasedActions;
        triggers = new Dictionary<string, List<Trigger>>();
    }

    public void AddTrigger(String eventName, Trigger trigger)
    {
        if (!triggers.ContainsKey(eventName))
        {
            triggers.Add(eventName, new List<Trigger>());
        }
        triggers[eventName].Add(trigger);
    }

    public void RemoveTrigger(String eventName, Trigger trigger)
    {
        if (triggers.ContainsKey(eventName))
        {
            triggers[eventName].Remove(trigger);
        }
    }
    
    public async Task TriggerEvent(GameEvent gameEvent)
    {
        String eventName = gameEvent.Name;
        if (triggers.TryGetValue(eventName, out var list))
        {
            Debug.Log(eventName+ " triggered");
            foreach (Trigger trigger in new List<Trigger>(list))
            {
                if(trigger!=null) await trigger.Activate(gameEvent);
            }
        }
        else
        {
            Debug.Log(eventName+ " triggered with 0 listeners");
        }

        await StateBasedActions();
    }
}

public class GameEvent
{
    public string Name { get; }
    public Card card { get; }
    public CardPlayer player { get; }
    public int amount { get; }
    public CardZone from { get; }
    public CardZone to { get; }

    public GameEvent(string name, Card card = null, CardPlayer player = null, int amount = 0, CardZone from = null, CardZone to = null)
    {
        Name = name;
        this.card = card;
        this.player = player;
        this.amount = amount;
        this.from = from;
    }
    
    public GameEvent(string name, CardPlayer player, int amount = 0)
    {
        Name = name;
        this.player = player;
        this.amount = amount;
    }
}

public sealed class EffectContext
{
    public Card cardSource { get; }
    public GameEvent gameEvent { get; }
    public RoundManager roundManager { get; }
 
    public EffectContext(Card source, RoundManager roundManager, GameEvent gameEvent = null, CardPlayer controller = null)
    {
        this.cardSource = source;
        this.roundManager = roundManager;
        this.gameEvent = gameEvent;
    }

    public EffectContext(RoundManager roundManager, GameEvent gameEvent = null)
    {
        this.roundManager = roundManager;
        this.gameEvent = gameEvent;
    }
    
    public CardPlayer controller => cardSource?.state.owner ?? roundManager?.activePlayer;
    public Card eventCard => gameEvent?.card;
    public CardPlayer eventPlayer => gameEvent?.player;
    
}

public class TriggerData
{
    public readonly string eventName;
    public readonly Ability ability;
    public TriggerData(string eventName, Ability ability)
    {
        this.eventName = eventName;
        this.ability = ability;
    }
}

public class Trigger
{
    private TriggerData data;
    private Card card;
    private RoundManager roundManager;

    public Trigger(TriggerData triggerData, RoundManager roundManager, Card card = null)
    {
        this.data = triggerData;
        this.card = card;
        this.roundManager = roundManager;
    }

    public void AddTrigger(TriggerEventSystem triggerEventSystem)
    {
        triggerEventSystem.AddTrigger(data.eventName, this);
    }

    public void RemoveTrigger(TriggerEventSystem triggerEventSystem)
    {
        triggerEventSystem.RemoveTrigger(data.eventName, this);
    }

    public async Task Activate(GameEvent gameEvent = null)
    {
        if (card != null)
        {
            await data.ability.Activate(new EffectContext(card, roundManager, gameEvent));
        }
        else
        {
            await data.ability.Activate(new EffectContext(roundManager, gameEvent));
        }
    }
}
