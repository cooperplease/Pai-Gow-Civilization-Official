using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class CardVisual : MonoBehaviour
{
    public Card card;

    public Image selection;
    public Canvas canvas;
    public Image art;
    public TextMeshProUGUI cardName;
    public TextMeshProUGUI cardDescription;
    public TextMeshProUGUI stats;
    public TextMeshProUGUI flavorText;
    
    public CardZone currentZone;
    public Collider col; 
    
    private Camera mainCam;
    public float hoverRotationAmount = 2f;
    public float rotationSnapiness = .01f;
    public float movementSnapiness = 1f;
    public Color selectableColor;
    public Color hoveredSelectableColor;
    public Color selectedColor;
    
    public Vector3 basePos;
    public Quaternion baseRot;
    bool hovered = false;
    bool selectable = false;
    bool selected = false;
    
    public Action<CardVisual> selectCallback;
  
    private Vector3 moveVelocity = Vector3.zero;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCam = Camera.main;
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
        
        String descriptionText = "";
        foreach (Keywords keyword in card.cardData.keywords)
        {
            descriptionText += keyword.ToString();
            descriptionText += " ";
        }
        descriptionText += '\n';
        descriptionText += card.cardData.description;
        descriptionText += '\n';
        descriptionText += "<i>"+card.cardData.flavorText+"</i>";
        descriptionText += '\n';
        descriptionText += "<size=150%><align=right>"+card.cardData.power + "/" + card.cardData.toughness + "";
        cardDescription.text = descriptionText;
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

    public void SetSelectable(bool selectable)
    {
        this.selectable = selectable;
        selection.enabled = selectable;
    }

    public void SetSelected(bool selected)
    {
        this.selected = selected;
        selection.color = selected ? selectedColor : hoveredSelectableColor;
    }
    
    public static float Damp(float a, float b, float lambda, float dt)
    {
        return Mathf.Lerp(a, b, 1 - Mathf.Exp(-lambda * dt));
    }

    public void UpdateRotation()
    {
        Quaternion goalRot = baseRot;
        
        if (hovered)
        {
            goalRot = Quaternion.LookRotation(mainCam.transform.forward,mainCam.transform.up);
            if (Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
            {
                Vector3 localPoint = hit.transform.InverseTransformPoint(hit.point);
                
                float x_rot = -localPoint.y;
                float y_rot = -localPoint.x;
                goalRot *= Quaternion.Euler(-x_rot*hoverRotationAmount, y_rot*hoverRotationAmount, 0);
            }
        }
        else
        {
            if (card != null && card.state != null && card.state.tapped)
            {
                goalRot*= Quaternion.Euler(0,0,90);
            }
        }
        
        transform.rotation = Quaternion.Slerp(transform.rotation, goalRot, rotationSnapiness);
    }

    public void UpdatePosition()
    {
        Vector3 goalPos = basePos;
        Vector3 newPos = Vector3.SmoothDamp(transform.position,goalPos,ref moveVelocity,movementSnapiness);
        moveVelocity = newPos - transform.position;
        transform.position = newPos;
    }

    public void UpdateCanvasPosition()
    {
        if(hovered)canvas.transform.localScale += (new Vector3(1.5f,1.5f,1)- canvas.transform.localScale) * (5f*Time.deltaTime);
        else canvas.transform.localScale += (new Vector3(1,1,1)- canvas.transform.localScale) * (4f*Time.deltaTime);
        
        if(hovered)canvas.transform.localPosition += (new Vector3(0,0,-1)- canvas.transform.localPosition) * (5f*Time.deltaTime);
        else canvas.transform.localPosition += (new Vector3(0,0,0)- canvas.transform.localPosition) * (4f*Time.deltaTime);
    }

    public void UpdateSelectable()
    {
        if (selectable && !selected)
        {
            if (hovered)
            {
                selection.color = hoveredSelectableColor;
            }
            else
            {
                selection.color = selectableColor;
            }
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        //todo: make this good
        UpdateSelectable();
        UpdatePosition();
        UpdateCanvasPosition();
        UpdateRotation();
        UpdateZone();
    }
    
    void OnMouseOver()
    {
        hovered = true;
    }

    private void OnMouseDown()
    {
        if (selectable)
        {
            selectCallback?.Invoke(this);
            Debug.Log("selected!");
        }
    }

    void OnMouseExit()
    {
        hovered = false;
    }
}