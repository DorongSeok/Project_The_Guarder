using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonManager : MonoBehaviour
{
    public void ResetButtonClick()
    {
        this.GetComponent<GameSceneController>().GameReset();
    }
    public void LeveUpButtonClick()
    {
        this.GetComponent<GameSceneController>().LevelUp();
    }
    public void LevelDownButtonClick()
    {
        this.GetComponent<GameSceneController>().LevelDown();
    }
}
