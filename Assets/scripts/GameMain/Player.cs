using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player
{
    private PlayerObjectBehaviour playerObjectBehaviour;
    private Deck deck = null;
    private List<Card> hand;
    private CardSlot[] cardSlots;
    private List<Card> grave;
    private int health;
    private int mana;
    private int maxHealth;
    private int maxMana;

    public Deck Deck { get { return deck; } }
    public int Health { get { return health; } set { health = value; } }
    public int Mana { get { return mana; } set { mana = value; } }
    public PlayerObjectBehaviour POB {get{return playerObjectBehaviour;}}

    public Player(PlayerObjectBehaviour aob, string deckName)
    {
        playerObjectBehaviour = aob;

        hand = new List<Card>();
        grave = new List<Card>();
        cardSlots = new CardSlot[5];
        // HARDCODE
        cardSlots[0] = new CardSlot(Colors.WHITE);
        cardSlots[1] = new CardSlot(Colors.BLUE);
        cardSlots[2] = new CardSlot(Colors.ALL);
        cardSlots[3] = new CardSlot(Colors.GREEN);
        cardSlots[4] = new CardSlot(Colors.RED);

        // HARDCODE
        maxMana = 0;
        mana = maxMana;
        maxHealth = 20;
        health = maxHealth;

        InitDeck(deckName);
    }

    internal Attack.AttackResult Attack(Player defender, int cardSlot)
    {
        Attack.AttackResult res = new Attack.AttackResult();

        int attackerPower = GetSlotPower(cardSlot);
        int defenderPower = defender.GetSlotPower(cardSlot);
        List<Card> thisKilled = cardSlots[cardSlot].TakeDamage(defenderPower);
        List<Card> defenderkilled = defender.cardSlots[cardSlot].TakeDamage(attackerPower);
        thisKilled.ForEach(card => grave.Add(card));
        defenderkilled.ForEach(card => defender.grave.Add(card));
        res.Attacker = thisKilled;
        res.Defender = defenderkilled;
        return res;
    }

    internal void TurnAddMana()
    {
        maxMana += 1;
        // HARDCODE
        if (maxMana > 12)
        {
            maxMana = 12;
        }
        mana = maxMana;
    }

    public int GetSlotPower(int slotId)
    {
        return cardSlots[slotId].GetTotalPower();
    }

    internal Card DrawRandom()
    {
        Card card = Deck.DrawRandom();
        hand.Add(card);
        return card;
    }

    internal void PlayCardFromHand(Card card, int slotId)
    {
        if (hand.Remove(card))
        {
            if (card.Type.Equals(CardTypes.CREATURE) || card.Type.Equals(CardTypes.ENCHANTMENT))
            {
                cardSlots[slotId].Add(card);
            }
            else
            {
                // TODO playing spells
                grave.Add(card);
            }
        }
        else
        {
            throw new Exception("Playing card that not in hand");
        }
    }

    private void InitDeck(string name)
    {
        DataManager dm = GameObject.FindGameObjectWithTag("GameData").GetComponent<DataManager>();
        List<DataManager.DeckData> ddl = null;
        if (dm.DeckBase.TryGetValue(name, out ddl))
        {
            deck = new Deck();
            ddl.ForEach(dd =>
            {
                for (int i = 0; i < dd.Num; i ++)
                {
                    deck.Cards.Add(new Card(dd.CardId));
                }
            });
        }
        else
        {
            throw new Exception("Initing deck with unknown deck name:" + name);
        }
    }
}