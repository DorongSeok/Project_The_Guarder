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
        // X 는 10의 자리로 Y 는 1의 자리로
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
