public class CrossScenePayload
{
    public delegate void ProcessData(object[] data);
    private ProcessData Callback;
    private object[] Data;

    public CrossScenePayload(ProcessData callback, params object[] data)
    {
        Callback = callback;
        Data = data;
    }

    public void Fire()
    {
        Callback(Data);
    }
}
