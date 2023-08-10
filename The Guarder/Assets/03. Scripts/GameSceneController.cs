using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneController : MonoBehaviour
{
    [SerializeField]
    private int gameLevel = 1;
    [SerializeField]
    private int gameScore = 0;
    public GameObject player;
    public GameObject monsterManager;
    private bool isGameOver = false;

    public GameObject GameOverText;

    public Text ScoreText;
    public Text LevelText;

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
    public void GameReset()
    {
        isGameOver = false;
        ScoreReset();
        SetGameLevel(1);
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
    public void DamagedMonsterMoveOnToNextStep()
    {
        if (isGameOver == false)
        {
            monsterManager.GetComponent<MonsterManager>().DamagedMonsterMoveOnToNextStep();
        }
    }


    public int GetLevel()
    {
        return gameLevel;
    }
    public void SetGameLevel(int gameLevel)
    {
        this.gameLevel = gameLevel;
        LevelText.text = "Level : " + gameLevel;
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
    public void LevelDown()
    {
        if (gameLevel - 1 > 1)
        {
            SetGameLevel(gameLevel - 1);
        }
    }

    public void GameOver()
    {
        isGameOver = true;
        GameOverText.SetActive(true);
        player.GetComponent<PlayerContoller>().IsGameOver(isGameOver);
        player.GetComponent<PlayerMoveInGrid>().IsGameOver(isGameOver);
    }
    public void ScoreReset()
    {
        gameScore = 0;
        ScoreText.text = "Score : " + gameScore;
    }
    public void AddScore(int _score)
    {
        gameScore += _score;
        ScoreText.text = "Score : " + gameScore;
        if (gameScore % 3000 == 0)
        {
            LevelUp();
        }
    }
}
