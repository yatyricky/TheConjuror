using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameConfig
{
    private static GameConfig instance = null;
    private Dictionary<string, string> table;

    private static GameConfig Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameConfig();
            }
            return instance;
        }
    }

    public static float F(string key)
    {
        string val;
        if (Instance.table.TryGetValue(key, out val))
        {
            return float.Parse(val);
        } else
        {
            throw new Exception("No such config: " + key);
        }
    }

    public static int I(string key)
    {
        string val;
        if (Instance.table.TryGetValue(key, out val))
        {
            return int.Parse(val);
        }
        else
        {
            throw new Exception("No such config: " + key);
        }
    }

    public static string S(string key)
    {
        string val;
        if (Instance.table.TryGetValue(key, out val))
        {
            return val;
        }
        else
        {
            throw new Exception("No such config: " + key);
        }
    }

    private GameConfig()
    {
        table = new Dictionary<string, string>();
        StringReader reader = new StringReader(Resources.Load<TextAsset>("data/config").text);
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            string[] tokens = line.Split('=');
            if (tokens.Length == 2)
            {
                table.Add(tokens[0], tokens[1]);
            }
        }

    }

}
