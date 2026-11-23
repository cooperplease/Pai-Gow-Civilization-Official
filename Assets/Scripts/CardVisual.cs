using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class CardVisual : MonoBehaviour
{
    public Card card;
    public Canvas canvas;

    public Image art;
    public TextMeshProUGUI cardName;
    public TextMeshProUGUI cardDescription;
    public TextMeshProUGUI stats;

    public Vector3 basePos;
    public Quaternion baseRot;
    
    public CardZone currentZone;
    public Collider col;
    public bool hovered = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void SetCard(Card card)
    {
        this.card = card;
        SetStats();
        UpdateZone();
    }

    void SetStats()
    {
        cardName.text = card.cardData.name;
        cardDescription.text = card.cardData.description;
        stats.text = card.cardData.power + "/" + card.cardData.toughness + "";
        if(card.cardData.art!=null) art.sprite = card.cardData.art;
    }
    
    void UpdateZone()
    {
        CardZone newZone = card?.state.zone;
        if (newZone == currentZone) return;

        if (currentZone != null && currentZone.visual != null)
            currentZone.visual.RemoveCard(this);

        currentZone = newZone;

        if (currentZone != null && currentZone.visual != null)
            currentZone.visual.AddCard(this);
    }


    // Update is called once per frame
    void Update()
    {
        //todo: make this good
        transform.position += (basePos - transform.position) *(2f* Time.deltaTime);
        if(hovered)canvas.transform.localScale += (new Vector3(2,2,1)- canvas.transform.localScale) * (2f*Time.deltaTime);
        else canvas.transform.localScale += (new Vector3(1,1,1)- canvas.transform.localScale) * (2f*Time.deltaTime);
        if(hovered)canvas.transform.localPosition += (new Vector3(0,0,-1)- canvas.transform.localPosition) * (2f*Time.deltaTime);
        else canvas.transform.localPosition += (new Vector3(0,0,0)- canvas.transform.localPosition) * (2f*Time.deltaTime);
        transform.rotation = baseRot;
        if (card != null && card.state != null && card.state.tapped)
        {
            transform.rotation*= Quaternion.Euler(0,0,90);
        }
        UpdateZone();
    }
    
    void OnMouseOver()
    {
        hovered = true;
    }

    void OnMouseExit()
    {
        hovered = false;
    }
    
    
    
}