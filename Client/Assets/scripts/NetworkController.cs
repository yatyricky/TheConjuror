using UnityEngine;
using SocketIO;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public class NetworkController : MonoBehaviour
{
    public static NetworkController Instance;

    private SocketIOComponent socket;
    private MessageBehaviour messageOverlay;

    private List<Card> JSONListToCardList(List<JSONObject> list)
    {
        List<Card> clist = new List<Card>();
        foreach (JSONObject obj in list)
        {
            int guid = (int)obj.GetField("guid").n;
            int id = (int)obj.GetField("id").n;
            string name = obj.GetField("name").str;
            string color = obj.GetField("color").str;
            string ctype = obj.GetField("ctype").str;
            int power = obj.GetField("power") == null ? 0 : (int)obj.GetField("power").n;
            string desc = obj.GetField("desc") == null ? "" : obj.GetField("desc").str;
            int cost = (int)obj.GetField("cost").n;
            clist.Add(new Card(guid, id, name, color, ctype, power, desc, cost));
        }
        return clist;
    }

    private List<BattleCardModifier> JSONListToBCMList(List<JSONObject> list)
    {
        List<BattleCardModifier> bcmlist = new List<BattleCardModifier>();
        foreach (JSONObject obj in list)
        {
            int guid = (int)obj.GetField("guid").n;
            int mod = (int)obj.GetField("mod").n;
            bcmlist.Add(new BattleCardModifier(guid, mod));
        }
        return bcmlist;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        messageOverlay = GameObject.FindGameObjectWithTag("ForceOverlay").GetComponent<MessageBehaviour>();
        messageOverlay.Hide();

        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();
        socket.On("connection_established", ConnectionEstablishedCallback);
        socket.On("login", LoginCallback);
        socket.On("turn_for", TurnForCallback);
        socket.On("draw_card", DrawCardCallback);
        socket.On("play_card", PlayCardCallback);
        socket.On("play_card_fail", PlayCardFailCallback);
        socket.On("update_mana", UpdateManaCallback);
        socket.On("battle_res", BattleResCallback);
        socket.On("select_target", SelectTargetCallback);
        socket.On("play_card_slot", PlayCardSlotCallback);
        socket.On("discard_card", DiscardCardCallback);
    }

    #region Connection and Login

    private void ConnectionEstablishedCallback(SocketIOEvent e)
    {
        Debug.Log("[O]Connected to server");
        messageOverlay.Hide();

        JSONObject data = JSONObject.Create();
        data.AddField("name", BoardBehaviour.LocalPlayerName);
        socket.Emit("login", data);
    }

    private void LoginCallback(SocketIOEvent e)
    {
        Debug.Log("[O]Login result received: " + e.data);
        if (e.data.GetField("cmd").str.Equals("wait"))
        {
            messageOverlay.Show(MessageBehaviour.WAITING);
        }
        else if (e.data.GetField("cmd").str.Equals("start"))
        {
            // Get start data
            BoardBehaviour.CrossScenePayloads.Enqueue(new CrossScenePayload(BoardBehaviour.FirstLoadDataCallback, e.data));
            // Switch to game scene
            Scene s = SceneManager.GetSceneByBuildIndex(1);
            SceneManager.LoadScene(1);
            SceneManager.SetActiveScene(s);
        }
    }

    internal void LoginPlayer(string name)
    {
        socket.Connect();
        BoardBehaviour.LocalPlayerName = name;
        messageOverlay.Show(MessageBehaviour.CONNECTING);
    }

    #endregion

    #region Turn For Player

    private void TurnForCallback(SocketIOEvent e)
    {
        BoardBehaviour.CurrentPlayerName = e.data.GetField("name").str;
    }

    #endregion

    #region Draw Cards

    private void DrawCardCallback(SocketIOEvent e)
    {
        Debug.Log("[O]PlayerDrawCards " + e.ToString());
        string who = e.data.GetField("name").str;
        List<JSONObject> cardsObj = e.data.GetField("cards").list;
        List<Card> cards = new List<Card>();
        int deckN = (int)e.data.GetField("deckN").n;
        foreach (JSONObject obj in cardsObj)
        {
            int guid = (int)obj.GetField("guid").n;
            int id = (int)obj.GetField("id").n;
            string name = obj.GetField("name").str;
            string color = obj.GetField("color").str;
            string ctype = obj.GetField("ctype").str;
            int power = obj.GetField("power") == null ? 0 : (int)obj.GetField("power").n;
            string desc = obj.GetField("desc") == null ? "" : obj.GetField("desc").str;
            int cost = (int)obj.GetField("cost").n;
            cards.Add(new Card(guid, id, name, color, ctype, power, desc, cost));
        }
        BoardBehaviour.CrossScenePayloads.Enqueue(new CrossScenePayload(BoardBehaviour.PlayerDrawCards, who, cards, deckN));
    }

    #endregion

    #region Play Card to Slot

    internal void PlayerPlayCardToSlot(string playerName, int guid, int slotId)
    {
        JSONObject data = JSONObject.Create();
        data.AddField("name", playerName);
        data.AddField("guid", guid);
        data.AddField("slotId", slotId);
        socket.Emit("play_card", data);
    }

    private void PlayCardCallback(SocketIOEvent e)
    {
        Debug.Log("[O]PlayerPlayCardToSlot " + e.ToString());
        string who = e.data.GetField("name").str;
        int guid = (int)e.data.GetField("guid").n;
        int mana = (int)e.data.GetField("mana").n;
        BoardBehaviour.CrossScenePayloads.Enqueue(new CrossScenePayload(BoardBehaviour.PlayCardCallback, who, guid, mana));
    }

    private void PlayCardFailCallback(SocketIOEvent e)
    {
        Debug.Log("[O]PlayerPlayCardToSlotFailed: " + e.ToString());
        string who = e.data.GetField("name").str;
        int guid = (int)e.data.GetField("guid").n;
        BoardBehaviour.CrossScenePayloads.Enqueue(new CrossScenePayload(BoardBehaviour.PlayCardFailCallback, who, guid));
    }

    #endregion

    #region Player End Turn

    internal void PlayerEndTurn()
    {
        JSONObject data = JSONObject.Create();
        data.AddField("name", BoardBehaviour.LocalPlayerName);
        socket.Emit("end_turn", data);
    }

    #endregion

    #region Update Player Mana

    private void UpdateManaCallback(SocketIOEvent e)
    {
        string who = e.data.GetField("name").str;
        int mana = (int)e.data.GetField("mana").n;
        BoardBehaviour.CrossScenePayloads.Enqueue(new CrossScenePayload(BoardBehaviour.UpdatePlayerMana, who, mana));
    }

    #endregion

    #region Player Attack With Slot

    internal void PlayerAttackWithSlot(string who, int from, int to)
    {
        JSONObject data = JSONObject.Create();
        data.AddField("name", who);
        data.AddField("from", from);
        data.AddField("to", to);
        socket.Emit("attack_slot", data);
    }

    private void BattleResCallback(SocketIOEvent e)
    {
        JSONObject res = e.data.GetField("res");
        string aname = res.GetField("aname").str;
        string dname = res.GetField("dname").str;
        List<BattleCardModifier> amods = JSONListToBCMList(res.GetField("amods").list);
        List<BattleCardModifier> dmods = JSONListToBCMList(res.GetField("dmods").list);
        int aoap = (int)res.GetField("aoap").n;
        int doap = (int)res.GetField("doap").n;
        List<Card> abattle = JSONListToCardList(res.GetField("abattle").list);
        List<Card> bbattle = JSONListToCardList(res.GetField("bbattle").list);
        List<Card> akilled = JSONListToCardList(res.GetField("akilled").list);
        List<Card> atouched = JSONListToCardList(res.GetField("atouched").list);
        List<Card> dkilled = JSONListToCardList(res.GetField("dkilled").list);
        List<Card> dtouched = JSONListToCardList(res.GetField("dtouched").list);
        int ahit = (int)res.GetField("ahit").n;
        int dhit = (int)res.GetField("dhit").n;
        int aaap = (int)res.GetField("aaap").n;
        int daap = (int)res.GetField("daap").n;
        int acs = (int)res.GetField("acs").n;
        int dcs = (int)res.GetField("dcs").n;
        int ahp = (int)res.GetField("ahp").n;
        int dhp = (int)res.GetField("dhp").n;

        BoardBehaviour.CrossScenePayloads.Enqueue(new CrossScenePayload(BoardBehaviour.BattleResult, aname, dname, amods, dmods, aoap, doap, akilled, atouched, dkilled, dtouched, ahit, dhit, abattle, bbattle, aaap, daap, acs, dcs, ahp, dhp));
    }

    #endregion

    #region Select Target

    private void SelectTargetCallback(SocketIOEvent e)
    {
        Debug.Log("Should be selecting a target now");
    }

    #endregion

    #region Discard Card

    private void DiscardCardCallback(SocketIOEvent e)
    {
        string who = e.data.GetField("name").str;
        int guid = (int)e.data.GetField("guid").n;
        BoardBehaviour.CrossScenePayloads.Enqueue(new CrossScenePayload(BoardBehaviour.DiscardCardCallback, who, guid));
    }

    #endregion

    #region Play Card Slot

    private void PlayCardSlotCallback(SocketIOEvent e)
    {
        string who = e.data.GetField("name").str;
        int guid = (int)e.data.GetField("guid").n;
        int slot = (int)e.data.GetField("slot").n;
        int power = (int)e.data.GetField("power").n;
        BoardBehaviour.CrossScenePayloads.Enqueue(new CrossScenePayload(BoardBehaviour.PlayCardSlotCallback, who, guid, slot, power));
    }

    #endregion

}
