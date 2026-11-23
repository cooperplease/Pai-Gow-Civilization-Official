using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public abstract class CardSelector
{
    abstract public Task<List<Card>> Resolve(EffectContext context);
}

public class AnyCardInMyHand : CardSelector
{
    private PlayerSelector playerSelector;
    public AnyCardInMyHand(PlayerSelector player)
    {
        playerSelector = player;
    }
    public override async Task<List<Card>> Resolve(EffectContext source)
    {
        CardPlayer player = playerSelector.Resolve(source)[0];

        if (player == null)
        {
            Debug.Log("no valid player");
            return new List<Card>();
        }

        return await player.ChooseCardInHand();
    }
}

public class AllMyCardsInPlay : CardSelector
{
    private PlayerSelector _selector;
    public AllMyCardsInPlay(PlayerSelector selector)
    {
        _selector = selector;
    }
    public override async Task<List<Card>> Resolve(EffectContext source)
    {
        List<CardPlayer> players = _selector.Resolve(source);

        if (players == null)
        {
            Debug.Log("no valid player");
            return new List<Card>();
        }
        List<Card> cards =  new List<Card>();

        foreach (CardPlayer player in players)
        {
            cards.AddRange(player.inPlay.cards);
        }

        return cards;
    }
}

public class AllCardsInPlay : CardSelector
{
    public override async Task<List<Card>> Resolve(EffectContext source)
    {
        CardPlayer player = source.roundManager.activePlayer;
        return player.inPlay.cards.Concat(player.opponent.inPlay.cards).ToList();
    }
}

public class ChooseAttackers : CardSelector
{
    public override async Task<List<Card>> Resolve(EffectContext source)
    {
        CardPlayer player = source.roundManager.activePlayer;

        if (player == null)
        {
            Debug.Log("no valid player");
            return new List<Card>();
        }
        List<Card> options = new List<Card>();
        foreach (Card card in player.inPlay.cards)
        {
            if(!card.state.tapped &&(!card.state.summoningSick || card.cardData.keywords.Contains(Keywords.HASTE)))
            {
                options.Add(card);
            }
        }
        List<Card> result = await player.ChooseCardsFromList(options);
        Debug.Log("fini");
        return result;
    }
}

public class ThisCard : CardSelector
{
    public override async Task<List<Card>> Resolve(EffectContext source)
    {
        if (source.cardSource != null)
        {
            return new List<Card>() { source.cardSource };
        }
        else
        {
            return new List<Card>();
        }
    }
}

