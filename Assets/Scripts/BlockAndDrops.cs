using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BlockAndDrops : MonoBehaviour
{
    [Serializable] private enum BlockType { BLOCK, DROP }

    [SerializeField] private BlockType blockType;
    [SerializeField, Min(0)] private float amount = 0.1f;

    public float getAmont
    {
        get
        {
            return blockType == BlockType.BLOCK ? -amount : amount;
        }
    }
}
