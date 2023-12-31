using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    private int[,] monsterMap = new int[9, 9];
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
    [SerializeField]
    private int monsterMaxHp;

    private int playerGridPositionX;
    private int playerGridPositionY;

    private bool isCreateMonster = false;

    public GameObject GameSceneManager;
    public GameObject NextWaveObject;
    public GameObject MonsterPrefeb;
    public GameObject DyingMonsterAnimation;
    public GameObject Player;

    void Start()
    {
        SetMonsterStartGridPosition();
        ResetMonsterManager();
    }
    public void MoveOnToNextStep()
    {
        MoveMonsters();
        CreateNextWave();
        GenerateMonsters();
        DrawMonsterMap();
        Invoke("RegenerateMonsters", 0.2f);
    }

    public void DamagedMonsterMoveOnToNextStep()
    {
        MoveDamgedMonster();
        DrawMonsterMap();
        DamagedMonsterCheck();
        Invoke("RegenerateMonsters", 0.2f);
    }

    public void ResetMonsterManager()
    {
        isCreateMonster = false;
        DestroyMonsters();
        ApplyGameLevel();
        ResetMonsterMap();
        numberOfMonsters = 0;
        ResetNextWave();
    }

    
    public void ResetMonsterMap()
    {
        for (int i = 0; i < 9; ++i)
        {
            for (int j = 0; j < 9; ++j)
            {
                monsterMap[i, j] = 0;
            }
        }
    }
    public void DrawMonsterMap()
    {
        ResetMonsterMap();

        for (int i = numberOfMonsters; i >= 1; --i)
        {
            int monsterGridPositionXY = this.transform.GetChild(i).gameObject.GetComponent<MonsterController>().GetPositionXYInGrid();

            // 100의 자리 = 방향, 10의 자리 = hp, 1의 자리 = 이동 횟수
            int monsterMapStatus = this.transform.GetChild(i).gameObject.GetComponent<MonsterController>().GetMonsterMapStatus();

            if (monsterMap[monsterGridPositionXY % 10, monsterGridPositionXY / 10] == 0)
            {
                monsterMap[monsterGridPositionXY % 10, monsterGridPositionXY / 10] = monsterMapStatus;
            }
            else
            {
                int moveDirectionNumber;
                int hp;
                int moveCount;
                int combinedMonsterMapStatus = monsterMap[monsterGridPositionXY % 10, monsterGridPositionXY / 10];

                if(combinedMonsterMapStatus % 10 > monsterMapStatus % 10)
                {
                    moveDirectionNumber = monsterMapStatus / 100;
                    hp = ((combinedMonsterMapStatus % 100) / 10) + ((monsterMapStatus % 100) / 10);
                    if(hp>6)
                    {
                        hp = 6;
                    }
                    moveCount = monsterMapStatus % 10;
                }
                else
                {
                    moveDirectionNumber = combinedMonsterMapStatus / 100;
                    hp = ((combinedMonsterMapStatus % 100) / 10) + ((monsterMapStatus % 100) / 10);
                    if (hp > 6)
                    {
                        hp = 6;
                    }
                    moveCount = combinedMonsterMapStatus % 10;
                }
                monsterMap[monsterGridPositionXY % 10, monsterGridPositionXY / 10] = ((moveDirectionNumber * 100) + (hp * 10) + moveCount);
            }
        }
    }
    private void RegenerateMonsters()
    {
        DestroyMonsters();
        numberOfMonsters = 0;

        for (int i = 0; i < 9; ++i)
        {
            for (int j = 0; j < 9; ++j)
            {
                if (monsterMap[j, i] != 0)
                {
                    if ((monsterMap[j, i] % 100) / 10 > 0)
                    {
                        GameObject monster = Instantiate(MonsterPrefeb) as GameObject;
                        monster.GetComponent<MonsterController>().SetMonsterStatus(monsterMap[j, i] / 100, j, i, (monsterMap[j, i] % 100) / 10, monsterMap[j, i] % 10);
                        monster.transform.SetParent(this.transform, false);
                        numberOfMonsters++;
                    }
                    else
                    {
                        GameObject dyingMonsterAnimation = Instantiate(DyingMonsterAnimation) as GameObject;
                        dyingMonsterAnimation.transform.SetParent(Player.transform, false);
                        monsterMap[j, i] = 0;
                    }
                }
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
            for (int i = 1; i <= numberOfMonsters; ++i)
            {
                this.transform.GetChild(i).gameObject.GetComponent<MonsterController>().Move();
            }
        }
    }
    private void MoveDamgedMonster()
    {
        if (numberOfMonsters > 0)
        {
            for (int i = 1; i <= numberOfMonsters; ++i)
            {
                if (this.transform.GetChild(i).gameObject.GetComponent<MonsterController>().GetIsDamagedNextTurn() == true)
                {
                    int positionXY = this.transform.GetChild(i).gameObject.GetComponent<MonsterController>().GetPositionXYInGrid();

                    if (positionXY / 10 == 0 || positionXY / 10 == 8 ||
                        positionXY % 10 == 0 || positionXY % 10 == 8)
                    {
                        for (int j = 0; j < 28; ++j)
                        {
                            if (positionXY / 10 == monsterStartGridPosition[j, 0] && positionXY % 10 == monsterStartGridPosition[j, 1])
                            {
                                NextWaveObject.transform.GetChild(j).gameObject.SetActive(false);
                                nextWave[j] = false;
                                break;
                            }
                        }
                    }

                    this.transform.GetChild(i).gameObject.GetComponent<MonsterController>().Move();
                }
            }
        }
    }

    private void GenerateMonsters()
    {
        isCreateMonster = true;
        SetPlayerGridPosition();
        for (int i = 0; i < 28; ++i)
        {
            if (nextWave[i] == true)
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
                monster.GetComponent<MonsterController>().SetMonsterStatus(moveDirectionNum, monsterStartGridPosition[i, 1], monsterStartGridPosition[i, 0], Random.Range(1, monsterMaxHp + 1));
                monster.transform.SetParent(this.transform, false);
                numberOfMonsters++;
            }
        }
    }
    private void CreateFirstWave()
    {
        int maxNumberOfNextWaveMonster = 1;

        if (isCreateMonster == true)
        {
            for (int i = 0; i < 28; ++i)
            {
                NextWaveObject.transform.GetChild(i).gameObject.SetActive(false);
                nextWave[i] = false;
            }
        }

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
        isCreateMonster = false;

        GenerateMonsters();
    }
    private void CreateNextWave()
    {
        waveCount--;

        if (isCreateMonster == true)
        {
            for (int i = 0; i < 28; ++i)
            {
                NextWaveObject.transform.GetChild(i).gameObject.SetActive(false);
                nextWave[i] = false;
            }
        }

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
            isCreateMonster = false;
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
                monsterMaxHp = 1;
                break;
            case 2:
                numberOfCreateNextWaveMonster = 1;
                nextWaveCount = 2;
                monsterMaxHp = 1;
                break;
            case 3:
                numberOfCreateNextWaveMonster = 2;
                nextWaveCount = 3;
                monsterMaxHp = 1;
                break;
            case 4:
                numberOfCreateNextWaveMonster = 2;
                nextWaveCount = 2;
                monsterMaxHp = 2;
                break;
            case 5:
                numberOfCreateNextWaveMonster = 1;
                nextWaveCount = 1;
                monsterMaxHp = 2;
                break;
            case 6:
                numberOfCreateNextWaveMonster = 3;
                nextWaveCount = 2;
                monsterMaxHp = 2;
                break;
            case 7:
                numberOfCreateNextWaveMonster = 3;
                nextWaveCount = 2;
                monsterMaxHp = 3;
                break;
            default:
                numberOfCreateNextWaveMonster = 4;
                nextWaveCount = 3;
                monsterMaxHp = 3;
                break;
        }
        waveCount = nextWaveCount;
    }

    public bool KillAndAttackCheckThisTurn(int x, int y)
    {
        for (int i = numberOfMonsters; i >= 1; --i)
        {
            int monsterGridPositionXY = this.transform.GetChild(i).gameObject.GetComponent<MonsterController>().GetPositionXYInGrid();

            if ((monsterGridPositionXY / 10) == x && (monsterGridPositionXY % 10) == y)
            {
                return true;
            }
        }
        return false;
    }
    public bool KillAndAttackCheckNextTurn(int x, int y)
    {
        bool damagedMonsterCheck = false;
        // 위에서 내려오는 몬스터 체크
        if ((y + 1) <= 8 && monsterMap[y+1,x] /100 == 3)
        {
            for (int i = numberOfMonsters; i >= 1; --i)
            {
                int monsterGridPositionXY = this.transform.GetChild(i).gameObject.GetComponent<MonsterController>().GetPositionXYInGrid();
                if ((monsterGridPositionXY / 10) == x && (monsterGridPositionXY % 10) == (y + 1))
                {
                    damagedMonsterCheck = true;
                    this.transform.GetChild(i).gameObject.GetComponent<MonsterController>().SetIsDamagedNextTurn(true);
                    break;
                }
            }
        }
        // 오른쪽에서 왼쪽으로 오는 몬스터 체크
        else if ((x + 1) <= 8 && monsterMap[y, x+1] / 100 == 4)
        {
            for (int i = numberOfMonsters; i >= 1; --i)
            {
                int monsterGridPositionXY = this.transform.GetChild(i).gameObject.GetComponent<MonsterController>().GetPositionXYInGrid();
                if ((monsterGridPositionXY / 10) == (x + 1) && (monsterGridPositionXY % 10) == y)
                {
                    damagedMonsterCheck = true;
                    this.transform.GetChild(i).gameObject.GetComponent<MonsterController>().SetIsDamagedNextTurn(true);
                    break;
                }
            }
        }
        // 아래에서 올라 오는 몬스터 체크
        else if ((y - 1) >= 0&& monsterMap[y - 1, x] / 100 == 1)
        {
            for (int i = numberOfMonsters; i >= 1; --i)
            {
                int monsterGridPositionXY = this.transform.GetChild(i).gameObject.GetComponent<MonsterController>().GetPositionXYInGrid();
                if ((monsterGridPositionXY / 10) == x && (monsterGridPositionXY % 10) == (y - 1))
                {
                    damagedMonsterCheck = true;
                    this.transform.GetChild(i).gameObject.GetComponent<MonsterController>().SetIsDamagedNextTurn(true);
                    break;
                }
            }
        }
        // 왼쪽에서 오른쪽으로 오는 몬스터 체크
        else if ((x - 1) >= 0&& monsterMap[y, x-1] / 100 == 2)
        {
            for (int i = numberOfMonsters; i >= 1; --i)
            {
                int monsterGridPositionXY = this.transform.GetChild(i).gameObject.GetComponent<MonsterController>().GetPositionXYInGrid();
                if ((monsterGridPositionXY / 10) == (x - 1) && (monsterGridPositionXY % 10) == y)
                {
                    damagedMonsterCheck = true;
                    this.transform.GetChild(i).gameObject.GetComponent<MonsterController>().SetIsDamagedNextTurn(true);
                    break;
                }
            }
        }
        // 데미지를 입을 몬스터가 있는지 없는지 반환
        return damagedMonsterCheck;
    }
    private void DamagedMonsterCheck()
    {
        SetPlayerGridPosition();
        monsterMap[playerGridPositionY, playerGridPositionX] -= 10;
    }

    private void DestroyMonsters()
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
        monsterStartGridPosition[0, 1] = 8;
        monsterStartGridPosition[1, 0] = 2;
        monsterStartGridPosition[1, 1] = 8;
        monsterStartGridPosition[2, 0] = 3;
        monsterStartGridPosition[2, 1] = 8;
        monsterStartGridPosition[3, 0] = 4;
        monsterStartGridPosition[3, 1] = 8;
        monsterStartGridPosition[4, 0] = 5;
        monsterStartGridPosition[4, 1] = 8;
        monsterStartGridPosition[5, 0] = 6;
        monsterStartGridPosition[5, 1] = 8;
        monsterStartGridPosition[6, 0] = 7;
        monsterStartGridPosition[6, 1] = 8;

        monsterStartGridPosition[7, 0] = 8;
        monsterStartGridPosition[7, 1] = 7;
        monsterStartGridPosition[8, 0] = 8;
        monsterStartGridPosition[8, 1] = 6;
        monsterStartGridPosition[9, 0] = 8;
        monsterStartGridPosition[9, 1] = 5;
        monsterStartGridPosition[10, 0] = 8;
        monsterStartGridPosition[10, 1] = 4;
        monsterStartGridPosition[11, 0] = 8;
        monsterStartGridPosition[11, 1] = 3;
        monsterStartGridPosition[12, 0] = 8;
        monsterStartGridPosition[12, 1] = 2;
        monsterStartGridPosition[13, 0] = 8;
        monsterStartGridPosition[13, 1] = 1;

        monsterStartGridPosition[14, 0] = 7;
        monsterStartGridPosition[14, 1] = 0;
        monsterStartGridPosition[15, 0] = 6;
        monsterStartGridPosition[15, 1] = 0;
        monsterStartGridPosition[16, 0] = 5;
        monsterStartGridPosition[16, 1] = 0;
        monsterStartGridPosition[17, 0] = 4;
        monsterStartGridPosition[17, 1] = 0;
        monsterStartGridPosition[18, 0] = 3;
        monsterStartGridPosition[18, 1] = 0;
        monsterStartGridPosition[19, 0] = 2;
        monsterStartGridPosition[19, 1] = 0;
        monsterStartGridPosition[20, 0] = 1;
        monsterStartGridPosition[20, 1] = 0;

        monsterStartGridPosition[21, 0] = 0;
        monsterStartGridPosition[21, 1] = 1;
        monsterStartGridPosition[22, 0] = 0;
        monsterStartGridPosition[22, 1] = 2;
        monsterStartGridPosition[23, 0] = 0;
        monsterStartGridPosition[23, 1] = 3;
        monsterStartGridPosition[24, 0] = 0;
        monsterStartGridPosition[24, 1] = 4;
        monsterStartGridPosition[25, 0] = 0;
        monsterStartGridPosition[25, 1] = 5;
        monsterStartGridPosition[26, 0] = 0;
        monsterStartGridPosition[26, 1] = 6;
        monsterStartGridPosition[27, 0] = 0;
        monsterStartGridPosition[27, 1] = 7;
    }
}
