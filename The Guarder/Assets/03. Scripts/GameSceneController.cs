using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneController : MonoBehaviour
{
    [SerializeField]
    private int GameLevel = 1;
    public GameObject player;
    public GameObject monsterManager;

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.R))
        {
            GameReset();
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            MoveOnToNextStep();
        }
    }
    private void GameReset()
    {
        player.GetComponent<PlayerMoveInGrid>().ResetPlayerPosition();
        monsterManager.GetComponent<MonsterManager>().ResetMonsterMap();
        monsterManager.GetComponent<MonsterManager>().ResetNextWave();
    }
    private void MoveOnToNextStep()
    {
        monsterManager.GetComponent<MonsterManager>().MoveOnToNextStep();
    }

    public int GetLevel()
    {
        return GameLevel;
    }
    public void SetGameLevel(int gameLevel)
    {
        GameLevel = gameLevel;
    }

}
