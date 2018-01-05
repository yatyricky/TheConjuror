public class Colors
{
    public static string WHITE = "white";
    public static string BLUE = "blue";
    public static string ALL = "all";
    public static string GREEN = "green";
    public static string RED = "red";
}

public class CardTypes
{
    public static string CREATURE = "creature";
    public static string SPELL = "spell";
    public static string ENCHANTMENT = "enchantment";
}

public class UIState
{
    public static int ACTION = 0;
    public static int BATTLING = 100;
    public static int TARGETING = 150;
}

public class CardState
{
    public static int DEFAULT = 0;
    public static int DECK = 50;
    public static int HAND = 100;
    public static int SLOT = 200;
    public static int GRAVE = 300;
    public static int EFFECT = 400;
}