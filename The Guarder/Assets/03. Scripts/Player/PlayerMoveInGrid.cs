
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
    private int gridMatrixNum = 7;                                  // �׸��� x, y ĭ ��;
    [SerializeField]   
    private int gridPositionY;                                      // �׸����� �� ��° �࿡ �ִ���
    [SerializeField]
    private int gridPositionX;                                      // �׸����� �� ��° ���� �ִ���
    private int nextGridPositionY;
    private int nextGridPositionX;

    private bool oneStepKill = false;                               // �� ĭ �̵� ų����
    private bool twoStepKill = false;                               // �� ĭ �̵� ų����
    private bool oneStepNextTurnKill = false;                       // �� ĭ �̵� ������ ų����
    private bool twoStepNextTurnKill = false;                       // �� ĭ �̵� ������ ų����

    public Vector3 MoveDirection { get; set; } = Vector3.zero;      // �̵� ����
    public bool IsMove { get; set; } = false;                       // ���� �̵� ������

    private float[] positionInGrid = new float[10];                 // �׸��� ���� position �� ����

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
                    // �÷��̾ �̵� �������� üũ
                    if (1 <= gridPositionX + MoveDirection.x && gridPositionX + MoveDirection.x <= gridMatrixNum
                        && 1 <= gridPositionY + MoveDirection.y && gridPositionY + MoveDirection.y <= gridMatrixNum)
                    {
                        nextGridPositionX = gridPositionX + (int)MoveDirection.x;
                        nextGridPositionY = gridPositionY + (int)MoveDirection.y;

                        // �̵� �ϴ� ĭ�� ���Ͱ� �ִ��� Ȯ��
                        if(KillCheck() == true)
                        {
                            GameSceneManager.GetComponent<GameSceneController>().AddScore(100);
                        }

                        // �÷��̾ ���͸� �� ������� ���� �̵�
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
                        // �÷��̾� �̵� �ð���ŭ �����̸� �ֱ� ����
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

        // �� ĭ �������� ���� �ְ� �ִ���
        oneStepKill = MonsterManager.GetComponent<MonsterManager>().KillCheckThisTurn(nextGridPositionX, nextGridPositionY);
        if (oneStepKill == true)
        {
            return true;
        }

        // ��ĭ �������� ���� �ְ� �ִ���
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

        // �� ĭ �������� ���� �Ͽ� ���� �ְ� �ִ���
        oneStepNextTurnKill = MonsterManager.GetComponent<MonsterManager>().KillCheckNextTurn(nextGridPositionX, nextGridPositionY);
        if (oneStepNextTurnKill == true)
        {
            return true;
        }
        
        // �� ĭ �������� ���� �Ͽ� ���� �ְ� �ִ���
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

    public int GetPlayerGridPosition()
    {
        // X �� 10�� �ڸ��� Y �� 1�� �ڸ���
        return (gridPositionX * 10) + gridPositionY;
    }
    public void IsGameOver(bool _isGameOver)
    {
        isGameOver = _isGameOver;
    }
}
