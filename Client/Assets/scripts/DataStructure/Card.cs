public class Card
{
    public int guid;
    public int id;
    public string name;
    public string color;
    public string ctype;
    public int power;
    public string desc;
    public int cost;

    public Card(int guid, int id, string name, string color, string ctype, int power, string desc, int cost)
    {
        this.guid = guid;
        this.id = id;
        this.name = name;
        this.color = color;
        this.ctype = ctype;
        this.power = power;
        this.desc = desc;
        this.cost = cost;
    }
}
