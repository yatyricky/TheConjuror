﻿using UnityEngine;

public class DragHandCard : DraggingActions
{
    private static GameObject[] cardSlots = null;
    private GameObject endSlot = null;

    private void Awake()
    {
        dragabble = true;
        cardSlots = GameObject.FindGameObjectWithTag("GameData").GetComponent<DataManager>().CardSlots;
    }

    public override void OnDraggingInUpdate()
    {
        DisableAllGlow();
        RaycastHit[] hits = Physics.RaycastAll(origin: new Vector3(transform.position.x, transform.position.y, -10), direction: new Vector3(0, 0, 1), maxDistance: 30f);
        foreach (RaycastHit h in hits)
        {
            if (h.transform.tag == "CardSlot")
            {
                h.transform.Find("glow").gameObject.SetActive(true);
            }
        }
    }

    private void DisableAllGlow()
    {
        foreach (GameObject cs in cardSlots)
        {
            cs.transform.Find("glow").gameObject.SetActive(false);
        }
    }

    public override void OnEndDrag()
    {
        DisableAllGlow();

        // find cardslot when end drag
        endSlot = null;
        RaycastHit[] hits = Physics.RaycastAll(origin: new Vector3(transform.position.x, transform.position.y, -10), direction: new Vector3(0, 0, 1), maxDistance: 30f);
        foreach (RaycastHit h in hits)
        {
            if (h.transform.tag == "CardSlot")
            {
                endSlot = h.transform.gameObject;
            }
        }
        if (endSlot != null)
        {
            CardObjectBehaviour cob = gameObject.GetComponent<CardObjectBehaviour>();
            new PlayCard(cob.Owner, cob.CardData).Fire(UpdateUI);
        }
    }

    /// <summary>
    /// 1. Make creatures un draggale
    /// 2. Remove CardObject from HandObject
    /// </summary>
    /// <param name="payload"></param>
    private void UpdateUI(object payload)
    {
        // 1.Make creatures un draggale
        dragabble = false;
        // 2. Remove CardObject from HandObject
        DataManager dm = GameObject.FindGameObjectWithTag("GameData").GetComponent<DataManager>();
        HandObjectBehaviour hob = dm.HandAreas[(int)payload].GetComponent<HandObjectBehaviour>();
        hob.RemoveCard(gameObject);
    }

    public override void OnStartDrag()
    {
        
    }

    protected override bool DragSuccessful()
    {
        return true;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
