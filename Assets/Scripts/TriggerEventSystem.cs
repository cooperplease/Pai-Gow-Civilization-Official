using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TriggerEventSystem
{
    Dictionary<String, List<Trigger>> triggers;
    
    public TriggerEventSystem()
    {
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
    
    public async Task TriggerEvent(String eventName, EffectContext context = null)
    {
        if (triggers.TryGetValue(eventName, out var list))
        {
            Debug.Log(eventName+ " triggered");
            foreach (Trigger trigger in new List<Trigger>(list))
            {
                if(trigger!=null) await trigger.Activate(context);
            }
        }
        else
        {
            Debug.Log(eventName+ " triggered with 0 listeners");
        }
    }
}

public abstract class GameEvent
{
    public abstract string Name { get; }
    
    public virtual Card card => null;
    public virtual CardPlayer player => null;
}

public class CreatureDiedEvent : GameEvent
{
    public override string Name => "CreatureDied";
 
    public Card creature { get; }
    public CardZone from { get; }
 
    public CreatureDiedEvent(Card creature, CardZone from)
    {
        this.creature = creature;
        this.from = from;
    }
 
    public override Card card => creature;
    public override CardPlayer player => creature?.state.owner;
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

    public EffectContext(RoundManager roundManager)
    {
        this.roundManager = roundManager;
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
    TriggerData data;

    public Trigger(TriggerData triggerData)
    {
        this.data = triggerData;
    }

    public void AddTrigger(TriggerEventSystem triggerEventSystem)
    {
        triggerEventSystem.AddTrigger(data.eventName, this);
    }

    public void RemoveTrigger(TriggerEventSystem triggerEventSystem)
    {
        triggerEventSystem.RemoveTrigger(data.eventName, this);
    }

    public async Task Activate(EffectContext effectContext = null)
    {
        await data.ability.Activate(effectContext);
    }
}
