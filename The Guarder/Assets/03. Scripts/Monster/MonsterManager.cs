using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    private int[,] nowMonsterMap = new int[8, 8];
    private int[,] nextMonsterMap = new int[8, 8];
    private bool[] nextWave = new bool[28];
    private int numberOfMonsters = 0;

    private int gameLevel;

    public GameObject gameSceneManager;
    public GameObject nextWaveObject;

    void Start()
    {
        ResetMonsterMap();
        ResetNextWave();
    }

    public void MoveOnToNextStep()
    {
        MoveMonsters();
        CreateMonsters();
        CreateNextWave();
    }

    public void ResetMonsterMap()
    {
        numberOfMonsters = 0;
        for (int i = 0; i < 8; ++i)
        {
            for (int j = 0; j < 8; ++j)
            {
                nowMonsterMap[i, j] = 0;
                nextMonsterMap[i, j] = 0;
            }
        }
    }
    public void ResetNextWave()
    {
        for (int i = 0; i < 28; ++i)
        {
            nextWave[i] = false;
            nextWaveObject.transform.GetChild(i).gameObject.SetActive(nextWave[i]);
        }
        CreateNextWave();
    }
    private void MoveMonsters()
    {

    }
    private void CreateMonsters()
    {

    }

    private void CreateNextWave()
    {
        gameLevel = gameSceneManager.GetComponent<GameSceneController>().GetLevel();
        int maxNumberOfNextWaveMonster = gameLevel;

        while(maxNumberOfNextWaveMonster > 0)
        {
            int random = Random.Range(0, 28);
            if (nextWave[random] == false)
            {
                nextWave[random] = true;
                maxNumberOfNextWaveMonster--;
                Debug.Log("now num = " + random);
            }
        }

        for(int i = 0; i < 28; ++i)
        {
            nextWaveObject.transform.GetChild(i).gameObject.SetActive(nextWave[i]);
        }
    }

}
