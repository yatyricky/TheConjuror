public abstract class GameAction
{
    public delegate void UpdateUICallBack(Payload payload);
    public abstract void Fire(UpdateUICallBack callBack);

    public class Payload
    {
        public string ActionName;
        public object payload;
    }

}
