
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveInGrid : MonoBehaviour
{
    [SerializeField]
    private float moveTime = 0.5f;                                  // 1칸 이동에 소요되는 시간
    [SerializeField]
    private float gridSize = 0.7f;                                  // 1칸 이동할때 거리
    [SerializeField]
    private int gridMatrixNum = 7;                                  // 그리드 x, y 칸 수;
    [SerializeField]   
    private int gridPositionY;                                      // 그리드의 몇 번째 행에 있는지
    [SerializeField]
    private int gridPositionX;                                      // 그리드의 몇 번째 열에 있는지
    private int nextGridPositionY;
    private int nextGridPositionX;

    private bool oneStepKill = false;                               // 한 칸 이동 킬인지
    private bool twoStepKill = false;                               // 두 칸 이동 킬인지
    private bool oneStepNextTurnKill = false;                       // 한 칸 이동 다음턴 킬인지
    private bool twoStepNextTurnKill = false;                       // 두 칸 이동 다음턴 킬인지

    public Vector3 MoveDirection { get; set; } = Vector3.zero;      // 이동 방향
    public bool IsMove { get; set; } = false;                       // 현재 이동 중인지

    private float[] positionInGrid = new float[10];                 // 그리드 내의 position 값 저장

    public GameObject GameSceneManager;
    public GameObject MonsterManager;

    private bool isGameOver = false;
        
    private void Awake()
    {
        ResetPlayerPosition();
        SetPositionInGrid();
    }

    private IEnumerator Start()
    {
        while(true)
        {
            if (isGameOver == false)
            {
                if (MoveDirection != Vector3.zero && IsMove == false)
                {
                    // 플레이어가 이동 가능한지 체크
                    if (1 <= gridPositionX + MoveDirection.x && gridPositionX + MoveDirection.x <= gridMatrixNum
                        && 1 <= gridPositionY + MoveDirection.y && gridPositionY + MoveDirection.y <= gridMatrixNum)
                    {
                        nextGridPositionX = gridPositionX + (int)MoveDirection.x;
                        nextGridPositionY = gridPositionY + (int)MoveDirection.y;

                        // 이동 하는 칸에 몬스터가 있는지 확인
                        if(KillCheck() == true)
                        {
                            GameSceneManager.GetComponent<GameSceneController>().AddScore(100);
                        }

                        // 플레이어가 몬스터를 못 잡았으면 몬스터 이동
                        if (oneStepKill == false && twoStepKill == false &&
                            oneStepNextTurnKill == false && twoStepNextTurnKill == false)
                        {
                            GameSceneManager.GetComponent<GameSceneController>().MoveOnToNextStep();
                        }

                        Vector3 moveDirection = Vector3.zero;
                        moveDirection.x = positionInGrid[nextGridPositionX];
                        moveDirection.y = positionInGrid[nextGridPositionY];

                        Vector3 end = moveDirection;

                        gridPositionX = nextGridPositionX;
                        gridPositionY = nextGridPositionY;

                        yield return StartCoroutine(GridSmoothMovement(end));
                    }
                    else
                    {
                        // 플레이어 이동 시간만큼 딜레이를 주기 위함
                        yield return new WaitForSeconds(moveTime);
                        GameSceneManager.GetComponent<GameSceneController>().MoveOnToNextStep();
                    }
                }
            }
            yield return null;
        }
    }

    private IEnumerator GridSmoothMovement(Vector3 end)
    {
        Vector3 start = transform.position;
        float current = 0;
        float percent = 0;

        IsMove = true;

        while(percent < 1)
        {
            current += Time.deltaTime;
            percent = current / moveTime;

            transform.position = Vector3.Lerp(start, end, percent);

            yield return null;
        }

        IsMove = false;
    }

    private bool KillCheck()
    {
        oneStepKill = false;
        twoStepKill = false;
        oneStepNextTurnKill = false;
        twoStepNextTurnKill = false;
        MonsterManager.GetComponent<MonsterManager>().DrawMonsterMap();

        // 한 칸 움직여서 잡을 애가 있는지
        oneStepKill = MonsterManager.GetComponent<MonsterManager>().KillCheckThisTurn(nextGridPositionX, nextGridPositionY);
        if (oneStepKill == true)
        {
            return true;
        }

        // 두칸 움직여서 잡을 애가 있는지
        if (1 <= nextGridPositionX + MoveDirection.x && nextGridPositionX + MoveDirection.x <= gridMatrixNum
            && 1 <= nextGridPositionY + MoveDirection.y && nextGridPositionY + MoveDirection.y <= gridMatrixNum)
        {
            int twoStepGridPositionX = nextGridPositionX + (int)MoveDirection.x;
            int twoStepGridPositionY = nextGridPositionY + (int)MoveDirection.y;
            twoStepKill = MonsterManager.GetComponent<MonsterManager>().KillCheckThisTurn(twoStepGridPositionX, twoStepGridPositionY);

            if (twoStepKill == true)
            {
                nextGridPositionX = twoStepGridPositionX;
                nextGridPositionY = twoStepGridPositionY;
                return true;
            }
        }

        // 한 칸 움직여서 다음 턴에 잡을 애가 있는지
        oneStepNextTurnKill = MonsterManager.GetComponent<MonsterManager>().KillCheckNextTurn(nextGridPositionX, nextGridPositionY);
        if (oneStepNextTurnKill == true)
        {
            return true;
        }
        
        // 두 칸 움직여서 다음 턴에 잡을 애가 있는지
        if (1 <= nextGridPositionX + MoveDirection.x && nextGridPositionX + MoveDirection.x <= gridMatrixNum
            && 1 <= nextGridPositionY + MoveDirection.y && nextGridPositionY + MoveDirection.y <= gridMatrixNum)
        {
            int twoStepGridPositionX = nextGridPositionX + (int)MoveDirection.x;
            int twoStepGridPositionY = nextGridPositionY + (int)MoveDirection.y;
            twoStepNextTurnKill = MonsterManager.GetComponent<MonsterManager>().KillCheckNextTurn(twoStepGridPositionX, twoStepGridPositionY);
        
            if (twoStepNextTurnKill == true)
            {
                nextGridPositionX = twoStepGridPositionX;
                nextGridPositionY = twoStepGridPositionY;
                return true;
            }
        }
        return false;
    }

    public void ResetPlayerPosition()
    {
        transform.position = Vector3.zero;

        // 격자 포지션 초기화
        gridPositionX = gridMatrixNum / 2 + 1;
        gridPositionY = gridMatrixNum / 2 + 1;
    }

    private void SetPositionInGrid()
    {
        float startValue = -(gridMatrixNum / 2) * gridSize;
        for (int i = 1; i <= gridMatrixNum; ++i)
        {
            positionInGrid[i] = Mathf.Round(startValue * 10) * 0.1f;    // 소수점 둘째자리에서 반올림
            startValue += gridSize;
        }
    }

    public int GetPlayerGridPosition()
    {
        // X 는 10의 자리로 Y 는 1의 자리로
        return (gridPositionX * 10) + gridPositionY;
    }
    public void IsGameOver(bool _isGameOver)
    {
        isGameOver = _isGameOver;
    }
}
