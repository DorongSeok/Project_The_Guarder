using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    private int[,] nowMonsterMap = new int[8, 8];
    private int[,] nextMonsterMap = new int[8, 8];
    private bool[] nextWave = new bool[28];
    private int[,] monsterStartGridPosition = new int[28,2];
    private int numberOfMonsters = 0;

    private int gameLevel;

    public GameObject GameSceneManager;
    public GameObject NextWaveObject;
    public GameObject MonsterPrefeb;

    void Start()
    {
        SetMonsterStartGridPosition();
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
            NextWaveObject.transform.GetChild(i).gameObject.SetActive(nextWave[i]);
        }
        CreateNextWave();
    }
    private void MoveMonsters()
    {
        if (numberOfMonsters > 0)
        {
            for (int i = 1; i <= numberOfMonsters; ++i)
            {
                this.transform.GetChild(i).gameObject.GetComponent<MonsterMoveInGrid>().Move();
            }
        }
    }
    private void CreateMonsters()
    {
        for (int i = 0; i < 28; ++i)
        {
            if (nextWave[i] == true)
            {
                nextWave[i] = false;

                int moveDirectionNum = 0;
                if(0 <= i && i <= 6)
                {
                    moveDirectionNum = 3;
                }
                else if (7 <= i && i <= 13)
                {
                    moveDirectionNum = 4;
                }
                else if (14 <= i && i <= 20)
                {
                    moveDirectionNum = 1;
                }
                else if (21 <= i && i <= 27)
                {
                    moveDirectionNum = 2;
                }

                GameObject monster = Instantiate(MonsterPrefeb) as GameObject;
                monster.GetComponent<MonsterMoveInGrid>().SetMonsterStatus(moveDirectionNum, monsterStartGridPosition[i, 1], monsterStartGridPosition[i, 0], 1);
                monster.transform.SetParent(this.transform, false);
                numberOfMonsters++;
            }
        }

        for (int i = 0; i < 28; ++i)
        {
            NextWaveObject.transform.GetChild(i).gameObject.SetActive(nextWave[i]);
        }
    }

    private void CreateNextWave()
    {
        gameLevel = GameSceneManager.GetComponent<GameSceneController>().GetLevel();
        int maxNumberOfNextWaveMonster = gameLevel;

        while(maxNumberOfNextWaveMonster > 0)
        {
            int random = Random.Range(0, 28);
            if (nextWave[random] == false)
            {
                nextWave[random] = true;
                maxNumberOfNextWaveMonster--;
            }
        }

        for(int i = 0; i < 28; ++i)
        {
            NextWaveObject.transform.GetChild(i).gameObject.SetActive(nextWave[i]);
        }
    }
    private void SetMonsterStartGridPosition()
    {
        monsterStartGridPosition[0, 0] = 1;
        monsterStartGridPosition[0, 1] = 7;
        monsterStartGridPosition[1, 0] = 2;
        monsterStartGridPosition[1, 1] = 7;
        monsterStartGridPosition[2, 0] = 3;
        monsterStartGridPosition[2, 1] = 7;
        monsterStartGridPosition[3, 0] = 4;
        monsterStartGridPosition[3, 1] = 7;
        monsterStartGridPosition[4, 0] = 5;
        monsterStartGridPosition[4, 1] = 7;
        monsterStartGridPosition[5, 0] = 6;
        monsterStartGridPosition[5, 1] = 7;
        monsterStartGridPosition[6, 0] = 7;
        monsterStartGridPosition[6, 1] = 7;

        monsterStartGridPosition[7, 0] = 7;
        monsterStartGridPosition[7, 1] = 7;
        monsterStartGridPosition[8, 0] = 7;
        monsterStartGridPosition[8, 1] = 6;
        monsterStartGridPosition[9, 0] = 7;
        monsterStartGridPosition[9, 1] = 5;
        monsterStartGridPosition[10, 0] = 7;
        monsterStartGridPosition[10, 1] = 4;
        monsterStartGridPosition[11, 0] = 7;
        monsterStartGridPosition[11, 1] = 3;
        monsterStartGridPosition[12, 0] = 7;
        monsterStartGridPosition[12, 1] = 2;
        monsterStartGridPosition[13, 0] = 7;
        monsterStartGridPosition[13, 1] = 1;

        monsterStartGridPosition[14, 0] = 7;
        monsterStartGridPosition[14, 1] = 1;
        monsterStartGridPosition[15, 0] = 6;
        monsterStartGridPosition[15, 1] = 1;
        monsterStartGridPosition[16, 0] = 5;
        monsterStartGridPosition[16, 1] = 1;
        monsterStartGridPosition[17, 0] = 4;
        monsterStartGridPosition[17, 1] = 1;
        monsterStartGridPosition[18, 0] = 3;
        monsterStartGridPosition[18, 1] = 1;
        monsterStartGridPosition[19, 0] = 2;
        monsterStartGridPosition[19, 1] = 1;
        monsterStartGridPosition[20, 0] = 1;
        monsterStartGridPosition[20, 1] = 1;

        monsterStartGridPosition[21, 0] = 1;
        monsterStartGridPosition[21, 1] = 1;
        monsterStartGridPosition[22, 0] = 1;
        monsterStartGridPosition[22, 1] = 2;
        monsterStartGridPosition[23, 0] = 1;
        monsterStartGridPosition[23, 1] = 3;
        monsterStartGridPosition[24, 0] = 1;
        monsterStartGridPosition[24, 1] = 4;
        monsterStartGridPosition[25, 0] = 1;
        monsterStartGridPosition[25, 1] = 5;
        monsterStartGridPosition[26, 0] = 1;
        monsterStartGridPosition[26, 1] = 6;
        monsterStartGridPosition[27, 0] = 1;
        monsterStartGridPosition[27, 1] = 7;
    }
}
