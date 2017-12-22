using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

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
                    new PlaySpellCard(cob.Owner.Player, cob.CardData).Fire(PlaySpellCardUpdateUI);
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

    /// <summary>
    /// 1. Make creatures undraggale
    /// 2. Remove CardObject from HandObject
    /// 3. Ease/ re-organize cards in card slot
    /// 4. Update player mana
    /// </summary>
    /// <param name="payload"></param>
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

    /// <summary>
    /// 1. Make creatures undraggale
    /// 2. Discard card into graveyard
    /// 3. Remove CardObject from HandObject
    /// 4. Update player mana
    /// 5. Do corresponding UI logic
    /// </summary>
    /// <param name="payload"></param>
    private void PlaySpellCardUpdateUI(GameAction.Payload payload)
    {
        PlayerObjectBehaviour pob = gameObject.GetComponent<CardObjectBehaviour>().Owner;
        // 1.Make creatures un draggale
        dragabble = false;
        // 2. Discard card into graveyard
        CardObjectBehaviour cob = gameObject.GetComponent<CardObjectBehaviour>();
        cob.OriginPos = new Vector3(7.35f, -4f, -0.1f);
        gameObject.transform.DOMove(cob.OriginPos, 1f);
        // 3. Remove CardObject from HandObject
        pob.HandArea.GetComponent<HandObjectBehaviour>().RemoveCard(gameObject);
        // 4. update player mana
        pob.UpdateMana();
        // 5. Do corresponding UI logic
        if (payload != null)
        {
            if (payload.ActionName == "DrawCard")
            {
                new DrawCardView((List<Card>)payload.payload, pob);
            }
        }
    }

    public override void OnStartDrag()
    {
    }

    protected override bool DragSuccessful()
    {
        return true;
    }
}
