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
                GameObject item = cardObjs.ElementAt(i);
                item.transform.DOMove(pos, GameConfig.CARD_SLOT_RENDER_MOVE_TIME);
                item.transform.DOScale(1.0f, GameConfig.CARD_SLOT_RENDER_MOVE_TIME);
                item.GetComponent<CardObjectBehaviour>().OriginPos = pos;

                item.SetActive(true);
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
        if (pob.Player.GetSlotPower(SlotId) > 0 && loop.CurrentPlayer == pob.PlayerId && pob.Player.CanAttackWithSlot(SlotId))
        {
            new Attack(pob.Player, pob.GetOpponent().Player, SlotId).Fire(UpdateUI);
            BoardBehaviour bb = GameObject.FindGameObjectWithTag("GameController").GetComponent<BoardBehaviour>();
            bb.SetUIState(UIState.BATTLING);
        }
    }

    public void UpdateUI(GameAction.Payload payload)
    {
        Debug.Log("Updating UI");
        Attack.AttackResult aa = (Attack.AttackResult)payload.payload;
        // Move 2 set of cards in each slot
        CardSlotBehaviour csbA = pob.CardSlots[aa.AttackerSlotId].GetComponent<CardSlotBehaviour>();
        CardSlotBehaviour csbB = pob.GetOpponent().CardSlots[aa.AttackerSlotId].GetComponent<CardSlotBehaviour>();

        Sequence s = DOTween.Sequence();
        float endTimeNode = 0f;

        // To battle position
        Vector3 attackerAnchor = GameObject.FindGameObjectWithTag("BattleAttacker").transform.position;
        attackerAnchor.z = GameConfig.BATTLE_Z_INDEX;
        Vector3 defenderAnchor = GameObject.FindGameObjectWithTag("BattleDefender").transform.position;
        defenderAnchor.z = GameConfig.BATTLE_Z_INDEX;
        for (int i = 0; i < csbA.cardObjs.Count; i++)
        {
            GameObject item = csbA.cardObjs.ElementAt(i);
            float time = GameConfig.BATTLE_CARD_INTERVAL * i;
            s.Insert(time, item.transform.DOMove(attackerAnchor, GameConfig.BATTLE_CARD_FLY_TIME).SetEase(Ease.OutCubic));
            s.Insert(time, item.transform.DOScale(GameConfig.BATTLE_CARD_SCALE, GameConfig.BATTLE_CARD_SCALE_TIME));
            attackerAnchor.x += GameConfig.BATTLE_CARD_SPACING;
            attackerAnchor.z -= 0.1f;
        }
        for (int i = 0; i < csbB.cardObjs.Count; i++)
        {
            GameObject item = csbB.cardObjs.ElementAt(i);
            float time = GameConfig.BATTLE_CARD_INTERVAL * i;
            s.Insert(time, item.transform.DOMove(defenderAnchor, GameConfig.BATTLE_CARD_FLY_TIME).SetEase(Ease.OutCubic));
            s.Insert(time, item.transform.DOScale(GameConfig.BATTLE_CARD_SCALE, GameConfig.BATTLE_CARD_SCALE_TIME));
            defenderAnchor.x -= GameConfig.BATTLE_CARD_SPACING;
            defenderAnchor.z -= 0.1f;
        }
        endTimeNode = Math.Max(csbA.cardObjs.Count, csbB.cardObjs.Count) * GameConfig.BATTLE_CARD_INTERVAL;

        // Cards do effects

        // Do damage
        s.InsertCallback(endTimeNode, () =>
        {
            GameObject defSlashSFX = Instantiate(Resources.Load("prefabs/SlashEffect")) as GameObject;
            defSlashSFX.transform.position = defenderAnchor;
            Destroy(defSlashSFX, GameConfig.BATTLE_SLASH_TIME);
            GameObject atkSlashSFX = Instantiate(Resources.Load("prefabs/SlashEffect")) as GameObject;
            atkSlashSFX.transform.position = attackerAnchor;
            Destroy(atkSlashSFX, GameConfig.BATTLE_SLASH_TIME);
        });
        endTimeNode += GameConfig.BATTLE_SLASH_TIME;

        endTimeNode += GameConfig.BATTLE_AFTER_DAMAGE_INTV;
        // Clear up
        s.InsertCallback(endTimeNode, () =>
        {
            UpdatePower();
            csbB.UpdatePower();

            UpdateCardsPower();
            csbB.UpdateCardsPower();

            pob.UpdateHealth();
            pob.GetOpponent().UpdateHealth();
        });

        // Move deads to grave
        for (int i = 0; i < aa.Attacker.Count; i++)
        {
            GameObject co = FindCardObjectByCardData(aa.Attacker.ElementAt(i));
            MoveToGrave(s, endTimeNode + GameConfig.BATTLE_CARD_INTERVAL * i, co);
        }

        for (int i = 0; i < aa.Defender.Count; i++)
        {
            GameObject co = csbB.FindCardObjectByCardData(aa.Defender.ElementAt(i));
            csbB.MoveToGrave(s, endTimeNode + GameConfig.BATTLE_CARD_INTERVAL * i, co);
        }

        s.OnComplete(() =>
        {
            RerenderCards();
            csbB.RerenderCards();
        });
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

    private void MoveToGrave(Sequence s, float timePos, GameObject co)
    {
        cardObjs.Remove(co);
        CardObjectBehaviour cob = co.GetComponent<CardObjectBehaviour>();
        cob.OriginPos = new Vector3(7.35f, -4f, -0.1f);
        s.Insert(timePos, co.transform.DOMove(cob.OriginPos, GameConfig.BATTLE_CARD_DEATH_FLY_TIME));
        s.Insert(timePos, co.transform.DOScale(1.0f, GameConfig.BATTLE_CARD_SCALE_TIME));
    }
}
