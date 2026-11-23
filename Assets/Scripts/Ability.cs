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
    PlayerSelector selectorPlayer;
    public PlayCard(CardSelector selector, PlayerSelector playerSelector)
    {
        this.selector = selector;
        this.selectorPlayer = playerSelector;
    }

    public override async Task Activate(EffectContext context)
    {
        await context.roundManager.PlayCards(selectorPlayer.Resolve(context)[0],await selector.Resolve(context));
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

public class AssignBlockers : Ability
{
    public override async Task Activate(EffectContext source)
    {
        Dictionary<Card,Card> Blocks =  new Dictionary<Card,Card>();
        
        CardPlayer player = source.roundManager.activePlayer.opponent;
        List<Card> attackingCards = new List<Card>();
        foreach (Card card in player.opponent.inPlay.cards)
        {
            if(card.state.declaredAttacker) attackingCards.Add(card);
        }
        List<Card> potentialBlockers = new List<Card>();
        foreach (Card card in player.inPlay.cards)
        {
            if (!card.state.tapped)
            {   
                potentialBlockers.Add(card);
            }
        }
        foreach (Card card in potentialBlockers)
        {
            List<Card> EligibleAttackers = new List<Card>();
            foreach (Card card2 in attackingCards)
            {
                if(!(card2.cardData.keywords.Contains(Keywords.FLYING)&&!card.cardData.keywords.Contains(Keywords.FLYING))) EligibleAttackers.Add(card2);
            }
            List<Card> cardToBlock = (await player.ChooseUpToXCardsFromList(1,EligibleAttackers));
            if(cardToBlock!=null&&cardToBlock.Count == 1) Blocks.Add(card, cardToBlock[0]);
        }
        foreach (Card card in Blocks.Keys)
        {
            await source.roundManager.AssignBlock(card,Blocks[card]);
        }
    }
}

public class Conditional : Ability
{
    Condition test;
    Ability ifTrue;

    public Conditional(Condition test, Ability ifTrue)
    {
        this.test = test;
        this.ifTrue = ifTrue;
    }
    
    public override async Task Activate(EffectContext context)
    {
        if (await test.Activate(context))
        {
            await ifTrue.Activate(context);
        }
    }
}