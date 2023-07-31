using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneController : MonoBehaviour
{
    public GameObject player;

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.R))
        {
            GameReset();
        }
    }
    private void GameReset()
    {
        player.GetComponent<PlayerMoveInGrid>().ResetPlayerPosition();
    }
}
