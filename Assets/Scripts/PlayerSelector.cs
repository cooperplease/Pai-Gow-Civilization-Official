using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerSelector
{
    abstract public List<CardPlayer> Resolve(EffectContext source);
}

public class Opponent : PlayerSelector
{
    public override List<CardPlayer> Resolve(EffectContext source)
    {
        return new List<CardPlayer>() {source.controller.opponent};
    }
}

public class Owner : PlayerSelector
{
    public override List<CardPlayer> Resolve(EffectContext source)
    {
        return new List<CardPlayer>(){source.controller};
    }
}