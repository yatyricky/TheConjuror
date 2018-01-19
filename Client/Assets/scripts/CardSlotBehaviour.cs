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

    internal void RerenderCards()
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
                CardObjectBehaviour cob = cardObjs.ElementAt(i).GetComponent<CardObjectBehaviour>();
                cob.OriginPos = new Vector3(basePos.x + i * margin, basePos.y, (i + 1) * -0.01f - 1f);

                cob.AddDoTweens(() => 
                {
                    cob.SetTweening(true);
                    Sequence s = DOTween.Sequence();
                    s.Insert(0f, cob.gameObject.transform.DOMove(cob.OriginPos, GameConfig.F("CARD_SLOT_RENDER_MOVE_TIME")));
                    s.Insert(0f, cob.gameObject.transform.DOScale(1.0f, GameConfig.F("CARD_SLOT_RENDER_MOVE_TIME")));
                    s.OnComplete(() =>
                    {
                        cob.SetTweening(false);
                    });
                });
            }
        }
    }

    internal void SetGlow(bool v)
    {
        Glow.SetActive(v);
    }

    internal void AddCard(GameObject co)
    {
        cardObjs.Add(co);
        co.GetComponent<CardObjectBehaviour>().State = CardState.SLOT;
        RerenderCards();
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
        if (BoardBehaviour.GetUIState() != UIState.TARGETING && BoardBehaviour.GetUIState() != UIState.SLOT_TARGETING && BoardBehaviour.IsCurrentPlayerAction())
        {
            NetworkController.Instance.PlayerAttackWithSlot(BoardBehaviour.LocalPlayerName, SlotId, SlotId);
        }
        if (BoardBehaviour.GetUIState() == UIState.SLOT_TARGETING)
        {
            NetworkController.Instance.SelectSlotEmit(BoardBehaviour.LocalPlayerName, Pob.PlayerName, SlotId);
            BoardBehaviour.SetUIState(UIState.ACTION);
        }
    }

    internal void MoveToGrave(Sequence s, float timePos, GameObject co)
    {
        if (cardObjs.Remove(co))
        {
            CardObjectBehaviour cob = co.GetComponent<CardObjectBehaviour>();
            cob.OriginPos = Pob.Grave.transform.position;
            cob.State = CardState.GRAVE;
            s.Insert(timePos, co.transform.DOMove(cob.OriginPos, GameConfig.F("BATTLE_CARD_DEATH_FLY_TIME")));
            s.Insert(timePos, co.transform.DOScale(1.0f, GameConfig.F("BATTLE_CARD_SCALE_TIME")));
        }
        else
        {
            Debug.LogError(Environment.StackTrace);
            throw new Exception("Removing invalid card object from card slot");
        }
    }
}
