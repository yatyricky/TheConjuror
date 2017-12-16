using System;
using UnityEngine;

public class BoardBehaviour : MonoBehaviour
{
    public GameObject PlayerA;
    public GameObject PlayerB;

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
}
