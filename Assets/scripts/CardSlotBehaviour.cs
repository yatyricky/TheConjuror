using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardSlotBehaviour : MonoBehaviour
{
    public int SlotId;
    public Sprite ImageAsset;
    public Image TheImage;
    public Text TotalPower;

    private PlayerObjectBehaviour pob;
    private List<GameObject> cardObjs;

    private void Awake()
    {
        pob = GameLoop.FindParentWithTag(gameObject, "Player").GetComponent<PlayerObjectBehaviour>();
        TheImage.sprite = ImageAsset;
        if (pob.PlayerId == 1)
        {
            TheImage.GetComponent<RectTransform>().Rotate(new Vector3(-180f, 180f, 0f));
        }
        cardObjs = new List<GameObject>();
    }

    private void RerenderCards()
    {
        if (cardObjs.Count > 0)
        {
            RectTransform cardTransform = cardObjs.ElementAt(0).transform.GetChild(0).GetComponent<RectTransform>();
            float cardWidth = cardTransform.rect.width * cardTransform.lossyScale.x;
            RectTransform slotTransform = gameObject.transform.Find("frame").GetComponent<RectTransform>();
            float handWidth = slotTransform.rect.width * slotTransform.lossyScale.x * 0.9f;
            float margin = 0f;
            Vector3 basePos = gameObject.transform.position;
            if (cardObjs.Count > 1)
            {
                margin = (handWidth - cardWidth) / (cardObjs.Count - 1);
                basePos = new Vector3(gameObject.transform.position.x - handWidth / 2.0f + cardWidth / 2.0f, gameObject.transform.position.y, gameObject.transform.position.z);
            }

            for (int i = 0; i < cardObjs.Count; i ++)
            {
                Vector3 pos = new Vector3(basePos.x + i * margin, basePos.y, (i + 1) * -0.01f - 1f);
                cardObjs.ElementAt(i).transform.position = pos;
                CardObjectBehaviour cob = cardObjs.ElementAt(i).GetComponent<CardObjectBehaviour>();
                cob.OriginPos = pos;

                cardObjs.ElementAt(i).SetActive(true);
            }
        }
    }

    internal void AddCard(GameObject co)
    {
        cardObjs.Add(co);
        RerenderCards();
        UpdatePower();
    }

    public void UpdatePower()
    {
        int sum = pob.Player.GetSlotPower(SlotId);
        if (sum == 0)
        {
            TotalPower.enabled = false;
        }
        else
        {
            TotalPower.enabled = true;
            TotalPower.text = sum.ToString();
        }
    }

    private void OnMouseDown()
    {
        GameLoop loop = GameObject.FindGameObjectWithTag("GameLoop").GetComponent<GameLoop>();
        if (pob.Player.GetSlotPower(SlotId) > 0 && loop.CurrentPlayer == pob.PlayerId)
        {
            new Attack(pob.Player, pob.GetOpponent().Player, SlotId).Fire(UpdateUI);
        }
    }

    public void UpdateUI(object payload)
    {
        Attack.AttackResult aa = (Attack.AttackResult)payload;
        
        CardSlotBehaviour defenderCsb = pob.GetOpponent().CardSlots[SlotId].GetComponent<CardSlotBehaviour>();

        for (int i = 0; i < aa.Attacker.Count; i++)
        {
            GameObject co = FindCardObjectByCardData(aa.Attacker.ElementAt(i));
            MoveToGrave(co);
        }

        for (int i = 0; i < aa.Defender.Count; i++)
        {
            GameObject co = defenderCsb.FindCardObjectByCardData(aa.Defender.ElementAt(i));
            defenderCsb.MoveToGrave(co);
        }
        UpdatePower();
        defenderCsb.UpdatePower();

        UpdateCardsPower();
        defenderCsb.UpdateCardsPower();
    }

    private void UpdateCardsPower()
    {
        for (int i = 0; i < cardObjs.Count; i ++)
        {
            CardObjectBehaviour cob = cardObjs[i].GetComponent<CardObjectBehaviour>();
            cob.CardPower.text = cob.CardData.Power.ToString();
        }
    }

    private GameObject FindCardObjectByCardData(Card card)
    {
        foreach (GameObject cardObj in cardObjs)
        {
            if (cardObj.GetComponent<CardObjectBehaviour>().CardData == card)
            {
                return cardObj;
            }
        }
        throw new Exception("Card not found in card slot: "+card.Name);
    }

    private void MoveToGrave(GameObject co)
    {
        cardObjs.Remove(co);
        RerenderCards();
        CardObjectBehaviour cob = co.GetComponent<CardObjectBehaviour>();
        cob.OriginPos = new Vector3(7.35f, -4f, -0.1f);
        co.transform.DOMove(cob.OriginPos, 1f);
    }
}
