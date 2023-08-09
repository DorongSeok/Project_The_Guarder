using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    [SerializeField]
    private float moveTime = 0.5f;                                  // 1ĭ �̵��� �ҿ�Ǵ� �ð�
    [SerializeField]
    private float gridSize = 0.7f;                                  // 1ĭ �̵��Ҷ� �Ÿ�
    [SerializeField]
    private int gridMatrixNum = 7;                                  // �׸��� x, y ĭ �� + 2;
    [SerializeField]
    private int gridPositionY;                                      // �׸����� �� ��° �࿡ �ִ���
    [SerializeField]
    private int gridPositionX;                                      // �׸����� �� ��° ���� �ִ���

    private int playerGridPositionX;                                // �÷��̾ �׸����� �� ��° ���� �ִ���
    private int playerGridPositionY;                                // �÷��̾ �׸����� �� ��° �࿡ �ִ���

    private int monsterMoveCount;                                   // �� �� �� ����������

    public Vector3 MoveDirection { get; set; } = Vector3.zero;      // �̵� ����
    public bool IsMove { get; set; } = false;                       // ���� �̵� ������
    private float[] positionInGrid = new float[10];                 // �׸��� ���� position �� ����
    [SerializeField]
    private int moveDirectionNumber;                                // �̵� ���� (1��, 2��, 3��, 4��)
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

        // �׸��� ������ �Ѿ�� ������
        if (0 <= gridPositionX + MoveDirection.x && gridPositionX + MoveDirection.x <= (gridMatrixNum + 1)
            && 0 <= gridPositionY + MoveDirection.y && gridPositionY + MoveDirection.y <= (gridMatrixNum + 1))
        {
            // �̵��� ���� �÷��̾ ������
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

        // ���� ������ �ʱ�ȭ
        gridPositionX = gridMatrixNum / 2 + 1;
        gridPositionY = gridMatrixNum / 2 + 1;
    }

    private void SetPositionInGrid()
    {
        float startValue = -((gridMatrixNum + 2) / 2) * gridSize;
        for (int i = 0; i <= gridMatrixNum + 1; ++i)
        {
            positionInGrid[i] = Mathf.Round(startValue * 10) * 0.1f;    // �Ҽ��� ��°�ڸ����� �ݿø�
            startValue += gridSize;
        }
    }
    public int GetPositionXYInGrid()
    {
        return (gridPositionX * 10) + gridPositionY;
    }

    public void SetPlayerGridPosition(int playerGridPosition)
    {
        // 10�� �ڸ��� X, 1�� �ڸ��� Y
        this.playerGridPositionX = playerGridPosition / 10;
        this.playerGridPositionY = playerGridPosition % 10;
    }
    public int GetMonsterMapStatus()
    {
        // 100�� �ڸ� = ����, 10�� �ڸ� = hp, 1�� �ڸ� = �̵� Ƚ��
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
        // �״� �ִϸ��̼� �ְ� �ִϸ��̼� �������� Destroy �Լ� ȣ��
        Destroy(this.gameObject);
    }
    public void MoveAndDie()
    {
        if (hp == 0)
        {
            return;
        }

        // �׸��� ������ �Ѿ�� ������
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
