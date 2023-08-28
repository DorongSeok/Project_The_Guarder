
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

                        bool killAndAttackCheck = false;
                        killAndAttackCheck = KillAndAttackCheck();

                        // �̵� �ϴ� ĭ�� ���Ͱ� �ִ��� Ȯ��
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

                        // �÷��̾ ������ ���ϴ� ���ʹ� ���� �̵�
                        if (killAndAttackCheck == true)
                        {
                            GameSceneManager.GetComponent<GameSceneController>().DamagedMonsterMoveOnToNextStep();
                        }
                        // �÷��̾ ���͸� �� �������� ���� �̵�
                        else
                        {
                            GameSceneManager.GetComponent<GameSceneController>().MoveOnToNextStep();
                        }

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
        // ���� �ִϸ��̼����� ����
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

        // ���̵� �ִϸ��̼����� ����
        this.GetComponent<PlayerAnimationController>().ChangePlayerAnimation("Idle");
    }

    private bool KillAndAttackCheck()
    {
        // ���ϰ� 1�� ����, 2�� ų
        bool noStepCheck = false;           // �� ĭ �̹� �� Ȯ��
        bool oneStepCheck = false;           // �� ĭ �̹� �� Ȯ��
        bool oneStepNextTurnCheck = false;   // �� ĭ ���� �� Ȯ��
        bool twoStepNextTurnCheck = false;   // �� ĭ ���� �� Ȯ��

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

        // �� ĭ �������� ���� �Ͽ� ���� �ְ� �ִ���
        oneStepNextTurnCheck = MonsterManager.GetComponent<MonsterManager>().KillAndAttackCheckNextTurn(nextGridPositionX, nextGridPositionY);
        if (oneStepNextTurnCheck == true)
        { 
            return true;
        }

        // �� ĭ �������� ���� �Ͽ� ���� �ְ� �ִ���
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
