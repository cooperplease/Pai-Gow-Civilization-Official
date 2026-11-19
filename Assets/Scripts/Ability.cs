using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Ability
{
    abstract public Task Activate(EffectContext source);
}

public class ChangeLife : Ability
{
    int delta;
    PlayerSelector selector;

    public ChangeLife(PlayerSelector selector, int delta)
    {
        this.delta = delta;
        this.selector = selector;
    }

    public override async Task Activate(EffectContext source)
    {
        foreach (CardPlayer player in selector.Resolve(source))
        {
            //todo: gamemanager.changeLife(player)
        }
    }
}

public class PlayCard : Ability
{
    CardSelector selector;
    public PlayCard(CardSelector selector)
    {
        this.selector = selector;
    }

    public override async Task Activate(EffectContext context)
    {
        //todo: (gamemanager.Playcard(context.eventPlayer,(await selector.Resolve(context));
    }
}

public class Tap : Ability
{
    CardSelector selector;

    public Tap(CardSelector selector)
    {
        this.selector = selector;
    }
    public override async Task Activate(EffectContext context)
    {
        foreach (Card card in await selector.Resolve(context))
        {
            await context.roundManager.TapCard(card);
        }
    }
}

public class Untap : Ability
{
    CardSelector selector;

    public Untap(CardSelector selector)
    {
        this.selector = selector;
    }
    public override async Task Activate(EffectContext context)
    {
        foreach (Card card in await selector.Resolve(context))
        {
            await context.roundManager.UntapCard(card);
        }
    }
}

public class DeclareAttacker : Ability
{
    CardSelector selector;

    public DeclareAttacker(CardSelector selector)
    {
        this.selector = selector;
    }

    public override async Task Activate(EffectContext context)
    {
        foreach (Card card in await selector.Resolve(context))
        {
            await context.roundManager.DeclareAttacker(card);
        }
    }
}

public class EndstepReset : Ability
{
    CardSelector selector;
    
    public EndstepReset(CardSelector selector)
    {
        this.selector = selector;
    }

    public override async Task Activate(EffectContext context)
    {
        foreach (Card card in await selector.Resolve(context))
        {
            card.Endstep();
        }
    }
}