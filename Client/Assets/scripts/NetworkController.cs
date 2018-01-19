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
        socket.On("select_slot", SelectSlotCallback);
        socket.On("play_card_slot", PlayCardSlotCallback);
        socket.On("discard_card", DiscardCardCallback);
        socket.On("select_done", SelectDoneCallback);
        socket.On("add_buff", AddBuffCallback);
        socket.On("remove_buff", RemoveBuffCallback);
        socket.On("update_slot_power", UpdateSlotPowerCallback);
        socket.On("update_card_power", UpdateCardPowerCallback);
    }

    #region Connection and Login

    private void ConnectionEstablishedCallback(SocketIOEvent e)
    {
        Debug.Log("[O]connection_established");
        messageOverlay.Hide();

        JSONObject data = JSONObject.Create();
        data.AddField("name", BoardBehaviour.LocalPlayerName);
        socket.Emit("login", data);
    }

    private void LoginCallback(SocketIOEvent e)
    {
        Debug.Log("[O]login: " + e.data);
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
        Debug.Log("[O]turn_for: " + e.data);
        BoardBehaviour.CurrentPlayerName = e.data.GetField("name").str;
    }

    #endregion

    #region Draw Cards

    private void DrawCardCallback(SocketIOEvent e)
    {
        Debug.Log("[O]draw_card: " + e.ToString());
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
        Debug.Log("[O]play_card " + e.ToString());
        string who = e.data.GetField("name").str;
        int guid = (int)e.data.GetField("guid").n;
        int mana = (int)e.data.GetField("mana").n;
        BoardBehaviour.CrossScenePayloads.Enqueue(new CrossScenePayload(BoardBehaviour.PlayCardCallback, who, guid, mana));
    }

    private void PlayCardFailCallback(SocketIOEvent e)
    {
        Debug.Log("[O]play_card_fail: " + e.ToString());
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
        Debug.Log("[O]update_mana: " + e.ToString());
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
        Debug.Log("[O]battle_res: " + e.ToString());
        string aname = e.data.GetField("aname").str;
        string dname = e.data.GetField("dname").str;
        List<BattleCardModifier> amods = JSONListToBCMList(e.data.GetField("amods").list);
        List<BattleCardModifier> dmods = JSONListToBCMList(e.data.GetField("dmods").list);
        int aoap = (int)e.data.GetField("aoap").n;
        int doap = (int)e.data.GetField("doap").n;
        List<Card> abattle = JSONListToCardList(e.data.GetField("abattle").list);
        List<Card> bbattle = JSONListToCardList(e.data.GetField("bbattle").list);
        List<Card> akilled = JSONListToCardList(e.data.GetField("akilled").list);
        List<Card> atouched = JSONListToCardList(e.data.GetField("atouched").list);
        List<Card> dkilled = JSONListToCardList(e.data.GetField("dkilled").list);
        List<Card> dtouched = JSONListToCardList(e.data.GetField("dtouched").list);
        int ahit = (int)e.data.GetField("ahit").n;
        int dhit = (int)e.data.GetField("dhit").n;
        int aaap = (int)e.data.GetField("aaap").n;
        int daap = (int)e.data.GetField("daap").n;
        int acs = (int)e.data.GetField("acs").n;
        int dcs = (int)e.data.GetField("dcs").n;
        int ahp = (int)e.data.GetField("ahp").n;
        int dhp = (int)e.data.GetField("dhp").n;

        BoardBehaviour.CrossScenePayloads.Enqueue(new CrossScenePayload(BoardBehaviour.BattleResult, aname, dname, amods, dmods, aoap, doap, akilled, atouched, dkilled, dtouched, ahit, dhit, abattle, bbattle, aaap, daap, acs, dcs, ahp, dhp));
    }

    #endregion

    #region Select Target

    internal void SelectTargetEmit(string name, int guid)
    {
        JSONObject data = JSONObject.Create();
        data.AddField("name", name);
        data.AddField("guid", guid);
        socket.Emit("select_target", data);
    }

    private void SelectTargetCallback(SocketIOEvent e)
    {
        Debug.Log("[O]select_target: " + e.ToString());
        string who = e.data.GetField("name").str;
        int guid = (int)e.data.GetField("guid").n;
        BoardBehaviour.CrossScenePayloads.Enqueue(new CrossScenePayload(BoardBehaviour.SelectTargetCallback, who, guid));
    }

    #endregion

    #region Select Slot

    internal void SelectSlotEmit(string name, string targetName, int slotId)
    {
        JSONObject data = JSONObject.Create();
        data.AddField("name", name);
        data.AddField("target", targetName);
        data.AddField("slot", slotId);
        socket.Emit("select_slot", data);
    }

    private void SelectSlotCallback(SocketIOEvent e)
    {
        Debug.Log("[O]select_slot: " + e.ToString());
        string who = e.data.GetField("name").str;
        int guid = (int)e.data.GetField("guid").n;
        BoardBehaviour.CrossScenePayloads.Enqueue(new CrossScenePayload(BoardBehaviour.SelectSlotCallback, who, guid));
    }

    #endregion

    #region Discard Card

    private void DiscardCardCallback(SocketIOEvent e)
    {
        Debug.Log("[O]discard_card: " + e.ToString());
        string who = e.data.GetField("name").str;
        int guid = (int)e.data.GetField("guid").n;
        BoardBehaviour.CrossScenePayloads.Enqueue(new CrossScenePayload(BoardBehaviour.DiscardCardCallback, who, guid));
    }

    #endregion

    #region Play Card Slot

    private void PlayCardSlotCallback(SocketIOEvent e)
    {
        Debug.Log("[O]play_card_slot: " + e.ToString());
        string who = e.data.GetField("name").str;
        int guid = (int)e.data.GetField("guid").n;
        int slot = (int)e.data.GetField("slot").n;
        int power = (int)e.data.GetField("power").n;
        BoardBehaviour.CrossScenePayloads.Enqueue(new CrossScenePayload(BoardBehaviour.PlayCardSlotCallback, who, guid, slot, power));
    }

    #endregion

    #region Select Done

    private void SelectDoneCallback(SocketIOEvent e)
    {
        Debug.Log("[O]select_done: " + e.ToString());
        string who = e.data.GetField("name").str;
        BoardBehaviour.CrossScenePayloads.Enqueue(new CrossScenePayload(BoardBehaviour.SelectDoneCallback, who));
    }

    #endregion

    #region Add Buff

    private void AddBuffCallback(SocketIOEvent e)
    {
        Debug.Log("[O]add_buff: " + e.ToString());
        int cardGuid = (int)e.data.GetField("guid").n;
        JSONObject obj = e.data.GetField("buff");
        int buffGuid = (int)obj.GetField("guid").n;
        string iconPath = obj.GetField("icon").str;
        BoardBehaviour.CrossScenePayloads.Enqueue(new CrossScenePayload(BoardBehaviour.AddBuffCallback, cardGuid, buffGuid, iconPath));
    }

    #endregion

    #region Remove Buff

    private void RemoveBuffCallback(SocketIOEvent e)
    {
        Debug.Log("[O]remove_buff: " + e.ToString());
        int cardGuid = (int)e.data.GetField("cguid").n;
        int buffGuid = (int)e.data.GetField("bguid").n;
        BoardBehaviour.CrossScenePayloads.Enqueue(new CrossScenePayload(BoardBehaviour.RemoveBuffCallback, cardGuid, buffGuid));
    }

    #endregion

    #region Update Card Power

    private void UpdateCardPowerCallback(SocketIOEvent e)
    {
        Debug.Log("[O]update_card_power: " + e.ToString());
        int guid = (int)e.data.GetField("guid").n;
        int power = (int)e.data.GetField("power").n;
        BoardBehaviour.CrossScenePayloads.Enqueue(new CrossScenePayload(BoardBehaviour.UpdateCardPowerCallback, guid, power));
    }

    #endregion

    #region Update Slot Power

    private void UpdateSlotPowerCallback(SocketIOEvent e)
    {
        Debug.Log("[O]update_slot_power: " + e.ToString());
        string name = e.data.GetField("name").str;
        int slot = (int)e.data.GetField("slot").n;
        int power = (int)e.data.GetField("power").n;
        BoardBehaviour.CrossScenePayloads.Enqueue(new CrossScenePayload(BoardBehaviour.UpdateSlotPowerCallback, name, slot, power));
    }

    #endregion

}
