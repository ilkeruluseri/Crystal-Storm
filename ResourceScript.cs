using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceScript : MonoBehaviour
{
    [SerializeField] private int increaseAmount = 1;

    public int GetIncrease()
    {
        return increaseAmount;
    }
}
