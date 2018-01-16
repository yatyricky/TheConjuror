using DG.Tweening;
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
    private static SelectTargetCard currentSelector;

    public GameObject PlayerA;
    public GameObject PlayerB;

    private void Awake()
    {
        currentSelector = null;
        uiState = UIState.ACTION;

        LocalPlayer = PlayerA.GetComponent<PlayerObjectBehaviour>();
        EnemyPlayer = PlayerB.GetComponent<PlayerObjectBehaviour>();
    }

    private void Update()
    {
        while (CrossScenePayloads.Count > 0)
        {
            CrossScenePayloads.Dequeue().Fire();
        }
    }

    internal static void SetCurrentSelector(SelectTargetCard selectTargetCard)
    {
        currentSelector = selectTargetCard;
    }

    internal static void SetUIState(int state)
    {
        uiState = state;
    }

    internal static int GetUIState()
    {
        return uiState;
    }

    internal static void SelectTarget(GameObject cardObject)
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

    internal static void PlayerPlayedACard(object[] data)
    {
        string who = (string)data[0];
        PlayerObjectBehaviour forPlayer = LocalPlayer;
        if (who.Equals(EnemyPlayer.PlayerName))
        {
            forPlayer = EnemyPlayer;
        }
        int guid = (int)data[1];
        int dest = (int)data[2];
        int mana = (int)data[3];
        GameObject co = CardObjectBehaviour.GetCOB(guid).gameObject;
        DragHandCard dhc = co.GetComponent<DragHandCard>();
        if (dest == -1)
        {
            forPlayer.Hob.PlayCardFail(co);
        }
        else if (dest == 99)
        {
            // Played a spell card
            forPlayer.UpdateMana(mana);
            forPlayer.Hob.RemoveCard(co);
            forPlayer.DiscardCard(co);
            dhc.CanDrag = false;
        }
        else
        {
            // played a creture/enchantment card
            int slotPower = (int)data[4];
            forPlayer.UpdateCardSlotPower(dest, slotPower);
            forPlayer.UpdateMana(mana);
            forPlayer.Hob.RemoveCard(co);
            forPlayer.CSob[dest].AddCard(co);
            dhc.CanDrag = false;
        }
    }

    internal static void PlayerPlayedACardFailed(object[] data)
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
        GameObject attackerGO = GameObject.FindGameObjectWithTag("BattleAttacker");
        GameObject defenderGO = GameObject.FindGameObjectWithTag("BattleDefender");
        Vector3 attackerAnchor = attackerGO.transform.position;
        attackerAnchor.z = GameConfig.F("BATTLE_Z_INDEX");
        Vector3 defenderAnchor = defenderGO.transform.position;
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
            GameObject buffLabel = attackerGO.transform.Find("Buff").gameObject;
            GameObject debuffLabel = attackerGO.transform.Find("Debuff").gameObject;
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
            GameObject buffLabel = defenderGO.transform.Find("Buff").gameObject;
            GameObject debuffLabel = defenderGO.transform.Find("Debuff").gameObject;
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

        // Clear up
        s.InsertCallback(endTimeNode, () =>
        {
            if (aname.Equals(LocalPlayer.PlayerName))
            {
                LocalPlayer.UpdateCardSlotPower(acs, aaap);
                EnemyPlayer.UpdateCardSlotPower(dcs, daap);
                LocalPlayer.UpdateHealth(ahp);
                EnemyPlayer.UpdateHealth(dhp);

                // Move deads to grave
                for (int i = 0; i < akilled.Count; i++)
                {
                    LocalPlayer.CSob[acs].MoveToGrave(s, endTimeNode + GameConfig.F("BATTLE_CARD_INTERVAL") * i, CardObjectBehaviour.GetCOB(akilled.ElementAt(i).guid).gameObject);
                }
                for (int i = 0; i < dkilled.Count; i++)
                {
                    EnemyPlayer.CSob[dcs].MoveToGrave(s, endTimeNode + GameConfig.F("BATTLE_CARD_INTERVAL") * i, CardObjectBehaviour.GetCOB(dkilled.ElementAt(i).guid).gameObject);
                }
            }
            else if (dname.Equals(LocalPlayer.PlayerName))
            {
                LocalPlayer.UpdateCardSlotPower(dcs, daap);
                EnemyPlayer.UpdateCardSlotPower(acs, aaap);
                LocalPlayer.UpdateHealth(dhp);
                EnemyPlayer.UpdateHealth(ahp);

                for (int i = 0; i < akilled.Count; i++)
                {
                    EnemyPlayer.CSob[acs].MoveToGrave(s, endTimeNode + GameConfig.F("BATTLE_CARD_INTERVAL") * i, CardObjectBehaviour.GetCOB(akilled.ElementAt(i).guid).gameObject);
                }
                for (int i = 0; i < dkilled.Count; i++)
                {
                    LocalPlayer.CSob[dcs].MoveToGrave(s, endTimeNode + GameConfig.F("BATTLE_CARD_INTERVAL") * i, CardObjectBehaviour.GetCOB(dkilled.ElementAt(i).guid).gameObject);
                }
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
}
