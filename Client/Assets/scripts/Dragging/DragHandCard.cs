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
        // find card object behaviour
        CardObjectBehaviour cob = gameObject.GetComponent<CardObjectBehaviour>();
        foreach (RaycastHit h in hits)
        {
            // Find slot owner
            CardSlotBehaviour csob = h.transform.gameObject.GetComponent<CardSlotBehaviour>();

            if (h.transform.tag == "CardSlot" && csob.Pob.PlayerName.Equals(cob.Owner.PlayerName))
            {
                csob.SetGlow(true);
            }
        }
    }

    private void DisableAllGlow()
    {
        CardSlotBehaviour[] cardSlots = gameObject.GetComponent<CardObjectBehaviour>().Owner.CSob;
        for (int i = 0; i < cardSlots.Length; i++)
        {
            cardSlots[i].SetGlow(false);
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
        CardObjectBehaviour cob = gameObject.GetComponent<CardObjectBehaviour>();
        if (endSlot != null && BoardBehaviour.IsCurrentPlayerAction())
        {
            //Dragged into a cardslot
            CardSlotBehaviour csb = endSlot.GetComponent<CardSlotBehaviour>();
            NetworkController.Instance.PlayerPlayCardToSlot(cob.Owner.PlayerName, cob.Guid, csb.SlotId);

            // Move card to neutral position and play effects
            Vector3 spellEffectPos = GameObject.FindGameObjectWithTag("BattleNeutral").transform.position;
            gameObject.transform.DOMove(new Vector3(spellEffectPos.x, spellEffectPos.y, -3f), GameConfig.F("SPELL_CARD_FLY_TIME"));
            gameObject.transform.DOScale(GameConfig.F("SPELL_CARD_SCALE"), GameConfig.F("SPELL_CARD_FLY_TIME"));
            //  - Add effect to the card
            cob.AddEffectParticle();
        }
        else
        {
            // Dragged into somewhere else
            transform.DOMove(cob.OriginPos, 0.5f).SetEase(Ease.OutCubic);
        }
    }

    private void PlaySpellCardPreAction(object player, Card cardData)
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
        s.Insert(timeNode, gameObject.transform.DOMove(new Vector3(spellEffectPos.x, spellEffectPos.y, -3f), GameConfig.F("SPELL_CARD_FLY_TIME")));
        s.Insert(timeNode, gameObject.transform.DOScale(GameConfig.F("SPELL_CARD_SCALE"), GameConfig.F("SPELL_CARD_FLY_TIME")));
        // 4. Add effect to the card
        cob.AddEffectParticle();
        timeNode += GameConfig.F("SPELL_CARD_DISPLAY_TIME") + GameConfig.F("SPELL_CARD_FLY_TIME");
        s.InsertCallback(timeNode, () =>
        {
            //new PlaySpellCard(player, cardData).Fire(PlaySpellCardUpdateUI);
        });
    }

    private void PlaySpellCardUpdateUI(object payload)
    {
        CardObjectBehaviour cob = gameObject.GetComponent<CardObjectBehaviour>();
        PlayerObjectBehaviour pob = cob.Owner;
        // 5. Update player mana
        //pob.UpdateMana();
        // 6. Do corresponding UI logic
        if (payload != null)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type type = null;
            try
            {
                //type = assembly.GetTypes().First(t => t.Name == payload.ActionName + "View");
            }
            catch (InvalidOperationException ioe)
            {
                Debug.LogWarning(ioe);
            }
            if (type != null)
            {
                GameActionUpdateUIView obj = (GameActionUpdateUIView)Activator.CreateInstance(type);
                //obj.Payload = payload.payload;
                obj.POB = pob;
                obj.DoAction();
            }
        }

        // 7. Discard card into graveyard
        cob.OriginPos = pob.Grave.transform.position;
        gameObject.transform.DOMove(cob.OriginPos, GameConfig.F("BATTLE_CARD_DEATH_FLY_TIME"));
        gameObject.transform.DOScale(1.0f, GameConfig.F("BATTLE_CARD_DEATH_FLY_TIME"));
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
