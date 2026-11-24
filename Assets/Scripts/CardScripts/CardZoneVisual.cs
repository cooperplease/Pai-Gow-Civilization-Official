using System.Collections.Generic;
using UnityEngine;

public class CardZoneVisual : MonoBehaviour
{
    public Vector3 offset = Vector3.up;
    public List<CardVisual> cards = new List<CardVisual>();
    public float cardWidth = 2.5f;
    public float cardHeight = 3.5f;
    public float spacing = .5f;

    public void AddCard(CardVisual card)
    {
        cards.Add(card);
        CalculateBasePositions();
    }

    public void RemoveCard(CardVisual card)
    {
        cards.Remove(card);
        CalculateBasePositions();
    }

    public static void MoveCard(CardVisual card, CardZone newZone)
    {
        
    }

    public void CalculateBasePositions()
    {
        float totalWidth = (cardHeight+ spacing) *cards.Count;
        for(int i = 0; i< cards.Count; i++)
        {
            
            cards[i].basePos = transform.TransformPoint((i - ((cards.Count-1) / 2f))/cards.Count * Vector3.right+ offset);
            cards[i].baseRot = transform.rotation;
        }
        
    }
}