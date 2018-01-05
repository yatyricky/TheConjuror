using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;

public class DragHandCard : DraggingActions
{
    private void Awake()
    {
        dragabble = true;
    }

    public override void OnDraggingInUpdate()
    {
        DisableAllGlow();
        RaycastHit[] hits = Physics.RaycastAll(origin: new Vector3(transform.position.x, transform.position.y, -10), direction: new Vector3(0, 0, 1), maxDistance: 30f);
        // find card owner
        CardObjectBehaviour cob = gameObject.GetComponent<CardObjectBehaviour>();
        foreach (RaycastHit h in hits)
        {
            // Find slot owner
            PlayerObjectBehaviour hitPob = GameLoop.FindParentWithTag(h.transform.gameObject, "Player").GetComponent<PlayerObjectBehaviour>();

            if (h.transform.tag == "CardSlot" && hitPob.PlayerId == cob.Owner.PlayerId)
            {
                h.transform.Find("glow").gameObject.SetActive(true);
            }
        }
    }

    private void DisableAllGlow()
    {
        GameObject[] cardSlots = gameObject.GetComponent<CardObjectBehaviour>().Owner.CardSlots;
        for (int i = 0; i < cardSlots.Length; i ++)
        {
            cardSlots[i].transform.Find("glow").gameObject.SetActive(false);
        }
    }

    public override void OnEndDrag()
    {
        DisableAllGlow();

        // find cardslot when end drag
        GameObject endSlot = null;
        RaycastHit[] hits = Physics.RaycastAll(origin: new Vector3(transform.position.x, transform.position.y, -10), direction: new Vector3(0, 0, 1), maxDistance: 30f);
        foreach (RaycastHit h in hits)
        {
            if (h.transform.tag == "CardSlot")
            {
                endSlot = h.transform.gameObject;
            }
        }
        bool playSuccess = false;
        if (endSlot != null)
        {
            // Dragged into a cardslot
            CardObjectBehaviour cob = gameObject.GetComponent<CardObjectBehaviour>();
            CardSlotBehaviour csb = endSlot.GetComponent<CardSlotBehaviour>();
            if (cob.Owner.Player.CanPlayCardToSlot(cob.CardData, csb.SlotId))
            {
                if (cob.CardData.Type == CardTypes.CREATURE)
                {
                    new PlayCreatureCard(cob.Owner.Player, cob.CardData, csb.SlotId).Fire(PlayCreatureCardUpdateUI);
                    playSuccess = true;
                }
                if (cob.CardData.Type == CardTypes.SPELL)
                {
                    PlaySpellCardPreAction(cob.Owner.Player, cob.CardData);
                    playSuccess = true;
                }
            }
        }
        if (!playSuccess)
        {
            // Dragged into somewhere else
            CardObjectBehaviour cob = gameObject.GetComponent<CardObjectBehaviour>();
            transform.DOMove(cob.OriginPos, 0.5f).SetEase(Ease.OutCubic);
        }
    }
    
    private void PlayCreatureCardUpdateUI(GameAction.Payload payload)
    {
        PlayerObjectBehaviour pob = gameObject.GetComponent<CardObjectBehaviour>().Owner;
        int slot = (int)payload.payload;
        // 1.Make creatures un draggale
        dragabble = false;
        // 2. Remove CardObject from HandObject
        pob.HandArea.GetComponent<HandObjectBehaviour>().RemoveCard(gameObject);
        // 3. Ease/ re-organize cards in card slot
        CardSlotBehaviour csb = pob.CardSlots[slot].GetComponent<CardSlotBehaviour>();
        csb.AddCard(gameObject);
        // 4. update player mana
        pob.UpdateMana();
    }

    private void PlaySpellCardPreAction(Player player, Card cardData)
    {
        CardObjectBehaviour cob = gameObject.GetComponent<CardObjectBehaviour>();
        PlayerObjectBehaviour pob = cob.Owner;
        // 1. Make creatures undraggale
        dragabble = false;
        cob.SetMouseHovering(false);
        // 1.1. Remove CardObject from HandObject
        pob.HandArea.GetComponent<HandObjectBehaviour>().RemoveCard(gameObject);
        // 2. Play effects
        Sequence s = DOTween.Sequence();
        float timeNode = 0f;
        // 3. Move to battle field neutral position
        Vector3 spellEffectPos = GameObject.FindGameObjectWithTag("BattleNeutral").transform.position;
        s.Insert(timeNode, gameObject.transform.DOMove(new Vector3(spellEffectPos.x, spellEffectPos.y, -3f), GameConfig.SPELL_CARD_FLY_TIME));
        s.Insert(timeNode, gameObject.transform.DOScale(GameConfig.SPELL_CARD_SCALE, GameConfig.SPELL_CARD_FLY_TIME));
        // 4. Add effect to the card
        cob.AddEffectParticle();
        timeNode += GameConfig.SPELL_CARD_DISPLAY_TIME + GameConfig.SPELL_CARD_FLY_TIME;
        s.InsertCallback(timeNode, () =>
        {
            new PlaySpellCard(player, cardData).Fire(PlaySpellCardUpdateUI);
        });
    }

    private void PlaySpellCardUpdateUI(GameAction.Payload payload)
    {
        CardObjectBehaviour cob = gameObject.GetComponent<CardObjectBehaviour>();
        PlayerObjectBehaviour pob = cob.Owner;
        // 5. Update player mana
        pob.UpdateMana();
        // 6. Do corresponding UI logic
        if (payload != null)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type type = null;
            try
            {
                type = assembly.GetTypes().First(t => t.Name == payload.ActionName + "View");
            }
            catch (InvalidOperationException ioe)
            {
                Debug.LogWarning(ioe);
            }
            if (type != null)
            {
                GameActionUpdateUIView obj = (GameActionUpdateUIView)Activator.CreateInstance(type);
                obj.Payload = payload.payload;
                obj.POB = pob;
                obj.DoAction();
            }
        }

        // 7. Discard card into graveyard
        cob.OriginPos = pob.Grave.transform.position;
        gameObject.transform.DOMove(cob.OriginPos, GameConfig.BATTLE_CARD_DEATH_FLY_TIME);
        gameObject.transform.DOScale(1.0f, GameConfig.BATTLE_CARD_DEATH_FLY_TIME);
        cob.SetMouseHovering(true);
    }

    public override void OnStartDrag()
    {
    }

    protected override bool DragSuccessful()
    {
        return true;
    }
}
