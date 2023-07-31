using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContoller : MonoBehaviour
{
    private PlayerMoveInGrid playerMoveInGrid;

    private void Awake()
    {
        playerMoveInGrid = GetComponent<PlayerMoveInGrid>();
    }

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        playerMoveInGrid.MoveDirection = new Vector3(x, y, 0);
    }
}