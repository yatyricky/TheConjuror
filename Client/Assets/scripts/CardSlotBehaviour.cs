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
    public GameObject Glow;
    public GameObject PlayerObject;

    [HideInInspector] public PlayerObjectBehaviour Pob;
    private List<GameObject> cardObjs;

    private void Awake()
    {
        Pob = PlayerObject.GetComponent<PlayerObjectBehaviour>();
        TheImage.sprite = ImageAsset;
        if (Pob.IsTop)
        {
            TheImage.GetComponent<RectTransform>().Rotate(new Vector3(-180f, 180f, 0f));
        }
        cardObjs = new List<GameObject>();
    }

    internal void RerenderCards(Sequence s, float t)
    {
        if (cardObjs.Count > 0)
        {
            RectTransform cardTransform = cardObjs.ElementAt(0).transform.GetChild(0).GetComponent<RectTransform>();
            float cardWidth = cardTransform.rect.width * 0.003f; // HARD CODE
            RectTransform slotTransform = gameObject.transform.Find("frame").GetComponent<RectTransform>();
            float handWidth = slotTransform.rect.width * slotTransform.lossyScale.x * 0.9f;
            float margin = 0f;
            Vector3 basePos = gameObject.transform.position;
            if (cardObjs.Count > 1)
            {
                margin = (handWidth - cardWidth) / (cardObjs.Count - 1);
                basePos = new Vector3(gameObject.transform.position.x - handWidth / 2.0f + cardWidth / 2.0f, gameObject.transform.position.y, gameObject.transform.position.z);
            }

            for (int i = 0; i < cardObjs.Count; i++)
            {
                Vector3 pos = new Vector3(basePos.x + i * margin, basePos.y, (i + 1) * -0.01f - 1f);
                GameObject item = cardObjs.ElementAt(i);
                s.Insert(t, item.transform.DOMove(pos, GameConfig.F("CARD_SLOT_RENDER_MOVE_TIME")));
                s.Insert(t, item.transform.DOScale(1.0f, GameConfig.F("CARD_SLOT_RENDER_MOVE_TIME")));
                item.GetComponent<CardObjectBehaviour>().OriginPos = pos;

                item.SetActive(true);
            }
        }
    }

    internal void SetGlow(bool v)
    {
        Glow.SetActive(v);
    }

    internal void AddCard(GameObject co, Sequence s, float t)
    {
        cardObjs.Add(co);
        co.GetComponent<CardObjectBehaviour>().State = CardState.SLOT;
        RerenderCards(s, t);
    }

    public void UpdatePower(int v)
    {
        if (v == 0)
        {
            TotalPower.enabled = false;
        }
        else
        {
            TotalPower.enabled = true;
            TotalPower.text = v.ToString();
        }
    }

    private void OnMouseDown()
    {
        if (BoardBehaviour.GetUIState() != UIState.TARGETING && BoardBehaviour.IsCurrentPlayerAction())
        {
            NetworkController.Instance.PlayerAttackWithSlot(BoardBehaviour.LocalPlayerName, SlotId, SlotId);
        }
    }

    internal void MoveToGrave(Sequence s, float timePos, GameObject co)
    {
        cardObjs.Remove(co);
        CardObjectBehaviour cob = co.GetComponent<CardObjectBehaviour>();
        cob.OriginPos = Pob.Grave.transform.position;
        cob.State = CardState.GRAVE;
        s.Insert(timePos, co.transform.DOMove(cob.OriginPos, GameConfig.F("BATTLE_CARD_DEATH_FLY_TIME")));
        s.Insert(timePos, co.transform.DOScale(1.0f, GameConfig.F("BATTLE_CARD_SCALE_TIME")));
    }
}
