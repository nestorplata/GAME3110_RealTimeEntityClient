using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleClick : MonoBehaviour
{
    void Start()
    {
        
    }
    void Update()
    {
        
    }
    public void OnMouseDown()
    {
        if (NetworkClientProcessing.GetGameLogic().GetIsConnected())
        {
            NetworkClientProcessing.GetGameLogic().SendMessageToServer(ClientToServerSignifiers.BallonPopped,
                NetworkClientProcessing.GetGameLogic().ScreenPositionToPorcentage(transform.position));
            Destroy(gameObject);
        }

    }
}
