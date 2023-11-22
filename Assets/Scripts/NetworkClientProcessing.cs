using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class NetworkClientProcessing
{

    #region Send and Receive Data Functions
    static public void ReceivedMessageFromServer(string msg, TransportPipeline pipeline)
    {
        Debug.Log("Network msg received =  " + msg + ", from pipeline = " + pipeline);
        string[] csv = msg.Split(',');
        int signifier = int.Parse(csv[0]);

        //if()
        switch (signifier)
        {
            case ServerToClientSignifiers.SettingMainPlayer:
                gameLogic.SetAsMainPlayer();
                break;
            case ServerToClientSignifiers.OtherBallonSpawned:
                string[] pos = csv[1].Split('_');
                Vector2 BallonPorcentage = new Vector2(float.Parse(pos[0]), float.Parse(pos[1]));
                gameLogic.SpawnNewBalloon(BallonPorcentage);
                break;
            case ServerToClientSignifiers.OtherBallonPopped:
                gameLogic.DeleteBallon(int.Parse(csv[1]));
                break;
            case ServerToClientSignifiers.GettingScreen:
                gameLogic.GetAllBallons(int.Parse(csv[1]));
                break;
            case ServerToClientSignifiers.SettingScreen:
                gameLogic.SetAllBallons(csv[1]);
                break;

        }

        //gameLogic.DoSomething();

    }

    static public void SendMessageToServer(string msg, TransportPipeline pipeline)
    {
        networkClient.SendMessageToServer(msg, pipeline);
    }

    #endregion

    #region Connection Related Functions and Events
    static public void ConnectionEvent()
    {
        Debug.Log("Network Connection Event!");
        //gameLogic.ToogleIsMainPlayer();
    }
    static public void DisconnectionEvent()
    {
        Debug.Log("Network Disconnection Event!");
        //gameLogic.ToogleIsMainPlayer();

    }
    static public bool IsConnectedToServer()
    {
        return networkClient.IsConnected();
    }
    static public void ConnectToServer()
    {
        networkClient.Connect();
    }
    static public void DisconnectFromServer()
    {
        networkClient.Disconnect();
    }

    #endregion

    #region Setup
    static NetworkClient networkClient;
    static GameLogic gameLogic;

    static public void SetNetworkedClient(NetworkClient NetworkClient)
    {
        networkClient = NetworkClient;
    }
    static public NetworkClient GetNetworkedClient()
    {
        return networkClient;
    }
    static public void SetGameLogic(GameLogic GameLogic)
    {
        gameLogic = GameLogic;
    }

    static public GameLogic GetGameLogic()
    {
        return gameLogic;
    }

    #endregion

}

#region Protocol Signifiers
static public class ClientToServerSignifiers
{
    public const int BallonSpawned = 0;
    public const int BallonPopped = 1;
    public const int SendingScreen = 2;
}

static public class ServerToClientSignifiers
{
    public const int SettingMainPlayer = -1;
    public const int OtherBallonSpawned = 0;
    public const int OtherBallonPopped = 1;
    public const int GettingScreen = 2;
    public const int SettingScreen = 3;
}

#endregion

