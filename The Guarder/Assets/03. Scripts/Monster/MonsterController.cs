using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    [SerializeField]
    private float moveTime = 0.5f;                                  // 1칸 이동에 소요되는 시간
    [SerializeField]
    private float gridSize = 0.7f;                                  // 1칸 이동할때 거리
    [SerializeField]
    private int gridMatrixNum = 7;                                  // 그리드 x, y 칸 수 + 2;
    [SerializeField]
    private int gridPositionY;                                      // 그리드의 몇 번째 행에 있는지
    [SerializeField]
    private int gridPositionX;                                      // 그리드의 몇 번째 열에 있는지

    private int playerGridPositionX;                                // 플레이어가 그리드의 몇 번째 열에 있는지
    private int playerGridPositionY;                                // 플레이어가 그리드의 몇 번째 행에 있는지

    private int monsterMoveCount;                                   // 총 몇 번 움직였는지

    public Vector3 MoveDirection { get; set; } = Vector3.zero;      // 이동 방향
    public bool IsMove { get; set; } = false;                       // 현재 이동 중인지
    private float[] positionInGrid = new float[10];                 // 그리드 내의 position 값 저장
    [SerializeField]
    private int moveDirectionNumber;                                // 이동 방향 (1상, 2우, 3하, 4좌)
    [SerializeField]
    private int hp = 1;

    public GameObject SpriteHp;

    private void Awake()
    {
        monsterMoveCount = 0;
        ResetMosnterPosition();
        SetPositionInGrid();
    }
    public void SetMonsterStatus(int moveDirectionNumber, int gridPositionY, int gridPositionX, int hp = 1)
    {
        this.moveDirectionNumber = moveDirectionNumber;

        switch (moveDirectionNumber)
        {
            case (1):
                MoveDirection = new Vector3(0, 1, 0);
                break;
            case (2):
                MoveDirection = new Vector3(1, 0, 0);
                this.transform.GetChild(0).gameObject.transform.rotation = Quaternion.Euler(0, 0, -90);
                break;
            case (3):
                MoveDirection = new Vector3(0, -1, 0);
                this.transform.GetChild(0).gameObject.transform.rotation = Quaternion.Euler(0, 0, -180);
                break;
            case (4):
                MoveDirection = new Vector3(-1, 0, 0);
                this.transform.GetChild(0).gameObject.transform.rotation = Quaternion.Euler(0, 0, -270);
                break;
            default:
                Debug.Log("ERROR!!!: Monster MoveDirectionNumberSetting");
                break;
        }

        this.gridPositionY = gridPositionY;
        this.gridPositionX = gridPositionX;

        this.transform.position = new Vector3(positionInGrid[gridPositionX], positionInGrid[gridPositionY], 0);

        this.hp = hp;
        SpriteHp.transform.GetChild(hp).gameObject.SetActive(true);
    }

    public void Move()
    {
        if (hp == 0)
        {
            return;
        }

        // 그리드 범위를 넘어가지 않으면
        if (0 <= gridPositionX + MoveDirection.x && gridPositionX + MoveDirection.x <= (gridMatrixNum + 1)
            && 0 <= gridPositionY + MoveDirection.y && gridPositionY + MoveDirection.y <= (gridMatrixNum + 1))
        {
            // 이동할 곳에 플레이어가 없으면
            if (((gridPositionX + (int)MoveDirection.x) == playerGridPositionX && (gridPositionY + (int)MoveDirection.y) == playerGridPositionY) == false)
            {
                gridPositionX += (int)MoveDirection.x;
                gridPositionY += (int)MoveDirection.y;

                Vector3 moveDirection = Vector3.zero;
                moveDirection.x = positionInGrid[gridPositionX];
                moveDirection.y = positionInGrid[gridPositionY];
                Vector3 end = moveDirection;

                monsterMoveCount++;
                StartCoroutine(GridSmoothMovement(end));
            }
        }

        else
        {
            // Game Over
            Debug.Log("GAME OVER!!");
        }
    }
    private IEnumerator GridSmoothMovement(Vector3 end)
    {
        Vector3 start = transform.position;
        float current = 0;
        float percent = 0;

        IsMove = true;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / moveTime;

            transform.position = Vector3.Lerp(start, end, percent);

            yield return null;
        }

        IsMove = false;
    }

    public void ResetMosnterPosition()
    {
        transform.position = Vector3.zero;

        // 격자 포지션 초기화
        gridPositionX = gridMatrixNum / 2 + 1;
        gridPositionY = gridMatrixNum / 2 + 1;
    }

    private void SetPositionInGrid()
    {
        float startValue = -((gridMatrixNum + 2) / 2) * gridSize;
        for (int i = 0; i <= gridMatrixNum + 1; ++i)
        {
            positionInGrid[i] = Mathf.Round(startValue * 10) * 0.1f;    // 소수점 둘째자리에서 반올림
            startValue += gridSize;
        }
    }
    public int GetPositionXYInGrid()
    {
        return (gridPositionX * 10) + gridPositionY;
    }

    public void SetPlayerGridPosition(int playerGridPosition)
    {
        // 10의 자리는 X, 1의 자리는 Y
        this.playerGridPositionX = playerGridPosition / 10;
        this.playerGridPositionY = playerGridPosition % 10;
    }
    public int GetMonsterMapStatus()
    {
        // 100의 자리 = 방향, 10의 자리 = hp, 1의 자리 = 이동 횟수
        return (moveDirectionNumber * 100) + (hp * 10) + monsterMoveCount;
    }
    public int GetMoveDirectionNumber()
    {
        return moveDirectionNumber;
    }
    public int GetHp()
    {
        return hp;
    }
    public int GetMoveCount()
    {
        return monsterMoveCount;
    }

    public void Damaged()
    {
        SpriteHp.transform.GetChild(hp).gameObject.SetActive(false);
        hp--;
        SpriteHp.transform.GetChild(hp).gameObject.SetActive(true);
        if (hp == 0)
        {
            Die();
        }
    }
    public void Die()
    {
        // 죽는 애니메이션 넣고 애니메이션 마지막에 Destroy 함수 호출
        Destroy(this.gameObject);
    }
    public void MoveAndDie()
    {
        if (hp == 0)
        {
            return;
        }

        // 그리드 범위를 넘어가지 않으면
        if (0 <= gridPositionX + MoveDirection.x && gridPositionX + MoveDirection.x <= (gridMatrixNum + 1)
            && 0 <= gridPositionY + MoveDirection.y && gridPositionY + MoveDirection.y <= (gridMatrixNum + 1))
        {
            gridPositionX += (int)MoveDirection.x;
            gridPositionY += (int)MoveDirection.y;

            Vector3 moveDirection = Vector3.zero;
            moveDirection.x = positionInGrid[gridPositionX];
            moveDirection.y = positionInGrid[gridPositionY];
            Vector3 end = moveDirection;

            monsterMoveCount++;
            StartCoroutine(GridSmoothMovementAndDie(end));
        }

        else
        {
            // Game Over
            Debug.Log("GAME OVER!!");
        }
    }
    private IEnumerator GridSmoothMovementAndDie(Vector3 end)
    {
        Vector3 start = transform.position;
        float current = 0;
        float percent = 0;

        IsMove = true;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / moveTime;

            transform.position = Vector3.Lerp(start, end, percent);

            yield return null;
        }

        IsMove = false;
        Die();
    }
}
