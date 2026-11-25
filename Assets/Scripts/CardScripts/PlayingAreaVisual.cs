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
    List<CardVisual> selectedCardVisuals = new List<CardVisual>();
    private int max_cards;
    private int min_cards;
    private Card highlightedCard;
    private Mode mode;

    enum Mode
    {
        HIGHLIGHTING,
        SELECTING
    }

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
        mode = Mode.HIGHLIGHTING;
        submitButtonCanvas.enabled = true;
        selectedCardVisuals.Clear();
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
        
        List<Card> SelectedCards = new List<Card>();
        foreach (CardVisual card in selectedCardVisuals) SelectedCards.Add(card.card);
        return SelectedCards;
    }

    public void CardClicked(CardVisual card)
    {
        if (!selectedCardVisuals.Contains(card))
        {
            if (max_cards > selectedCardVisuals.Count)
            {
                selectedCardVisuals.Add(card);
                card.SetSelected(true);
            }
            else
            {
                CardVisual unselected = selectedCardVisuals[0];
                selectedCardVisuals.Remove(unselected);
                unselected.SetSelected(false);
                
                selectedCardVisuals.Add(card);
                card.SetSelected(true);
            }
        }
        else
        {
            selectedCardVisuals.Remove(card);
            card.SetSelected(false);
        }
    }
    
    public void CardHighlighted(CardVisual card)
    {
        highlightedCard = card.card;
        readyToSubmit = true;
    }

    public void TrySubmitSelections()
    {
        switch (mode)
        {
            case Mode.SELECTING:
                if (selectedCardVisuals.Count >= min_cards && selectedCardVisuals.Count <= max_cards)
                    {
                        readyToSubmit = true;
                    }
                        else
                    {
                        Debug.Log("Can't submit selection");
                    }
                break;
            case Mode.HIGHLIGHTING:
                readyToSubmit = true;
                break;
            default:
                break;
        }
    }

    public async Task<Card> HighlightCardFromList(List<Card> selectableCards)
    {
        submitButtonCanvas.enabled = true;
        mode = Mode.HIGHLIGHTING;
        readyToSubmit = false;
        highlightedCard = null;
        foreach (CardVisual card in cards)
        {
            if (selectableCards.Contains(card.card))
            {
                card.SetSelectable(true);
                card.selectCallback += CardHighlighted;
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
                card.selectCallback -= CardHighlighted;
            }
        }

        submitButtonCanvas.enabled = false;
        return highlightedCard;
    }

    public void Reset()
    {
        foreach (CardVisual card in cards)
        {
            Destroy(card.gameObject);
        }
        player1Battlefield.cards.Clear();
        player2Battlefield.cards.Clear();
        player1Battlefield.cards.Clear();
        player2Battlefield.cards.Clear();
        player1Graveyard.cards.Clear();
        player2Graveyard.cards.Clear();
        cards.Clear();
    }
}