using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTime : MonoBehaviour
{
    public float time;
    public float turnTimeLimit = 15;
    private void Awake()
    {
        time = Time.time;

        if (time >= turnTimeLimit)
        {
        GameEvents.TurnChange.Invoke();
        }
    }


    public void ResetTime()
    {
        time = 0;
    }
}
