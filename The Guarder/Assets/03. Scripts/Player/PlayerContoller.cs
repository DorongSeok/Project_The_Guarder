using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContoller : MonoBehaviour
{
    private PlayerMoveInGrid playerMoveInGrid;

    public float minSwipeLength = 200f;
    private Vector2 firstPressPos;
    private Vector2 secondPressPos;
    private Vector2 currentSwipe;
    private float tweakFactor = 0.5f;
    
    private float swipeX;
    private float swipeY;

    private bool isGameOver = false;

    private void Awake()
    {
        playerMoveInGrid = GetComponent<PlayerMoveInGrid>();
    }

    private void Update()
    {
        if (isGameOver == false)
        {
            bool isSwipe;
            isSwipe = DetectSwipe();

            if (isSwipe == true)
            {
                playerMoveInGrid.MoveDirection = new Vector3(swipeX, swipeY, 0);
            }
            else
            {
                float x = Input.GetAxisRaw("Horizontal");
                float y = Input.GetAxisRaw("Vertical");

                playerMoveInGrid.MoveDirection = new Vector3(x, y, 0);
            }
        }
    }
    public bool DetectSwipe()
    {
        if (Input.touches.Length > 0)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
            {
                firstPressPos = new Vector2(t.position.x, t.position.y);
            }

            if (t.phase == TouchPhase.Ended)
            {
                secondPressPos = new Vector2(t.position.x, t.position.y);
                currentSwipe = new Vector3(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

                // 스와이프 입력이 아닌 탭 입력인지 확인
                if (currentSwipe.magnitude < minSwipeLength)
                {
                    return false;
                }

                currentSwipe.Normalize();

                // Swipe up
                if (currentSwipe.y > 0 && currentSwipe.x > 0 - tweakFactor && currentSwipe.x < tweakFactor)
                {
                    swipeX = 0;
                    swipeY = 1;

                    return true;
                }
                // Swipe down
                else if (currentSwipe.y < 0 && currentSwipe.x > 0 - tweakFactor && currentSwipe.x < tweakFactor)
                {
                    swipeX = 0;
                    swipeY = -1;

                    return true;
                }
                // Swipe left
                else if (currentSwipe.x < 0 && currentSwipe.y > 0 - tweakFactor && currentSwipe.y < tweakFactor)
                {
                    swipeX = -1;
                    swipeY = 0;

                    return true;
                }
                // Swipe right
                else if (currentSwipe.x > 0 && currentSwipe.y > 0 - tweakFactor && currentSwipe.y < tweakFactor)
                {
                    swipeX = 1;
                    swipeY = 0;

                    return true;
                }
                // Swipe up left
                else if (currentSwipe.y > 0 && currentSwipe.x < 0 )
                {
                    swipeX = -1;
                    swipeY = 1;

                    return true;
                }
                // Swipe up right
                else if (currentSwipe.y > 0 && currentSwipe.x > 0 )
                {
                    swipeX = 1;
                    swipeY = 1;

                    return true;
                }
                // Swipe down left
                else if (currentSwipe.y < 0 && currentSwipe.x < 0 )
                {
                    swipeX = -1;
                    swipeY = -1;

                    return true;
                }
                // Swipe down right
                else if (currentSwipe.y < 0 && currentSwipe.x > 0 )
                {
                    swipeX = 1;
                    swipeY = -1;

                    return true;
                }
            }
        }
        return false;
    }

    public void IsGameOver(bool _isGameOver)
    {
        isGameOver = _isGameOver;
    }
}