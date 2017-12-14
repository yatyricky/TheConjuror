public class Card
{
    public int id;
    public string name;
    public string color;
    public string type;
    public int cost;
    public string description;

    public int power; // creature only

    public Card(int id, string name, string color, string type, int cost, string description, int power)
    {
        this.id = id;
        this.name = name;
        this.color = color;
        this.type = type;
        this.cost = cost;
        this.description = description;
        this.power = power;
    }

}