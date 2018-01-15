using UnityEngine;

public class EndTurnBehaviour : MonoBehaviour
{

    private void OnMouseDown()
    {
        if (BoardBehaviour.IsCurrentPlayerAction())
        {
            NetworkController.Instance.PlayerEndTurn();
        }
    }
}
