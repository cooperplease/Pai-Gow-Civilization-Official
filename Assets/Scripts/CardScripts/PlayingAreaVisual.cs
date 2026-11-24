using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PlayingAreaVisual : MonoBehaviour
{
    public CardZoneVisual player1Hand;
    public CardZoneVisual player2Hand;

    public CardZoneVisual player1Battlefield;
    public CardZoneVisual player2Battlefield;

    public CardZoneVisual player1Graveyard;
    public CardZoneVisual player2Graveyard;
    
    public GameObject CardVisualPrefab;
    
    public List<CardVisual> cards;
    
    public Button submitButton;
    public Canvas submitButtonCanvas;
    bool readyToSubmit = false;
    List<Card> selectedCards = new List<Card>();
    private int max_cards;
    private int min_cards;

    void Start()
    {
        submitButton.onClick.AddListener(() => TrySubmitSelections());
    }

    public void SpawnCard(Card card)
    {
        GameObject cardVisual = Instantiate(CardVisualPrefab);
        CardVisual cardVisualScript = cardVisual.GetComponent<CardVisual>();
        cardVisualScript.SetCard(card);
        cards.Add(cardVisualScript);
    }

    public async Task<List<Card>> ChooseRangeOfCardsFromList(int min, int max, List<Card> selectableCards)
    {
        submitButtonCanvas.enabled = true;
        selectedCards.Clear();
        readyToSubmit = false;
        this.min_cards = min;
        this.max_cards = max;
        
        foreach (CardVisual card in cards)
        {
            if (selectableCards.Contains(card.card))
            {
                card.SetSelectable(true);
                card.selectCallback += CardClicked;
            }
        }
        
        while (!readyToSubmit)
        {
            await Task.Yield();
        }
        
        foreach (CardVisual card in cards)
        {
            if (selectableCards.Contains(card.card))
            {
                card.SetSelectable(false);
                card.SetSelected(false);
                card.selectCallback -= CardClicked;
            }
        }
        submitButtonCanvas.enabled = false;
        return selectedCards;
    }

    public void CardClicked(CardVisual card)
    {
        if (!selectedCards.Contains(card.card))
        {
            if (max_cards > selectedCards.Count)
            {
                selectedCards.Add(card.card);
                card.SetSelected(true);
            }
            else
            {
                Debug.Log("Can't select that many cards!");
            }
        }
        else
        {
            selectedCards.Remove(card.card);
            card.SetSelected(false);
        }
    }

    public void TrySubmitSelections()
    {
        if (selectedCards.Count >= min_cards && selectedCards.Count <= max_cards)
        {
            readyToSubmit = true;
        }
        else
        {
            Debug.Log("Can't submit selection");
        }
    }
}