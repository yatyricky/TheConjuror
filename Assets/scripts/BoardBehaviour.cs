using System;
using UnityEngine;

public class BoardBehaviour : MonoBehaviour
{
    public GameObject PlayerA;
    public GameObject PlayerB;
    private int uiState;
    private SelectTargetCard currentSelector;

    public static BoardBehaviour GetInstance()
    {
        return GameObject.FindGameObjectWithTag("GameController").GetComponent<BoardBehaviour>();
    }

    private void Awake()
    {
        currentSelector = null;
        uiState = UIState.ACTION;
    }

    internal GameObject GetCurrentPlayerObject(int currentPlayer)
    {
        if (currentPlayer == 0)
        {
            return PlayerA;
        }
        else
        {
            return PlayerB;
        }
    }

    internal void SetCurrentSelector(SelectTargetCard selectTargetCard)
    {
        currentSelector = selectTargetCard;
    }

    internal void SetUIState(int state)
    {
        uiState = state;
    }

    internal int GetUIState()
    {
        return uiState;
    }

    internal void SelectTarget(GameObject cardObject)
    {
        if (currentSelector == null)
        {
            throw new Exception("There is no TargetSelector object");
        }
        else
        {
            currentSelector.TargetAcquired(cardObject);
        }
    }
}
