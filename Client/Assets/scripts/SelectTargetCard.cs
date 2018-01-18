using System;
using UnityEngine;
using UnityEngine.UI;

public class SelectTargetCard : MonoBehaviour
{
    public Image Aim;
    public Image Arrow;
    public LineRenderer lineRenderer;

    [HideInInspector]public GameObject From;

    private float zDisplacement;

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
        if (From != null)
        {
            Vector3 mousePos = MouseInWorldCoords();
            Arrow.enabled = false;
            Aim.transform.position = new Vector3(mousePos.x, mousePos.y, transform.position.z);
            Vector3 from = new Vector3(From.transform.position.x, From.transform.position.y, -4f);
            Vector3 to = new Vector3(mousePos.x, mousePos.y, -4f);
            Vector3 direction = (to - from).normalized;
            lineRenderer.SetPositions(new Vector3[] { from, to - direction * 0.9f });
        }
    }
}
