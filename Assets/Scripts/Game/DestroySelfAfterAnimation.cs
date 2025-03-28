using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelfAfterAnimation : MonoBehaviour
{
    public float delay = 1f;

    void OnEnable()
    {
        Destroy(gameObject, delay);
    }
}
