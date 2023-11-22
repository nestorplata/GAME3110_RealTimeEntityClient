using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    [SerializeField] List<GameObject> BalloonList = new List<GameObject>();
    [SerializeField] float durationUntilNextBalloon =5f;
    bool isMainPlayer = false;

    Sprite circleTexture;

    float ScreenWidth = (float)Screen.width;
    float ScreenHeight = (float)Screen.height;

    void Start()
    {
        NetworkClientProcessing.SetGameLogic(this);
    }

    void Update()
    {
        if(isMainPlayer)
        {
            durationUntilNextBalloon -= Time.deltaTime;

            if (durationUntilNextBalloon < 0)
            {
                durationUntilNextBalloon = 5f;

                float screenPositionXPercent = Random.Range(0.0f, 1.0f);
                float screenPositionYPercent = Random.Range(0.0f, 1.0f);
                Vector2 ScreenPorcentage = new Vector2(screenPositionXPercent, screenPositionYPercent);

                SendMessageToServer(ClientToServerSignifiers.BallonSpawned, ScreenPorcentage);
                SpawnNewBalloon(ScreenPorcentage);
            }
        }
    }

    public void SetAsMainPlayer()
    {
        isMainPlayer = true;
        SpawnNewBalloon(new Vector2(-1f, -1f));
    }

    public void SpawnNewBalloon(Vector2 ScreenPorcentage)
    {
        if (circleTexture == null)
            circleTexture = Resources.Load<Sprite>("Circle");

        GameObject balloon = new GameObject("Balloon");

        balloon.AddComponent<SpriteRenderer>();
        balloon.GetComponent<SpriteRenderer>().sprite = circleTexture;
        balloon.AddComponent<CircleClick>();
        balloon.AddComponent<CircleCollider2D>();

        balloon.transform.position = PorcentageToScreenPosition(ScreenPorcentage);
        BalloonList.Add(balloon);

    }

    public void DeleteBallon(int ID)
    {
        DeleteBallon(BalloonList[ID]);
    }
    public void DeleteBallon(GameObject ballon)
    {
 
        Debug.Log(GetBallonID(ballon) + ", Ballon Deleated");
        BalloonList.RemoveAt(GetBallonID(ballon));
        Destroy(ballon);

    }


    public void GetAllBallons(int ID)
    {
        string ballonPositions="";
        Vector2 BallonPorcentage;
        foreach (GameObject balloon in BalloonList)
        {
            BallonPorcentage = Vector2.zero;

            BallonPorcentage = ScreenPositionToPorcentage(balloon.transform.position);
            ballonPositions += BallonPorcentage.x + "_" + BallonPorcentage.y + "&";

        }
        SendMessageToServer(ClientToServerSignifiers.SendingScreen, ID + "," + ballonPositions);


    }
    public void SetAllBallons(string ballonPositions)
    {
        string[] PositionsDecompressed = ballonPositions.Split('&');
        foreach (string Position in PositionsDecompressed)
        {
            string[] pos = Position.Split("_");
            if (pos[0]!="")
            {
                Vector2 Porcentage = new Vector2(float.Parse(pos[0]), float.Parse(pos[1]));
                SpawnNewBalloon(Porcentage);
            }
        }
    }


    public void SendMessageToServer(int signifier, Vector2 pos)
    {
        NetworkClientProcessing.SendMessageToServer(signifier + "," + pos.x + "_" + pos.y, TransportPipeline.ReliableAndInOrder);
    }

    public void SendMessageToServer(int signifier, string mesagge)
    {
        NetworkClientProcessing.SendMessageToServer(signifier + "," + mesagge, TransportPipeline.ReliableAndInOrder);
    }

    public Vector2 PorcentageToScreenPosition(Vector2 Porcentage)
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(Porcentage.x * ScreenWidth, Porcentage.y * ScreenHeight, 0));
    }

    public Vector2 ScreenPositionToPorcentage(Vector2 screenPosition)
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(new Vector3(screenPosition.x, screenPosition.y, 0));
        return new Vector2(pos.x / ScreenWidth, pos.y / ScreenHeight);
    }

    public int GetBallonID(GameObject thing)
    {
        for (int i = 0; i<BalloonList.Count; i++)
        {
            if (BalloonList[i].transform.position == thing.transform.position)
            {
                return i;
            }
        }
        return -1;
    }






}
