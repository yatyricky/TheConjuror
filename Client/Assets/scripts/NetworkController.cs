using UnityEngine;
using System.Collections;
using SocketIO;

public class NetworkController : MonoBehaviour
{
    public static NetworkController instance;

    private SocketIOComponent socket;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();

        socket.On("login_success", LoginSuccess);
    }

    public void LoginSuccess(SocketIOEvent e)
    {
        Debug.Log("Connected to server");
    }

    public void LoginPlayer(string name)
    {
        JSONObject data = JSONObject.Create();
        data.AddField("name", name);
        socket.Emit("login", data);
    }

}
