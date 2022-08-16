using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishGroundItem : MonoBehaviour
{
    [SerializeField] private int finishAmount = 1;

    public int getFinishAmount
    {
        get
        {
            return finishAmount;
        }
    }
}
