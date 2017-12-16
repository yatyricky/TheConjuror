using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerObjectBehaviour : MonoBehaviour
{
    public int PlayerId;
    public Text HealthPoints;
    public Text ManaPoints;
    public GameObject HandArea;
    public GameObject Deck;
    public GameObject[] CardSlots;

    private Player player;

    public Player Player { get { return player; }}

    private void Start()
    {
        // HARDCODE
        if (PlayerId == 0)
            player = new Player(this, "blue_basic");
        else
            player = new Player(this, "red_basic");
    }

    public void UpdateHealth()
    {
        int sum = player.Health;
        HealthPoints.text = sum.ToString();
    }

    public void UpdateMana()
    {
        int sum = player.Mana;
        ManaPoints.text = sum.ToString();
    }

    public PlayerObjectBehaviour GetOpponent()
    {
        BoardBehaviour bb = GameObject.FindGameObjectWithTag("GameController").GetComponent<BoardBehaviour>();
        PlayerObjectBehaviour p1 = bb.PlayerA.GetComponent<PlayerObjectBehaviour>();
        PlayerObjectBehaviour p2 = bb.PlayerB.GetComponent<PlayerObjectBehaviour>();
        if (PlayerId == p1.PlayerId)
        {
            return p2;
        }
        else
        {
            return p1;
        }
    }

    internal void UpdateAll()
    {
        UpdateHealth();
        UpdateMana();
        Deck.GetComponent<DeckObjectBehaviour>().UpdateDeckNumber();
        for (int i = 0; i < CardSlots.Length; i ++)
        {
            CardSlots[i].GetComponent<CardSlotBehaviour>().UpdatePower();
        }
    }
}
