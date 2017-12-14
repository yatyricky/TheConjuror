using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HandObjectBehaviour : MonoBehaviour
{
    private List<GameObject> cardObjs;

    private void Awake()
    {
        cardObjs = new List<GameObject>();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void RerenderCards()
    {
        if (cardObjs.Count > 0)
        {
            RectTransform cardTransform = cardObjs.ElementAt(0).transform.GetChild(0).GetComponent<RectTransform>();
            float cardWidth = cardTransform.rect.width * cardTransform.lossyScale.x;
            float handWidth = cardWidth * cardObjs.Count;

            Vector3 basePos = new Vector3(gameObject.transform.position.x - handWidth / 2.0f + cardWidth / 2.0f, gameObject.transform.position.y, gameObject.transform.position.z);
            for (int i = 0; i < cardObjs.Count; i++)
            {
                Vector3 pos = new Vector3(basePos.x + i * cardWidth * 1.03f, basePos.y, basePos.z);
                cardObjs.ElementAt(i).transform.position = pos;

                cardObjs.ElementAt(i).SetActive(true);
            }
        }
    }

    internal void AddCard(GameObject co)
    {
        cardObjs.Add(co);
        RerenderCards();
    }

    internal void RemoveCard(GameObject co)
    {
        int index = -1;
        for (int i = 0; i < cardObjs.Count && index == -1; i ++)
        {
            if (co == cardObjs.ElementAt(i))
            {
                index = i;
            }
        }
        if (index == -1)
        {
            throw new Exception("Removing CardObject that doesn't exist");
        }
        else
        {
            cardObjs.RemoveAt(index);
        }
        RerenderCards();
    }
}
