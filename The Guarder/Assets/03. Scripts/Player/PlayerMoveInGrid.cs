
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

                        bool killAndAttackCheck = false;
                        killAndAttackCheck = KillAndAttackCheck();

                        // 이동 하는 칸에 몬스터가 있는지 확인
                        if (killAndAttackCheck == true)
                        {
                            GameSceneManager.GetComponent<GameSceneController>().AddScore(100);
                        }

                        Vector3 moveDirection = Vector3.zero;
                        moveDirection.x = positionInGrid[nextGridPositionX];
                        moveDirection.y = positionInGrid[nextGridPositionY];

                        Vector3 end = moveDirection;

                        gridPositionX = nextGridPositionX;
                        gridPositionY = nextGridPositionY;

                        // 플레이어가 죽이지 못하는 몬스터는 따로 이동
                        if (killAndAttackCheck == true)
                        {
                            GameSceneManager.GetComponent<GameSceneController>().DamagedMonsterMoveOnToNextStep();
                        }
                        // 플레이어가 몬스터를 못 때렸으면 몬스터 이동
                        else
                        {
                            GameSceneManager.GetComponent<GameSceneController>().MoveOnToNextStep();
                        }

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
        // 무브 애니메이션으로 변경
        this.GetComponent<PlayerAnimationController>().ChangePlayerAnimation("Move");

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

        // 아이들 애니메이션으로 변경
        this.GetComponent<PlayerAnimationController>().ChangePlayerAnimation("Idle");
    }

    private bool KillAndAttackCheck()
    {
        // 리턴값 1은 어택, 2는 킬
        bool noStepCheck = false;           // 한 칸 이번 턴 확인
        bool oneStepCheck = false;           // 두 칸 이번 턴 확인
        bool oneStepNextTurnCheck = false;   // 한 칸 다음 턴 확인
        bool twoStepNextTurnCheck = false;   // 두 칸 다음 턴 확인

        noStepCheck = MonsterManager.GetComponent<MonsterManager>().KillAndAttackCheckThisTurn(nextGridPositionX, nextGridPositionY);
        if (noStepCheck == true) 
        {
            return true;
        }

        if (1 <= nextGridPositionX + MoveDirection.x && nextGridPositionX + MoveDirection.x <= gridMatrixNum
            && 1 <= nextGridPositionY + MoveDirection.y && nextGridPositionY + MoveDirection.y <= gridMatrixNum)
        {
            int twoStepGridPositionX = nextGridPositionX + (int)MoveDirection.x;
            int twoStepGridPositionY = nextGridPositionY + (int)MoveDirection.y;
            oneStepCheck = MonsterManager.GetComponent<MonsterManager>().KillAndAttackCheckThisTurn(twoStepGridPositionX, twoStepGridPositionY);

            if (oneStepCheck == true)
            {
                nextGridPositionX = twoStepGridPositionX;
                nextGridPositionY = twoStepGridPositionY;
                return true;
            }
        }

        // 한 칸 움직여서 다음 턴에 잡을 애가 있는지
        oneStepNextTurnCheck = MonsterManager.GetComponent<MonsterManager>().KillAndAttackCheckNextTurn(nextGridPositionX, nextGridPositionY);
        if (oneStepNextTurnCheck == true)
        { 
            return true;
        }

        // 두 칸 움직여서 다음 턴에 잡을 애가 있는지
        if (1 <= nextGridPositionX + MoveDirection.x && nextGridPositionX + MoveDirection.x <= gridMatrixNum
            && 1 <= nextGridPositionY + MoveDirection.y && nextGridPositionY + MoveDirection.y <= gridMatrixNum)
        {
            int twoStepGridPositionX = nextGridPositionX + (int)MoveDirection.x;
            int twoStepGridPositionY = nextGridPositionY + (int)MoveDirection.y;
            twoStepNextTurnCheck = MonsterManager.GetComponent<MonsterManager>().KillAndAttackCheckNextTurn(twoStepGridPositionX, twoStepGridPositionY);

            if (twoStepNextTurnCheck == true)
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
