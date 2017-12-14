public abstract class GameAction
{
    public delegate void UpdateUICallBack(object payload);
    public abstract void Fire(UpdateUICallBack callBack);
}
