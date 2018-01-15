using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using System;

public class CardObjectBehaviour : MonoBehaviour
{
    public static Dictionary<int, CardObjectBehaviour> AllCards = new Dictionary<int, CardObjectBehaviour>();

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
    public GameObject BuffAnchor;

    private static float zOffset = 0.5f;
    private float order;

    private Vector3 originPos;
    private PlayerObjectBehaviour owner;
    private List<GameObject> buffs;
    private int guid;
    private int state;
    private bool mouseHovering;
    private bool canMouseHover;
    private bool isPreviewing;

    public Vector3 OriginPos {get{ return originPos; } set { originPos = value;}}
    public PlayerObjectBehaviour Owner {get { return owner; }set { owner = value; }}
    public float Order {get { return order; }set { order = value; }}
    public int State { get { return state; } set { state = value; } }
    public int Guid { get { return guid; } }

    public Vector3 TempPos;

    public static GameObject Create(Card cardData, PlayerObjectBehaviour player)
    {
        GameObject co = Instantiate(Resources.Load("prefabs/CardObject")) as GameObject;
        CardObjectBehaviour cob = co.GetComponent<CardObjectBehaviour>();

        // card visual
        cob.CardImage.sprite = Resources.Load<Sprite>("sprites/card_images/" + cardData.id);
        cob.CardName.text = cardData.name;
        cob.CardClass.sprite = Resources.Load<Sprite>("sprites/card_ui/frame_" + cardData.color);
        cob.CardType.sprite = Resources.Load<Sprite>("sprites/card_ui/type_" + cardData.ctype);
        cob.CardPower.text = cardData.power.ToString();
        cob.CardDescription.text = cardData.desc.Replace("|n", "\n");
        cob.CardCostNumber.text = cardData.cost.ToString();
        cob.CardCostImage.sprite = Resources.Load<Sprite>("sprites/card_ui/mana_" + cardData.color);
        Sprite powerFrame = Resources.Load<Sprite>("sprites/card_ui/power_" + cardData.color);
        cob.CardTypeFrame.sprite = powerFrame;
        cob.CardPowerFrame.sprite = powerFrame;

        // layout
        cob.Order = zOffset;
        cob.isPreviewing = false;
        cob.State = CardState.DEFAULT;

        if (!cardData.ctype.Equals(CardTypes.CREATURE))
        {
            cob.CardPowerFrame.enabled = false;
            cob.CardPower.enabled = false;
        }

        cob.Owner = player;
        cob.buffs = new List<GameObject>();
        zOffset += -0.01f;
        cob.guid = cardData.guid;

        AddCard(cob);

        return co;
    }

    private static void AddCard(CardObjectBehaviour cob)
    {
        try
        {
            AllCards.Add(cob.guid, cob);
        }
        catch (ArgumentException e)
        {
            throw new Exception(" --- Catched --- : " + e.Message);
        }
    }

    public static CardObjectBehaviour GetCOB(int id)
    {
        CardObjectBehaviour ret = null;
        if (AllCards.TryGetValue(id, out ret))
        {
            return ret;
        }
        else
        {
            throw new Exception("No such card in the game, id: " + id);
        }
    }

    private void Awake()
    {
        mouseHovering = false;
        SetMouseHovering(true);
    }

    private void OnMouseOver()
    {
        if (!mouseHovering && !isPreviewing && BoardBehaviour.GetUIState() != UIState.BATTLING && canMouseHover)
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

    private void OnMouseDown()
    {
        if (BoardBehaviour.GetUIState() == UIState.TARGETING)
        {
            if (state == CardState.SLOT)
            {
                BoardBehaviour.SelectTarget(gameObject);
            }
        }
    }

    public void AddEffectParticle()
    {
        GameObject co = Instantiate(Resources.Load("prefabs/CardEffectParticle")) as GameObject;
        co.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -5f);
        co.transform.SetParent(gameObject.transform);
    }

    internal bool CanBattle()
    {
        bool can = true;
        foreach(GameObject bo in buffs)
        {
            if (bo.GetComponent<BuffBehaviour>().BuffData.Type == Buff.BuffType.NO_BATTLE)
            {
                can = false;
            }
        }
        return can;
    }

    public void SetMouseHovering(bool canMouseHover)
    {
        this.canMouseHover = canMouseHover;
    }

    private void RerenderBuffs()
    {
        Vector3 pos = BuffAnchor.transform.position;
        float z = gameObject.transform.position.z;
        for (int i = 0; i < buffs.Count; i ++) 
        {
            GameObject bo = buffs.ElementAt(i);
            bo.transform.position = new Vector3(pos.x + 0.144f + i * 0.288f, pos.y - 0.144f, z); // HARD CODE
        }
    }

    public void AddBuff(GameObject bo)
    {
        buffs.Add(bo);
        RerenderBuffs();
    }

    public void RemoveBuff(GameObject bo)
    {
        if (buffs.Remove(bo))
        {
            Destroy(bo);
            RerenderBuffs();
        }
        else
        {
            throw new Exception("Trying to remove buff that does not exist.");
        }
    }

    internal void UpdatePower(int val)
    {
        CardPower.text = val.ToString();
    }
}
