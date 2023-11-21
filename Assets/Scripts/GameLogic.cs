using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    List<GameObject> Balloons;
    float durationUntilNextBalloon =2f;
    bool isConnected;
    Sprite circleTexture;
    
    void Start()
    {
        NetworkClientProcessing.SetGameLogic(this);
    }

    void Update()
    {
        if(isConnected)
        {
            durationUntilNextBalloon -= Time.deltaTime;

            if (durationUntilNextBalloon < 0)
            {
                durationUntilNextBalloon = 1f;

                float screenPositionXPercent = Random.Range(0.0f, 1.0f);
                float screenPositionYPercent = Random.Range(0.0f, 1.0f);
                Vector2 Porcentage = new Vector2(screenPositionXPercent, screenPositionYPercent);

                SendMessageToServer(ClientToServerSignifiers.BallonSpawned, Porcentage);

                SpawnNewBalloon(PorcentageToScreenPosition(Porcentage));
            }
        }
    }

    public void SpawnNewBalloon(Vector2 ScreenPosition)
    {
        if (circleTexture == null)
            circleTexture = Resources.Load<Sprite>("Circle");

        GameObject balloon = new GameObject("Balloon");

        balloon.AddComponent<SpriteRenderer>();
        balloon.GetComponent<SpriteRenderer>().sprite = circleTexture;
        balloon.AddComponent<CircleClick>();
        balloon.AddComponent<CircleCollider2D>();

        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(ScreenPosition.x, ScreenPosition.y, 0));
        pos.z = 0;
        balloon.transform.position = pos;
        Balloons.Add(balloon);

        //go.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, -Camera.main.transform.position.z));
    }

    public void DeleteBallon(Vector2 ScreenPosition)
    {
        Vector3 CompletePosition = new Vector3(ScreenPosition.x, ScreenPosition.y, 0);
        foreach(GameObject balloon in Balloons)
        {
            if(balloon.transform.position== CompletePosition)
            {
                Destroy(balloon);
                break;
            }
        }
    }

    public void GetAllBallons(int ID)
    {
        string ballonPositions = Balloons.Count+",";
        Vector2 BallonPorcentage = Vector2.zero;
        foreach (GameObject balloon in Balloons)
        {
            BallonPorcentage = ScreenPositionToPorcentage(balloon.transform.position);
            ballonPositions += BallonPorcentage.x + "_" + BallonPorcentage.x + "&";
        }
        NetworkClientProcessing.SendMessageToServer(ClientToServerSignifiers.SendingScreen + "," +ID+ "," + ballonPositions, TransportPipeline.ReliableAndInOrder);
    }
    public void SetAllBallons(string ballonPositions)
    {
        string[] PositionsDecompressed = ballonPositions.Split('&');
        foreach (string Position in PositionsDecompressed)
        {
            string[] pos = Position.Split("_");
            SpawnNewBalloon(PorcentageToScreenPosition(float.Parse(pos[0]), float.Parse(pos[1])));
        }
    }



    public void SendMessageToServer(int signifier, Vector2 pos)
    {
        NetworkClientProcessing.SendMessageToServer(signifier + "," + pos.x + "_" + pos.y, TransportPipeline.ReliableAndInOrder);
    }

    public Vector2 ScreenPositionToPorcentage(Vector2 screenPosition)
    {
        return new Vector2(screenPosition.x / (float)Screen.width, screenPosition.y / (float)Screen.height);
    }
    public Vector2 ScreenPositionToPorcentage(float x, float y)
    {
        return new Vector2(x / (float)Screen.width, y / (float)Screen.height);
    }

    public Vector2 PorcentageToScreenPosition(Vector2 Porcentage)
    {
        return new Vector2(Porcentage.x * (float)Screen.width, Porcentage.y * (float)Screen.height);
    }

    public Vector2 PorcentageToScreenPosition(float x, float y)
    {
        return new Vector2(x * (float)Screen.width, y * (float)Screen.height);
    }

    public void ToogleIsConnected()
    {
        isConnected =!isConnected;
    }
    public bool GetIsConnected()
    {
        return isConnected;
    }
}
