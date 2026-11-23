using System.Threading.Tasks;
using UnityEngine;

public abstract class Condition
{
    abstract public Task<bool> Activate(EffectContext context);
}

public class InZone : Condition
{
    CardSelector cardSelector;
    CardZone.ZoneType zoneType;

    public InZone(CardSelector cardSelector, CardZone.ZoneType zoneType)
    {
        this.cardSelector = cardSelector;
        this.zoneType = zoneType;
    }

    public override async Task<bool> Activate(EffectContext source)
    {
        foreach (Card card in await cardSelector.Resolve(source))
        {
            if (card.state.zone.zoneType == this.zoneType)
            {
                return true;
            }
        }
        return false;
    }
}

public class OwnerIsActivePlayer : Condition
{
    CardSelector cardSelector;

    public OwnerIsActivePlayer(CardSelector cardSelector)
    {
        this.cardSelector = cardSelector;
    }

    public override async Task<bool> Activate(EffectContext source)
    {
        foreach (Card card in await cardSelector.Resolve(source))
        {
            if (card.state.owner == source.roundManager.activePlayer)
            {
                return true;
            }
        }
        return false;
    }
}
