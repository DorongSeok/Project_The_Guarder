
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveInGrid : MonoBehaviour
{
    [SerializeField]
    private float moveTime = 0.5f;                                  // 1ĭ �̵��� �ҿ�Ǵ� �ð�
    [SerializeField]
    private float gridSize = 0.7f;                                  // 1ĭ �̵��Ҷ� �Ÿ�
    [SerializeField]
    private int gridMatrixNum = 7;                                // �׸��� x, y ĭ ��;
    [SerializeField]   
    private int gridPositionY;                                      // �׸����� �� ��° �࿡ �ִ���
    [SerializeField]
    private int gridPositionX;                                      // �׸����� �� ��° ���� �ִ���

    public Vector3 MoveDirection { get; set; } = Vector3.zero;      // �̵� ����
    public bool IsMove { get; set; } = false;                       // ���� �̵� ������

    private float[] positionInGrid = new float[10];             // �׸��� ���� position �� ����

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

        // ���� ������ �ʱ�ȭ
        gridPositionX = gridMatrixNum / 2 + 1;
        gridPositionY = gridMatrixNum / 2 + 1;
    }

    private void SetPositionInGrid()
    {
        float startValue = -(gridMatrixNum / 2) * gridSize;
        for (int i = 1; i <= gridMatrixNum; ++i)
        {
            positionInGrid[i] = Mathf.Round(startValue * 10) * 0.1f;    // �Ҽ��� ��°�ڸ����� �ݿø�
            startValue += gridSize;
        }
    }
}
