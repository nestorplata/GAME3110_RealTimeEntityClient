using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleClick : MonoBehaviour
{
    public void OnMouseDown()
    {
        GameLogic gameLogic= NetworkClientProcessing.GetGameLogic();

        gameLogic.SendMessageToServer(ClientToServerSignifiers.BallonPopped,
            gameLogic.GetBallonID(gameObject)+"");

        gameLogic.DeleteBallon(gameObject);
    }
}
