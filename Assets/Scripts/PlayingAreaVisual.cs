using UnityEngine;

public class PlayingAreaVisual : MonoBehaviour
{
    
    public GameObject cardVisualPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnCard(Card card)
    {
        GameObject visual = Instantiate(cardVisualPrefab);
        CardVisual cardVisual = visual.GetComponent<CardVisual>();
        cardVisual.SetCard(card);
    }
}
