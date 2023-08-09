using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneController : MonoBehaviour
{
    [SerializeField]
    private int gameLevel = 1;
    public GameObject player;
    public GameObject monsterManager;
    private bool isGameOver = false;

    public GameObject GameOverText;

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.R))
        {
            GameReset();
        }
        if (Input.GetKeyUp(KeyCode.Z))
        {
            MoveOnToNextStep();
        }
        if (Input.GetKeyUp(KeyCode.X))
        {
            LevelUp();
        }
    }
    private void GameReset()
    {
        isGameOver = false;
        GameOverText.SetActive(false);
        player.GetComponent<PlayerContoller>().IsGameOver(isGameOver);
        player.GetComponent<PlayerMoveInGrid>().IsGameOver(isGameOver);
        player.GetComponent<PlayerMoveInGrid>().ResetPlayerPosition();
        monsterManager.GetComponent<MonsterManager>().ResetMonsterManager();
    }
    public void MoveOnToNextStep()
    {
        if (isGameOver == false)
        {
            monsterManager.GetComponent<MonsterManager>().MoveOnToNextStep();
        }
    }

    public int GetLevel()
    {
        return gameLevel;
    }
    public void SetGameLevel(int gameLevel)
    {
        this.gameLevel = gameLevel;
        monsterManager.GetComponent<MonsterManager>().ApplyGameLevel();
    }

    public int GetPlayerGridPosition()
    {
        // X �� 10�� �ڸ��� Y �� 1�� �ڸ���
        return player.GetComponent<PlayerMoveInGrid>().GetPlayerGridPosition();
    }
    public void LevelUp()
    {
        SetGameLevel(gameLevel + 1);
    }
    public void GameOver()
    {
        isGameOver = true;
        GameOverText.SetActive(true);
        player.GetComponent<PlayerContoller>().IsGameOver(isGameOver);
        player.GetComponent<PlayerMoveInGrid>().IsGameOver(isGameOver);
    }
}
