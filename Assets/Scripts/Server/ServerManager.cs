using System.Collections;
using System.Collections.Generic;
using System.IO;
using Lidgren.Network;
using UnityEngine;
using Windows;

public class ServerManager : DarckNet.DarckMonoBehaviour
{

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        Config conf = LoadConfig();
        StartServer(conf.Ip, conf.MaxPlayer, conf.Port);
    }

    void Update()
    {

    }

    public void StartServer(string ip,int maxplayers, int port)
    {
        DarckNet.Network.Create(ip, port, maxplayers);
    }

    public override void OnPlayerConnect(NetConnection Peer)
    {
        base.OnPlayerConnect(Peer);
    }

    public override void OnPlayerDisconnect(NetConnection Peer)
    {
        base.OnPlayerDisconnect(Peer);
    }

    public override void OnServerStart()
    {
        System.Console.ForegroundColor = System.ConsoleColor.Yellow;
        System.Console.WriteLine("Starting Server...");

        Game.GameManager.WorldName = "ServerMap";

        SaveWorld.CreateDerectorys();

        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Map");
        base.OnServerStart();
    }

    void OnLevelWasLoaded(int level)
    {
        System.Console.ForegroundColor = System.ConsoleColor.Yellow;
        System.Console.WriteLine("Server are Ready, You can Join Now (:");
    }

    Config LoadConfig()
    {
        Config conf = new Config();
        IniFile file = new IniFile("ServerConfig", Path.GetFullPath("./"));

        conf.Ip = file.GetString("Ip");
        conf.Port = file.GetInt("Port");
        conf.Password = file.GetString("Password");
        conf.MaxPlayer = file.GetInt("MaxPlayer");
        conf.Dedicated = file.GetBool("Dedicated");

        System.Console.ForegroundColor = System.ConsoleColor.Yellow;
        System.Console.WriteLine("Server Ip: " + conf.Ip);
        System.Console.ForegroundColor = System.ConsoleColor.Yellow;
        System.Console.WriteLine("Server Port: " + conf.Port);
        System.Console.ForegroundColor = System.ConsoleColor.Yellow;
        System.Console.WriteLine("Server Password: " + conf.Password);
        System.Console.ForegroundColor = System.ConsoleColor.Yellow;
        System.Console.WriteLine("Server MaxPlayer: " + conf.MaxPlayer);
        System.Console.ForegroundColor = System.ConsoleColor.Yellow;
        System.Console.WriteLine("Server Dedicated: " + conf.Dedicated);

        file.Save("ServerConfig", Path.GetFullPath("./"));
        return conf;
    }
}

struct Config
{
    public string Ip;
    public int Port;
    public string Password;
    public int MaxPlayer;
    public bool Dedicated;
}