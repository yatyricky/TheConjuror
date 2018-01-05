using System;
using UnityEngine;
using UnityEngine.UI;

public class SelectTargetCard : MonoBehaviour
{
    public Image Aim;
    public Image Arrow;
    public LineRenderer lineRenderer;

    private Player player;
    private Card card;
    private Action<GameObject> actWhenSelected;

    private float zDisplacement;

    internal void SetData(Player player, Card card, Action<GameObject> act)
    {
        this.player = player;
        this.card = card;
        actWhenSelected = act;
        BoardBehaviour bb = BoardBehaviour.GetInstance();
        bb.SetUIState(UIState.TARGETING);
        bb.SetCurrentSelector(this);
    }

    private void Start()
    {
        zDisplacement = - Camera.main.transform.position.z + transform.position.z;
    }

    private Vector3 MouseInWorldCoords()
    {
        var screenMousePos = Input.mousePosition;
        screenMousePos.z = zDisplacement;
        return Camera.main.ScreenToWorldPoint(screenMousePos);
    }

    private void Update()
    {
        Vector3 mousePos = MouseInWorldCoords();
        Arrow.enabled = false;
        Aim.transform.position = new Vector3(mousePos.x, mousePos.y, transform.position.z);
        Vector3 from = new Vector3(card.COB.gameObject.transform.position.x, card.COB.gameObject.transform.position.y, -4f);
        Vector3 to = new Vector3(mousePos.x, mousePos.y, -4f);
        Vector3 direction = (to - from).normalized;
        lineRenderer.SetPositions(new Vector3[] {from , to - direction * 0.9f});
    }

    internal void TargetAcquired(GameObject cardObject)
    {
        BoardBehaviour.GetInstance().SetUIState(UIState.ACTION);
        actWhenSelected(cardObject);
    }
}
