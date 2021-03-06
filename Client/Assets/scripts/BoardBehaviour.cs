﻿using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardBehaviour : MonoBehaviour
{
    public static Queue<CrossScenePayload> CrossScenePayloads = new Queue<CrossScenePayload>();
    public static string LocalPlayerName;
    public static string CurrentPlayerName;
    public static PlayerObjectBehaviour LocalPlayer;
    public static PlayerObjectBehaviour EnemyPlayer;

    private static int uiState;
    private static GameObject SelectTargetObject;

    [Header("Players")]
    public GameObject PlayerA;
    public GameObject PlayerB;
    [Header("Public Game Objects")]
    public GameObject NeutralBattlePointGO;
    public GameObject AttackerBattlePointtGO;
    public GameObject DefenderBattlePointGO;

    [HideInInspector] public static GameObject NeutralBattlePoint;
    [HideInInspector] public static GameObject AttackerBattlePoint;
    [HideInInspector] public static GameObject DefenderBattlePoint;

    private void Awake()
    {
        uiState = UIState.ACTION;

        LocalPlayer = PlayerA.GetComponent<PlayerObjectBehaviour>();
        EnemyPlayer = PlayerB.GetComponent<PlayerObjectBehaviour>();
        NeutralBattlePoint = NeutralBattlePointGO;
        AttackerBattlePoint = AttackerBattlePointtGO;
        DefenderBattlePoint = DefenderBattlePointGO;
    }

    private void Update()
    {
        while (CrossScenePayloads.Count > 0)
        {
            CrossScenePayloads.Dequeue().Fire();
        }
    }

    internal static void SetUIState(int state)
    {
        uiState = state;
    }

    internal static int GetUIState()
    {
        return uiState;
    }

    internal static void SelectTarget(GameObject co)
    {
        NetworkController.Instance.SelectTargetEmit(LocalPlayerName, co.GetComponent<CardObjectBehaviour>().Guid);
    }

    internal static bool IsCurrentPlayerAction()
    {
        return CurrentPlayerName.Equals(LocalPlayerName);
    }

    internal static void FirstLoadDataCallback(object[] data)
    {
        JSONObject firstLoadData = (JSONObject)data[0];

        JSONObject p1 = firstLoadData.GetField("players").GetField("p1");
        string p1Name = p1.GetField("name").str;
        int p1Hp = (int)p1.GetField("hp").n;
        int p1Mp = (int)p1.GetField("mp").n;

        JSONObject p2 = firstLoadData.GetField("players").GetField("p2");
        string p2Name = p2.GetField("name").str;
        int p2Hp = (int)p2.GetField("hp").n;
        int p2Mp = (int)p2.GetField("mp").n;

        if (p1Name.Equals(LocalPlayerName))
        {
            LocalPlayer.PlayerName = p1Name;
            LocalPlayer.UpdateHealth(p1Hp);
            LocalPlayer.UpdateMana(p1Mp);

            EnemyPlayer.PlayerName = p2Name;
            EnemyPlayer.UpdateHealth(p2Hp);
            EnemyPlayer.UpdateMana(p2Mp);
        }
        else if (p2Name.Equals(LocalPlayerName))
        {
            EnemyPlayer.PlayerName = p1Name;
            EnemyPlayer.UpdateHealth(p1Hp);
            EnemyPlayer.UpdateMana(p1Mp);

            LocalPlayer.PlayerName = p2Name;
            LocalPlayer.UpdateHealth(p2Hp);
            LocalPlayer.UpdateMana(p2Mp);
        }
        else
        {
            throw new Exception("[UpdateStartUI]Neither player name match local names");
        }
    }

    internal static void PlayerDrawCards(object[] data)
    {
        string who = (string)data[0];
        List<Card> cards = (List<Card>)data[1];
        int deckN = (int)data[2];
        if (LocalPlayer.PlayerName.Equals(who))
        {
            LocalPlayer.Dob.DrawCardsUI(cards, deckN);
        }
        else if (EnemyPlayer.PlayerName.Equals(who))
        {
            EnemyPlayer.Dob.DrawCardsUI(cards, deckN);
        }
        else
        {
            throw new Exception("[PlayerDrawCards]Neither player name match local names");
        }
    }

    private static PlayerObjectBehaviour ForPlayer(string name)
    {
        PlayerObjectBehaviour ret = LocalPlayer;
        if (name.Equals(EnemyPlayer.PlayerName))
        {
            ret = EnemyPlayer;
        }
        return ret;
    }

    internal static void PlayCardCallback(object[] data)
    {
        string who = (string)data[0];
        PlayerObjectBehaviour forPlayer = LocalPlayer;
        if (who.Equals(EnemyPlayer.PlayerName))
        {
            forPlayer = EnemyPlayer;
        }
        int guid = (int)data[1];
        int mana = (int)data[2];
        CardObjectBehaviour cob = CardObjectBehaviour.GetCOB(guid);
        forPlayer.Hob.RemoveCard(cob.gameObject);
        forPlayer.UpdateMana(mana);
        cob.gameObject.GetComponent<DragHandCard>().CanDrag = false;

        cob.AddDoTweens(() =>
        {
            // Move card to neutral position and play effects
            Vector3 spellEffectPos = NeutralBattlePoint.transform.position;
            cob.AddEffectParticle();
            cob.SetTweening(true);
            Sequence s = DOTween.Sequence();
            s.Insert(0f, cob.gameObject.transform.DOMove(new Vector3(spellEffectPos.x, spellEffectPos.y, -3f), GameConfig.F("SPELL_CARD_FLY_TIME")));
            s.Insert(0f, cob.gameObject.transform.DOScale(GameConfig.F("SPELL_CARD_SCALE"), GameConfig.F("SPELL_CARD_FLY_TIME")));
            s.AppendInterval(GameConfig.F("SPELL_CARD_FLY_TIME") + GameConfig.F("SPELL_CARD_DISPLAY_TIME"));
            s.OnComplete(() => 
            {
                cob.SetTweening(false);
            });
        });
    }

    internal static void PlayCardFailCallback(object[] data)
    {
        string who = (string)data[0];
        int guid = (int)data[1];
        PlayerObjectBehaviour forPlayer = LocalPlayer;
        if (who.Equals(EnemyPlayer.PlayerName))
        {
            forPlayer = EnemyPlayer;
        }
        forPlayer.Hob.PlayCardFail(CardObjectBehaviour.GetCOB(guid).gameObject);
    }

    internal static void UpdatePlayerMana(object[] data)
    {
        string who = (string)data[0];
        int mana = (int)data[1];
        PlayerObjectBehaviour forPlayer = LocalPlayer;
        if (who.Equals(EnemyPlayer.PlayerName))
        {
            forPlayer = EnemyPlayer;
        }
        forPlayer.UpdateMana(mana);
    }

    internal static void BattleResult(object[] data)
    {
        string aname = (string)data[0];
        string dname = (string)data[1];
        List<BattleCardModifier> amods = (List<BattleCardModifier>)data[2];
        List<BattleCardModifier> dmods = (List<BattleCardModifier>)data[3];
        int aoap = (int)data[4];
        int doap = (int)data[5];
        List<Card> akilled = (List<Card>)data[6];
        List<Card> atouched = (List<Card>)data[7];
        List<Card> dkilled = (List<Card>)data[8];
        List<Card> dtouched = (List<Card>)data[9];
        int ahit = (int)data[10];
        int dhit = (int)data[11];
        List<Card> abattle = (List<Card>)data[12];
        List<Card> bbattle = (List<Card>)data[13];
        int aaap = (int)data[14];
        int daap = (int)data[15];
        int acs = (int)data[16];
        int dcs = (int)data[17];
        int ahp = (int)data[18];
        int dhp = (int)data[19];

        Sequence s = DOTween.Sequence();
        float endTimeNode = 0f;
        // We are battling
        SetUIState(UIState.BATTLING);
        // Move 2 set of cards to battle position
        int indexCountA = 0;
        int indexCountB = 0;
        Vector3 attackerAnchor = AttackerBattlePoint.transform.position;
        attackerAnchor.z = GameConfig.F("BATTLE_Z_INDEX");
        Vector3 defenderAnchor = DefenderBattlePoint.transform.position;
        defenderAnchor.z = GameConfig.F("BATTLE_Z_INDEX");
        for (int i = 0; i < abattle.Count; i++)
        {
            CardObjectBehaviour item = CardObjectBehaviour.GetCOB(abattle.ElementAt(i).guid);
            float time = GameConfig.F("BATTLE_CARD_INTERVAL") * i;
            item.TempPos = attackerAnchor;
            s.Insert(time, item.transform.DOMove(attackerAnchor, GameConfig.F("BATTLE_CARD_FLY_TIME")).SetEase(Ease.OutCubic));
            s.Insert(time, item.transform.DOScale(GameConfig.F("BATTLE_CARD_SCALE"), GameConfig.F("BATTLE_CARD_SCALE_TIME")));
            attackerAnchor.x += GameConfig.F("BATTLE_CARD_SPACING");
            attackerAnchor.z -= 0.01f;
            indexCountA++;
        }
        for (int i = 0; i < bbattle.Count; i++)
        {
            CardObjectBehaviour item = CardObjectBehaviour.GetCOB(bbattle.ElementAt(i).guid);
            float time = GameConfig.F("BATTLE_CARD_INTERVAL") * i;
            item.TempPos = defenderAnchor;
            s.Insert(time, item.transform.DOMove(defenderAnchor, GameConfig.F("BATTLE_CARD_FLY_TIME")).SetEase(Ease.OutCubic));
            s.Insert(time, item.transform.DOScale(GameConfig.F("BATTLE_CARD_SCALE"), GameConfig.F("BATTLE_CARD_SCALE_TIME")));
            defenderAnchor.x -= GameConfig.F("BATTLE_CARD_SPACING");
            defenderAnchor.z -= 0.01f;
            indexCountB++;
        }
        endTimeNode = Math.Max(abattle.Count, bbattle.Count) * GameConfig.F("BATTLE_CARD_INTERVAL");

        // Cards do effects
        for (int i = 0; i < amods.Count; i++)
        {
            BattleCardModifier bcm = amods.ElementAt(i);
            CardObjectBehaviour item = CardObjectBehaviour.GetCOB(bcm.guid);
            float time = endTimeNode + (GameConfig.F("BATTLE_CARD_INTERVAL") +
                GameConfig.F("BATTLE_CARD_EFFECT_SCALE_TIME") * 2.0f +
                GameConfig.F("BATTLE_CARD_EFFECT_HIGHLIGHT_PAUSE") * 2.0f +
                GameConfig.F("BATTLE_CARD_EFFECT_LABEL_SCALE_TIME") +
                GameConfig.F("BATTLE_CARD_EFFECT_LABEL_PAUSE")) * i;
            // pop out active card
            s.Insert(time, item.transform.DOMove(new Vector3(item.TempPos.x, item.TempPos.y, item.TempPos.z - 1f), GameConfig.F("BATTLE_CARD_EFFECT_SCALE_TIME")));
            s.Insert(time, item.transform.DOScale(GameConfig.F("BATTLE_CARD_EFFECT_SCALE"), GameConfig.F("BATTLE_CARD_EFFECT_SCALE_TIME")));
            s.InsertCallback(time, () =>
            {
                item.AddEffectParticle();
            });
            // show effect values
            GameObject buffLabel = AttackerBattlePoint.transform.Find("Buff").gameObject;
            GameObject debuffLabel = AttackerBattlePoint.transform.Find("Debuff").gameObject;
            GameObject activeLabel = null;
            if (bcm.mod > 0)
            {
                buffLabel.SetActive(false);
                debuffLabel.SetActive(false);
                buffLabel.GetComponent<BuffLabelBehaviour>().Text.text = "+" + bcm.mod.ToString();
                activeLabel = buffLabel;
            }
            else
            {
                buffLabel.SetActive(false);
                debuffLabel.SetActive(false);
                debuffLabel.GetComponent<BuffLabelBehaviour>().Text.text = bcm.mod.ToString();
                activeLabel = debuffLabel;
            }
            s.InsertCallback(time +
                GameConfig.F("BATTLE_CARD_EFFECT_SCALE_TIME") +
                GameConfig.F("BATTLE_CARD_EFFECT_HIGHLIGHT_PAUSE"), () =>
                {
                    activeLabel.SetActive(true);
                    activeLabel.transform.DOScale(GameConfig.F("BATTLE_CARD_EFFECT_LABEL_SCALE"), GameConfig.F("BATTLE_CARD_EFFECT_LABEL_SCALE_TIME"));
                });
            s.InsertCallback(time +
                GameConfig.F("BATTLE_CARD_EFFECT_SCALE_TIME") +
                GameConfig.F("BATTLE_CARD_EFFECT_HIGHLIGHT_PAUSE") * 2.0f +
                GameConfig.F("BATTLE_CARD_EFFECT_LABEL_SCALE_TIME") +
                GameConfig.F("BATTLE_CARD_EFFECT_LABEL_PAUSE"), () =>
                {
                    activeLabel.SetActive(false);
                    activeLabel.transform.DOScale(1.0f, GameConfig.F("BATTLE_CARD_EFFECT_LABEL_SCALE_TIME"));
                });
            // restore showing card
            s.Insert(time +
                GameConfig.F("BATTLE_CARD_EFFECT_SCALE_TIME") +
                GameConfig.F("BATTLE_CARD_EFFECT_HIGHLIGHT_PAUSE") * 2.0f +
                GameConfig.F("BATTLE_CARD_EFFECT_LABEL_SCALE_TIME") +
                GameConfig.F("BATTLE_CARD_EFFECT_LABEL_PAUSE"), item.transform.DOMove(item.TempPos, GameConfig.F("BATTLE_CARD_EFFECT_SCALE_TIME")));
            s.Insert(time +
                GameConfig.F("BATTLE_CARD_EFFECT_SCALE_TIME") +
                GameConfig.F("BATTLE_CARD_EFFECT_HIGHLIGHT_PAUSE") * 2.0f +
                GameConfig.F("BATTLE_CARD_EFFECT_LABEL_SCALE_TIME") +
                GameConfig.F("BATTLE_CARD_EFFECT_LABEL_PAUSE"), item.transform.DOScale(GameConfig.F("BATTLE_CARD_SCALE"), GameConfig.F("BATTLE_CARD_EFFECT_SCALE_TIME")));
        }
        for (int i = 0; i < dmods.Count; i++)
        {
            BattleCardModifier bcm = dmods.ElementAt(i);
            CardObjectBehaviour item = CardObjectBehaviour.GetCOB(bcm.guid);
            float time = endTimeNode + (GameConfig.F("BATTLE_CARD_INTERVAL") +
                GameConfig.F("BATTLE_CARD_EFFECT_SCALE_TIME") * 2.0f +
                GameConfig.F("BATTLE_CARD_EFFECT_HIGHLIGHT_PAUSE") * 2.0f +
                GameConfig.F("BATTLE_CARD_EFFECT_LABEL_SCALE_TIME") +
                GameConfig.F("BATTLE_CARD_EFFECT_LABEL_PAUSE")) * i;
            // pop out active card
            s.Insert(time, item.transform.DOMove(new Vector3(item.TempPos.x, item.TempPos.y, item.TempPos.z - 1f), GameConfig.F("BATTLE_CARD_EFFECT_SCALE_TIME")));
            s.Insert(time, item.transform.DOScale(GameConfig.F("BATTLE_CARD_EFFECT_SCALE"), GameConfig.F("BATTLE_CARD_EFFECT_SCALE_TIME")));
            // show effect values
            GameObject buffLabel = DefenderBattlePoint.transform.Find("Buff").gameObject;
            GameObject debuffLabel = DefenderBattlePoint.transform.Find("Debuff").gameObject;
            GameObject activeLabel = null;
            if (bcm.mod > 0)
            {
                buffLabel.SetActive(false);
                debuffLabel.SetActive(false);
                buffLabel.GetComponent<BuffLabelBehaviour>().Text.text = "+" + bcm.mod.ToString();
                activeLabel = buffLabel;
            }
            else
            {
                buffLabel.SetActive(false);
                debuffLabel.SetActive(false);
                debuffLabel.GetComponent<BuffLabelBehaviour>().Text.text = bcm.mod.ToString();
                activeLabel = debuffLabel;
            }
            s.InsertCallback(time +
                GameConfig.F("BATTLE_CARD_EFFECT_SCALE_TIME") +
                GameConfig.F("BATTLE_CARD_EFFECT_HIGHLIGHT_PAUSE"), () =>
                {
                    activeLabel.SetActive(true);
                    activeLabel.transform.DOScale(GameConfig.F("BATTLE_CARD_EFFECT_LABEL_SCALE"), GameConfig.F("BATTLE_CARD_EFFECT_LABEL_SCALE_TIME"));
                });
            s.InsertCallback(time +
                GameConfig.F("BATTLE_CARD_EFFECT_SCALE_TIME") +
                GameConfig.F("BATTLE_CARD_EFFECT_HIGHLIGHT_PAUSE") * 2.0f +
                GameConfig.F("BATTLE_CARD_EFFECT_LABEL_SCALE_TIME") +
                GameConfig.F("BATTLE_CARD_EFFECT_LABEL_PAUSE"), () =>
                {
                    activeLabel.SetActive(false);
                    activeLabel.transform.DOScale(1.0f, GameConfig.F("BATTLE_CARD_EFFECT_LABEL_SCALE_TIME"));
                });
            // restore showing card
            s.Insert(time +
                GameConfig.F("BATTLE_CARD_EFFECT_SCALE_TIME") +
                GameConfig.F("BATTLE_CARD_EFFECT_HIGHLIGHT_PAUSE") * 2.0f +
                GameConfig.F("BATTLE_CARD_EFFECT_LABEL_SCALE_TIME") +
                GameConfig.F("BATTLE_CARD_EFFECT_LABEL_PAUSE"), item.transform.DOMove(item.TempPos, GameConfig.F("BATTLE_CARD_EFFECT_SCALE_TIME")));
            s.Insert(time +
                GameConfig.F("BATTLE_CARD_EFFECT_SCALE_TIME") +
                GameConfig.F("BATTLE_CARD_EFFECT_HIGHLIGHT_PAUSE") * 2.0f +
                GameConfig.F("BATTLE_CARD_EFFECT_LABEL_SCALE_TIME") +
                GameConfig.F("BATTLE_CARD_EFFECT_LABEL_PAUSE"), item.transform.DOScale(GameConfig.F("BATTLE_CARD_SCALE"), GameConfig.F("BATTLE_CARD_EFFECT_SCALE_TIME")));
        }
        endTimeNode += (GameConfig.F("BATTLE_CARD_INTERVAL") +
            GameConfig.F("BATTLE_CARD_EFFECT_SCALE_TIME") * 2.0f +
            GameConfig.F("BATTLE_CARD_EFFECT_HIGHLIGHT_PAUSE") * 2.0f +
            GameConfig.F("BATTLE_CARD_EFFECT_LABEL_SCALE_TIME") +
            GameConfig.F("BATTLE_CARD_EFFECT_LABEL_PAUSE")) * Math.Max(amods.Count, dmods.Count);

        // Do damage
        s.InsertCallback(endTimeNode, () =>
        {
            GameObject defSlashSFX = Instantiate(Resources.Load("prefabs/SlashEffect")) as GameObject;
            defSlashSFX.transform.position = defenderAnchor;
            Destroy(defSlashSFX, GameConfig.F("BATTLE_SLASH_TIME"));
            GameObject atkSlashSFX = Instantiate(Resources.Load("prefabs/SlashEffect")) as GameObject;
            atkSlashSFX.transform.position = attackerAnchor;
            Destroy(atkSlashSFX, GameConfig.F("BATTLE_SLASH_TIME"));
        });
        endTimeNode += GameConfig.F("BATTLE_SLASH_TIME");

        endTimeNode += GameConfig.F("BATTLE_AFTER_DAMAGE_INTV");

        // Clean up
        s.InsertCallback(endTimeNode, () =>
        {
            // Move deads to grave
            for (int i = 0; i < akilled.Count; i++)
            {
                CardObjectBehaviour cob = CardObjectBehaviour.GetCOB(akilled.ElementAt(i).guid);
                cob.Owner.CSob[acs].MoveToGrave(s, endTimeNode + GameConfig.F("BATTLE_CARD_INTERVAL") * i, cob.gameObject);
            }
            for (int i = 0; i < dkilled.Count; i++)
            {
                CardObjectBehaviour cob = CardObjectBehaviour.GetCOB(dkilled.ElementAt(i).guid);
                cob.Owner.CSob[dcs].MoveToGrave(s, endTimeNode + GameConfig.F("BATTLE_CARD_INTERVAL") * i, cob.gameObject);
            }
            // Update player health/slot power
            if (aname.Equals(LocalPlayer.PlayerName))
            {
                LocalPlayer.UpdateCardSlotPower(acs, aaap);
                EnemyPlayer.UpdateCardSlotPower(dcs, daap);
                LocalPlayer.UpdateHealth(ahp);
                EnemyPlayer.UpdateHealth(dhp);
            }
            else if (dname.Equals(LocalPlayer.PlayerName))
            {
                LocalPlayer.UpdateCardSlotPower(dcs, daap);
                EnemyPlayer.UpdateCardSlotPower(acs, aaap);
                LocalPlayer.UpdateHealth(dhp);
                EnemyPlayer.UpdateHealth(ahp);
            }
            for (int i = 0; i < atouched.Count; i++)
            {
                Card item = atouched.ElementAt(i);
                CardObjectBehaviour cob = CardObjectBehaviour.GetCOB(item.guid);
                cob.UpdatePower(item.power);
            }
            for (int i = 0; i < dtouched.Count; i++)
            {
                Card item = dtouched.ElementAt(i);
                CardObjectBehaviour cob = CardObjectBehaviour.GetCOB(item.guid);
                cob.UpdatePower(item.power);
            }
        });

        s.OnComplete(() =>
        {
            if (aname.Equals(LocalPlayer.PlayerName))
            {
                LocalPlayer.CSob[acs].RerenderCards();
                EnemyPlayer.CSob[dcs].RerenderCards();
            }
            else if (dname.Equals(LocalPlayer.PlayerName))
            {
                EnemyPlayer.CSob[acs].RerenderCards();
                LocalPlayer.CSob[dcs].RerenderCards();
            }
            SetUIState(UIState.ACTION);
        });

    }

    internal static void SelectSlotCallback(object[] data)
    {
        // Requesting a slot selection
        PlayerObjectBehaviour p = ForPlayer((string)data[0]);
        if (SelectTargetObject != null)
        {
            Destroy(SelectTargetObject);
            SelectTargetObject = null;
        }
        SelectTargetObject = Instantiate(Resources.Load("prefabs/Target")) as GameObject;
        SelectTargetCard sel = SelectTargetObject.GetComponent<SelectTargetCard>();
        CardObjectBehaviour cob = CardObjectBehaviour.GetCOB((int)data[1]);
        sel.From = cob.gameObject;
        SetUIState(UIState.SLOT_TARGETING);
    }

    internal static void UpdateSlotPowerCallback(object[] data)
    {
        PlayerObjectBehaviour p = ForPlayer((string)data[0]);
        p.UpdateCardSlotPower((int)data[1], (int)data[2]);
    }

    internal static void UpdateCardPowerCallback(object[] data)
    {
        CardObjectBehaviour cob = CardObjectBehaviour.GetCOB((int)data[0]);
        cob.UpdatePower((int)data[1]);
    }

    internal static void RemoveBuffCallback(object[] data)
    {
        CardObjectBehaviour cob = CardObjectBehaviour.GetCOB((int)data[0]);
        BuffBehaviour bob = BuffBehaviour.GetBOB((int)data[1]);
        cob.RemoveBuff(bob.gameObject);
    }

    internal static void AddBuffCallback(object[] data)
    {
        int cardGuid = (int)data[0];
        int buffGuid = (int)data[1];
        string iconPath = (string)data[2];
        BuffBehaviour.Create(CardObjectBehaviour.GetCOB(cardGuid), buffGuid, iconPath);
    }

    internal static void SelectDoneCallback(object[] data)
    {
        PlayerObjectBehaviour p = ForPlayer((string)data[0]);
        if (SelectTargetObject != null)
        {
            Destroy(SelectTargetObject);
            SelectTargetObject = null;
        }
        SetUIState(UIState.ACTION);
    }

    internal static void SelectTargetCallback(object[] data)
    {
        // Requesting a card selection
        PlayerObjectBehaviour p = ForPlayer((string)data[0]);
        if (SelectTargetObject != null)
        {
            Destroy(SelectTargetObject);
            SelectTargetObject = null;
        }
        SelectTargetObject = Instantiate(Resources.Load("prefabs/Target")) as GameObject;
        SelectTargetCard sel = SelectTargetObject.GetComponent<SelectTargetCard>();
        CardObjectBehaviour cob = CardObjectBehaviour.GetCOB((int)data[1]);
        sel.From = cob.gameObject;
        SetUIState(UIState.TARGETING);
    }

    internal static void PlayCardSlotCallback(object[] data)
    {
        PlayerObjectBehaviour p = ForPlayer((string)data[0]);
        CardObjectBehaviour cob = CardObjectBehaviour.GetCOB((int)data[1]);
        int slot = (int)data[2];
        int power = (int)data[3];
        p.CSob[slot].AddCard(cob.gameObject);
        p.UpdateCardSlotPower(slot, power);
    }

    internal static void DiscardCardCallback(object[] data)
    {
        PlayerObjectBehaviour p = ForPlayer((string)data[0]);
        CardObjectBehaviour cob = CardObjectBehaviour.GetCOB((int)data[1]);
        cob.AddDoTweens(() =>
        {
            cob.OriginPos = p.Grave.transform.position;
            cob.SetTweening(true);
            Sequence s = DOTween.Sequence();
            s.Insert(0f, cob.gameObject.transform.DOMove(cob.OriginPos, GameConfig.F("BATTLE_CARD_DEATH_FLY_TIME")));
            s.Insert(0f, cob.gameObject.transform.DOScale(1.0f, GameConfig.F("BATTLE_CARD_DEATH_FLY_TIME")));
            cob.State = CardState.GRAVE;
            s.OnComplete(() =>
            {
                cob.SetTweening(false);
            });
        });
    }
}
