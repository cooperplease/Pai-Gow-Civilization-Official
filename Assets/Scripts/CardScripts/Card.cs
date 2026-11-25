using System;
using System.Collections.Generic;
using UnityEngine;

public enum Keywords { FLASH, QUICK, TUNNEL, VIGILANCE, INDESTRUCTABLE, POWERTHROUGH}
public enum FoilTreatment {REGULAR, FOIL, ULTRAFOIL}

public class CardData
{
    public readonly Sprite art;
    public readonly String name;
    public readonly String description;
    public readonly String flavorText;
    public readonly int power;
    public readonly int toughness;
    public readonly FoilTreatment foilTreatment;
    
    public readonly List<Keywords> keywords;
    public readonly List<TriggerData> abilities;
    
    public CardData(String name, String description, String flavorText, int power, int toughness, Sprite art, List<TriggerData> abilities = null, List<Keywords>  keywords = null)
    {
        this.art = art;
        this.name = name;
        this.description = description;
        this.flavorText = flavorText;
        this.power = power;
        this.toughness = toughness;
        if(abilities != null) this.abilities = abilities; else this.abilities = new List<TriggerData>();
        if(keywords!=null) this.keywords = keywords; else this.keywords = new List<Keywords>();
    }
}

public class CardState
{
    public bool tapped { get; private set; } = false;
    public bool summoningSick { get; private set; } = true;
    public bool declaredAttacker { get; private set; } = false;
    public int damage { get; private set; } = 0;
    public Card blockingTarget { get; private set; } = null;
    public CardZone zone { get; private set; }
    public CardPlayer owner { get; private set; }

    public CardState(CardPlayer owner, CardZone zone)
    {
        this.owner = owner;
        this.zone = zone;
    }
    
    public void Tap()
    {
        tapped = true;
    }

    public void Untap()
    {
        tapped = false;
    }

    public void Endstep()
    {
        damage = 0;
        summoningSick = false;
    }

    public void Heal()
    {
        damage = 0;
    }

    public void Reset()
    {
        tapped = false;
        declaredAttacker = false;
        damage = 0;
        blockingTarget = null;
    }

    public void DeclareAttacker()
    {
        declaredAttacker = true;
    }

    public void DeclareBlocker(Card card)
    {
        blockingTarget = card;
    }
    
    public void EndCombat()
    {
        blockingTarget = null;
        declaredAttacker = false;
    }

    public void ChangeZone(CardZone newZone)
    {
        zone = newZone;
        summoningSick = true;
    }

    public void TakeDamage(int damage)
    {
        this.damage += damage;
    }
}

public class Card
{
    public readonly CardData cardData;
    public readonly CardState state;
    public List<Trigger> abilities;
    public int Toughness => cardData.toughness-state.damage;
    
    public Card(CardData cardData, CardPlayer owner, CardZone zone)
    {
        state = new CardState(owner, zone);
        this.cardData = cardData;
    }
    
    public void Tap()
    {
        state.Tap();
    }

    public void Untap()
    {
        state.Untap();
    }

    public void Endstep()
    {
        state.Endstep();
    }

    public void Heal()
    {
        state.Heal();
    }

    public void TakeDamage(int damage)
    {
        state.TakeDamage(damage);
    }

    public void DeclareAttacker()
    {
        state.DeclareAttacker();
        if (!cardData.keywords.Contains(Keywords.VIGILANCE))
        {
            Tap();
        }
    }

    public void RegisterTriggers(RoundManager roundManager)
    {
        abilities = new List<Trigger>();
        foreach(TriggerData data in cardData.abilities)
        {
            abilities.Add(new Trigger(data,roundManager,this));
        }
        foreach(Trigger trigger in abilities) trigger.AddTrigger(roundManager.triggerEventSystem);
    }

    public void UnregisterTriggers(RoundManager roundManager)
    {
        foreach(Trigger trigger in abilities) trigger.RemoveTrigger(roundManager.triggerEventSystem);
    }
}
