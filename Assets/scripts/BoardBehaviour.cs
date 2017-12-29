using System;
using UnityEngine;

public class BoardBehaviour : MonoBehaviour
{
    public GameObject PlayerA;
    public GameObject PlayerB;
    private int uiState;

    private void Awake()
    {
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

    internal void SetUIState(int state)
    {
        uiState = state;
    }
}
