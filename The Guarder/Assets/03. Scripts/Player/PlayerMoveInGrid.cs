
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
    private int gridMatrixNum = 7;                                // 그리드 x, y 칸 수;
    [SerializeField]   
    private int gridPositionY;                                      // 그리드의 몇 번째 행에 있는지
    [SerializeField]
    private int gridPositionX;                                      // 그리드의 몇 번째 열에 있는지

    public Vector3 MoveDirection { get; set; } = Vector3.zero;      // 이동 방향
    public bool IsMove { get; set; } = false;                       // 현재 이동 중인지

    private float[] positionInGrid = new float[10];             // 그리드 내의 position 값 저장

    public GameObject GameSceneManager;
        
    private void Awake()
    {
        ResetPlayerPosition();
        SetPositionInGrid();
    }

    private IEnumerator Start()
    {
        while(true)
        {
            if(MoveDirection != Vector3.zero && IsMove == false)
            {
                GameSceneManager.GetComponent<GameSceneController>().MoveOnToNextStep();

                if (1 <= gridPositionX + MoveDirection.x  && gridPositionX + MoveDirection.x <= gridMatrixNum
                    && 1 <= gridPositionY + MoveDirection.y && gridPositionY + MoveDirection.y <= gridMatrixNum)
                {
                    gridPositionX += (int)MoveDirection.x;
                    gridPositionY += (int)MoveDirection.y;
                }
                Vector3 moveDirection = Vector3.zero;
                moveDirection.x = positionInGrid[gridPositionX];
                moveDirection.y = positionInGrid[gridPositionY];

                Vector3 end = moveDirection;

                yield return StartCoroutine(GridSmoothMovement(end));
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
}
