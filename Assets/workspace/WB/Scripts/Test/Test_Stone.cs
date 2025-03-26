using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT = Commons.Constants.PlayerType;

public class Test_Stone : MonoBehaviour
{
    Transform tr;
    public Transform Tr
    {
        get
        {
            if (tr == null)
                tr = transform;
            return tr;
        }
    }
    public TT type;
    Vector2Int xy;
    public Vector2Int XY
    {
        get
        {
            return xy;
        }
        set
        {
            xy = value;
            gameObject.name = $"{value.x},{value.y}";
            Tr.position = new Vector3(value.x, value.y, 0f);
        }
    }
}
