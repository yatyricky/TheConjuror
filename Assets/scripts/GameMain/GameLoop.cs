using UnityEngine;

public class GameLoop : MonoBehaviour
{
    private int currentPlayer;
    public int CurrentPlayer
    {
        get { return currentPlayer; }
    }
    private int currentTurn;
    private BoardBehaviour board;

    void Start()
    {
        currentPlayer = 0;
        currentTurn = -1;
        board = GameObject.FindGameObjectWithTag("GameController").GetComponent<BoardBehaviour>();

        // register events
        RegisterTurnStartEvent(RestoreManaAndAddOne);
        RegisterTurnStartEvent(DrawCards);
        RegisterTurnStartEvent(RestoreSlotAttackCharges);
    }

    private void RestoreSlotAttackCharges(GameLoop loop)
    {
        PlayerObjectBehaviour pob = board.GetCurrentPlayerObject(CurrentPlayer).GetComponent<PlayerObjectBehaviour>();
        pob.Player.RestoreSlotAttackCharges();
    }

    private void DrawCards(GameLoop loop)
    {
        PlayerObjectBehaviour pob = board.GetCurrentPlayerObject(CurrentPlayer).GetComponent<PlayerObjectBehaviour>();
        if (currentTurn == 0)
        {
            // draw 4 cards
            pob.Deck.GetComponent<DeckObjectBehaviour>().DrawNCards(6);
        }
        else
        {
            // draw 1 card
            pob.Deck.GetComponent<DeckObjectBehaviour>().DrawNCards(1);
        }
    }

    private void RestoreManaAndAddOne(GameLoop loop)
    {
        PlayerObjectBehaviour pob = board.GetCurrentPlayerObject(CurrentPlayer).GetComponent<PlayerObjectBehaviour>();
        pob.Player.TurnAddMana();
        pob.UpdateMana();
    }

    public void EndTurn()
    {
        if (currentTurn == -1)
        {
            GameStart();
            currentTurn += 1;
        }
        else
        {
            if (currentPlayer == 0)
            {
                currentPlayer = 1;
            }
            else
            {
                currentPlayer = 0;
                currentTurn += 1;
            }

        }
        TurnStartEvents(this);
    }

    private void GameStart()
    {
        // game start
        BoardBehaviour bb = GameObject.FindGameObjectWithTag("GameController").GetComponent<BoardBehaviour>();
        GameObject p1 = bb.PlayerA;
        GameObject p2 = bb.PlayerB;
        p1.GetComponent<PlayerObjectBehaviour>().UpdateAll();
        p2.GetComponent<PlayerObjectBehaviour>().UpdateAll();
    }

    public delegate void TurnStartCallBack(GameLoop loop);
    private event TurnStartCallBack TurnStartEvents;
    public void RegisterTurnStartEvent(TurnStartCallBack callBack)
    {
        TurnStartEvents += callBack;
    }

    public static GameObject FindParentWithTag(GameObject childObject, string tag)
    {
        Transform t = childObject.transform;
        while (t.parent != null)
        {
            if (t.parent.tag == tag)
            {
                return t.parent.gameObject;
            }
            t = t.parent.transform;
        }
        return null; // Could not find a parent with given tag.
    }
}
