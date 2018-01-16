using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerObjectBehaviour : MonoBehaviour
{
    public bool IsTop;
    public Text HealthPoints;
    public Text ManaPoints;
    public GameObject HandArea;
    public GameObject Deck;
    public GameObject Grave;
    public GameObject[] CardSlots;

    private string playerName;
    public string PlayerName { get { return playerName; } set { playerName = value; } }

    [HideInInspector] public HandObjectBehaviour Hob;
    [HideInInspector] public DeckObjectBehaviour Dob;
    [HideInInspector] public CardSlotBehaviour[] CSob;

    private void Awake()
    {
        Hob = HandArea.GetComponent<HandObjectBehaviour>();
        Dob = Deck.GetComponent<DeckObjectBehaviour>();
        CSob = new CardSlotBehaviour[5];
        for (int i = 0; i < CardSlots.Length; i++)
        {
            CSob[i] = CardSlots[i].GetComponent<CardSlotBehaviour>();
            UpdateCardSlotPower(i, 0);
        }
        UpdateHealth(0);
        UpdateMana(0);
        UpdateDeckNumber(0);
    }

    internal void UpdateCardSlotPower(int n, int v)
    {
        CSob[n].UpdatePower(v);
    }

    internal void UpdateHealth(int hp)
    {
        HealthPoints.text = hp.ToString();
    }

    internal void UpdateMana(int mp)
    {
        ManaPoints.text = mp.ToString();
    }

    internal void UpdateDeckNumber(int deckN)
    {
        Dob.UpdateDeckNumber(deckN);
    }

    internal void DiscardCard(GameObject co)
    {
        CardObjectBehaviour cob = co.GetComponent<CardObjectBehaviour>();
        cob.OriginPos = Grave.transform.position;
        co.transform.DOMove(cob.OriginPos, GameConfig.F("BATTLE_CARD_DEATH_FLY_TIME"));
        co.transform.DOScale(1.0f, GameConfig.F("BATTLE_CARD_DEATH_FLY_TIME"));
    }
}
