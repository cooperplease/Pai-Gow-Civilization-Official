using System;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.InputSystem;

public class CardVisual : MonoBehaviour
{
    Card card;
    
    public Canvas canvas;
    public Image art;
    public TextMeshProUGUI cardName;
    public TextMeshProUGUI cardDescription;
    public TextMeshProUGUI stats;
    
    public CardZone currentZone;
    public Collider col; 
    
    private Camera mainCam;
    public float hoverRotationAmount = 2f;
    public float rotationSnapiness = .01f;
    public float movementSnapiness = 1f;
    
    public Vector3 basePos;
    public Quaternion baseRot;
    bool hovered = false;
    private static CardVisual selected;
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
    
    public static float Damp(float a, float b, float lambda, float dt)
    {
        return Mathf.Lerp(a, b, 1 - Mathf.Exp(-lambda * dt));
    }

    public void UpdateRotation()
    {
        Quaternion goalRot = baseRot;
        
        if (hovered)
        {
            goalRot = Quaternion.Inverse(mainCam.transform.rotation);
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
        
        canvas.transform.rotation = Quaternion.Slerp(canvas.transform.rotation, goalRot, rotationSnapiness);
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
    
    // Update is called once per frame
    void Update()
    {
        //todo: make this good
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
       
    }

    void OnMouseExit()
    {
        hovered = false;
    }
}