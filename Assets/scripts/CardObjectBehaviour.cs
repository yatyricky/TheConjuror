﻿using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardObjectBehaviour : MonoBehaviour
{
    public Image CardClass; // earth/ fire/ air/ water
    public Image CardImage; // the graphics
    public Text CardName; // card name
    public Image CardTypeFrame;
    public Image CardType;
    public Image CardPowerFrame;
    public Text CardPower;
    public Text CardDescription;
    public Image CardCostImage;
    public Text CardCostNumber;

    private static float zOffset = 0.5f;
    private float order;

    private Vector3 originPos;
    private Card cardData;
    private PlayerObjectBehaviour owner;
    private bool mouseHovering;
    private bool canMouseHover;
    private bool isPreviewing;

    public Vector3 OriginPos {get{ return originPos; } set { originPos = value;}}
    public Card CardData {get { return cardData; } set { cardData = value; }}
    public PlayerObjectBehaviour Owner {get { return owner; }set { owner = value; }}
    public float Order {get { return order; }set { order = value; }}

    public Vector3 TempPos;

    public static GameObject Create(Card cardData, PlayerObjectBehaviour player)
    {
        GameObject co = Instantiate(Resources.Load("prefabs/CardObject")) as GameObject;
        CardObjectBehaviour cob = co.GetComponent<CardObjectBehaviour>();

        // card visual
        cob.CardImage.sprite = Resources.Load<Sprite>("sprites/card_images/" + cardData.Id);
        cob.CardName.text = cardData.Name;
        cob.CardClass.sprite = Resources.Load<Sprite>("sprites/card_ui/frame_" + cardData.Color);
        cob.CardType.sprite = Resources.Load<Sprite>("sprites/card_ui/type_" + cardData.Type);
        cob.CardPower.text = cardData.Power.ToString();
        cob.CardDescription.text = cardData.Description.Replace("|n", "\n");
        cob.CardCostNumber.text = cardData.Cost.ToString();
        cob.CardCostImage.sprite = Resources.Load<Sprite>("sprites/card_ui/mana_" + cardData.Color);
        Sprite powerFrame = Resources.Load<Sprite>("sprites/card_ui/power_" + cardData.Color);
        cob.CardTypeFrame.sprite = powerFrame;
        cob.CardPowerFrame.sprite = powerFrame;

        // layout
        cob.Order = zOffset;
        cob.isPreviewing = false;

        if (!cardData.Type.Equals(CardTypes.CREATURE))
        {
            cob.CardPowerFrame.enabled = false;
            cob.CardPower.enabled = false;
        }

        cob.Owner = player;
        cob.CardData = cardData;

        zOffset += -0.01f;
        return co;
    }

    private void Awake()
    {
        mouseHovering = false;
        SetMouseHovering(true);
    }

    private void OnMouseOver()
    {
        BoardBehaviour bb = GameObject.FindGameObjectWithTag("GameController").GetComponent<BoardBehaviour>();
        if (!mouseHovering && !isPreviewing && bb.GetUIState() != UIState.BATTLING && canMouseHover)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -2f);
            mouseHovering = true;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            gameObject.transform.DOMove(new Vector3(0f, 0f, -2f), 0.2f).SetEase(Ease.OutCubic);
            gameObject.transform.DOScale(2f, 0.2f).SetEase(Ease.OutCubic);
            isPreviewing = true;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            gameObject.transform.DOMove(originPos, 0.2f).SetEase(Ease.OutCubic);
            gameObject.transform.DOScale(1f, 0.2f).SetEase(Ease.OutCubic);
            isPreviewing = false;
        }
    }

    private void OnMouseExit()
    {
        if (mouseHovering && !isPreviewing && canMouseHover)
        {
            gameObject.transform.position = originPos;
            mouseHovering = false;
        }
    }

    public void AddEffectParticle()
    {
        GameObject co = Instantiate(Resources.Load("prefabs/CardEffectParticle")) as GameObject;
        co.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -5f);
        co.transform.SetParent(gameObject.transform);
    }

    public void SetMouseHovering(bool canMouseHover)
    {
        this.canMouseHover = canMouseHover;
    }

}
