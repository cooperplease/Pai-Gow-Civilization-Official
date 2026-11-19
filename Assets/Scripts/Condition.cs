using System.Threading.Tasks;
using UnityEngine;

public abstract class Condition
{
    abstract public Task<bool> Activate(EffectContext context);
}
