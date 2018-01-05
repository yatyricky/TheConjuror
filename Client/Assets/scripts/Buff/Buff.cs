using UnityEngine;

public class Buff
{
    public class BuffType
    {
        public static int NO_BATTLE = 100;
        public static int WEAKEN = 200;
    }

    protected string iconPath;
    protected int type;
    protected GameObject bo;
    protected Player caster;

    public GameObject BO { get { return bo; } internal set { bo = value; } }
    public string IconPath { get { return iconPath; } }
    public int Type { get { return type; } }
    public Player Caster { get {return caster; } internal set {caster = value; } }

    internal virtual bool ShouldRemoveEndTurn()
    {
        return false;
    }
}