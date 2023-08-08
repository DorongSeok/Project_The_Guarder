using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    private int[,] MonsterMap = new int[8, 8];
    private bool[] nextWave = new bool[28];
    private int[,] monsterStartGridPosition = new int[28,2];
    private int numberOfMonsters = 0;

    private int gameLevel;
    [SerializeField]
    private int nextWaveCount;                  // 다음 몬스터가 생성될 때까지 필요한 턴
    [SerializeField]
    private int waveCount;
    [SerializeField]
    private int numberOfCreateNextWaveMonster;

    private int playerGridPositionX;
    private int playerGridPositionY;

    public GameObject GameSceneManager;
    public GameObject NextWaveObject;
    public GameObject MonsterPrefeb;

    void Start()
    {
        ResetMonsterManager();
        SetMonsterStartGridPosition();
    }
    public void MoveOnToNextStep()
    {
        MoveMonsters();
        CreateMonsters();
        CreateNextWave();
    }

    public void ResetMonsterManager()
    {
        DestroyMpnsters();
        ApplyGameLevel();
        ResetMonsterMap();
        ResetNextWave();
    }

    
    public void ResetMonsterMap()
    {
        numberOfMonsters = 0;
        for (int i = 0; i < 8; ++i)
        {
            for (int j = 0; j < 8; ++j)
            {
                MonsterMap[i, j] = 0;
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
        CreateFirstWave();
    }
    private void MoveMonsters()
    {
        if (numberOfMonsters > 0)
        {
            int playerGridPosition = GameSceneManager.GetComponent<GameSceneController>().GetPlayerGridPosition();
            for (int i = 1; i <= numberOfMonsters; ++i)
            {
                this.transform.GetChild(i).gameObject.GetComponent<MonsterController>().SetPlayerGridPosition(playerGridPosition);
                this.transform.GetChild(i).gameObject.GetComponent<MonsterController>().Move();
            }
        }
    }
    private void CreateMonsters()
    {
        SetPlayerGridPosition();
        for (int i = 0; i < 28; ++i)
        {
            if (nextWave[i] == true)
            {
                // 생성할 위치에 플레이어가 없으면 생성
                if ((playerGridPositionX == monsterStartGridPosition[i, 0] && playerGridPositionY == monsterStartGridPosition[i, 1]) == false)
                {
                    int moveDirectionNum = 0;
                    if (0 <= i && i <= 6)
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
                    monster.GetComponent<MonsterController>().SetMonsterStatus(moveDirectionNum, monsterStartGridPosition[i, 1], monsterStartGridPosition[i, 0], 1);
                    monster.transform.SetParent(this.transform, false);
                    numberOfMonsters++;
                    nextWave[i] = false;
                }
            }
        }

        for (int i = 0; i < 28; ++i)
        {
            NextWaveObject.transform.GetChild(i).gameObject.SetActive(nextWave[i]);
        }
    }
    private void CreateFirstWave()
    {
        int maxNumberOfNextWaveMonster = 1;

        while (maxNumberOfNextWaveMonster > 0)
        {
            int random = Random.Range(0, 28);
            if (nextWave[random] == false)
            {
                nextWave[random] = true;
                maxNumberOfNextWaveMonster--;
            }
        }

        for (int i = 0; i < 28; ++i)
        {
            NextWaveObject.transform.GetChild(i).gameObject.SetActive(nextWave[i]);
        }
    }
    private void CreateNextWave()
    {
        waveCount--;

        if (waveCount < 1)
        {
            waveCount = nextWaveCount;
            int maxNumberOfNextWaveMonster = numberOfCreateNextWaveMonster;

            while (maxNumberOfNextWaveMonster > 0)
            {
                int random = Random.Range(0, 28);
                if (nextWave[random] == false)
                {
                    nextWave[random] = true;
                    maxNumberOfNextWaveMonster--;
                }
            }

            for (int i = 0; i < 28; ++i)
            {
                NextWaveObject.transform.GetChild(i).gameObject.SetActive(nextWave[i]);
            }
        }
    }

    public void ApplyGameLevel()
    {
        // 레벨별 게임 난이도 설정
        gameLevel = GameSceneManager.GetComponent<GameSceneController>().GetLevel();
        switch (gameLevel)
        {
            case 1:
                numberOfCreateNextWaveMonster = 1;
                nextWaveCount = 3;
                break;
            case 2:
                numberOfCreateNextWaveMonster = 1;
                nextWaveCount = 2;
                break;
            case 3:
                numberOfCreateNextWaveMonster = 2;
                nextWaveCount = 3;
                break;
            case 4:
                numberOfCreateNextWaveMonster = 2;
                nextWaveCount = 2;
                break;
            case 5:
                numberOfCreateNextWaveMonster = 1;
                nextWaveCount = 1;
                break;
            case 6:
                numberOfCreateNextWaveMonster = 3;
                nextWaveCount = 2;
                break;
            default:
                numberOfCreateNextWaveMonster = 3;
                nextWaveCount = 1;
                break;
        }
        waveCount = nextWaveCount;
    }
    private void DestroyMpnsters()
    {
        for (int i = numberOfMonsters; i >= 1; --i)
        {
            this.transform.GetChild(i).gameObject.GetComponent<MonsterController>().Die();
        }
    }
    public void SetPlayerGridPosition()
    {
        int playerGridPosition = GameSceneManager.GetComponent<GameSceneController>().GetPlayerGridPosition();
        // 10의 자리는 X, 1의 자리는 Y
        this.playerGridPositionX = playerGridPosition / 10;
        this.playerGridPositionY = playerGridPosition % 10;
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
