using UnityEngine;

public class TargetingVisual : MonoBehaviour
{
    CardVisual target;
    CardVisual targeter;
    public SpriteRenderer arrow;

    // Update is called once per frame
    void Update()
    {
        if (targeter!=null)
        {
            transform.position =  (targeter.transform.position);
            if (target!=null)
            {
                transform.rotation = Quaternion.LookRotation(Vector3.up,target.transform.position - targeter.transform.position);
                arrow.size = new Vector2(2, Vector3.Distance(transform.position, target.transform.position))/transform.lossyScale.y;
            }
        }
    }

    public void Setup(CardVisual target, CardVisual targeter)
    {
        this.target = target;
        this.targeter = targeter;
    }
}
