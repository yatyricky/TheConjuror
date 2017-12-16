using UnityEngine;
using System.Collections;

public class EndTurnBehaviour : MonoBehaviour
{

    private void OnMouseDown()
    {
        GameLoop loop = GameObject.FindGameObjectWithTag("GameLoop").GetComponent<GameLoop>();
        loop.EndTurn();
    }
}
