using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Test_Placer : MonoBehaviour, IPointerClickHandler
{
    Test_RuleCheckTest rule;
    public Vector2Int XY;
    public int x, y;

    public void Initialize(int x, int y, Test_RuleCheckTest rule)
    {
        this.x = x;
        this.y = y;
        XY = new Vector2Int(x, y);
        this.rule = rule;
        transform.position = new Vector3(x, y, 0f);
        gameObject.name = $"{x},{y}";
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"OnPointerClick : {XY}");
        rule.PlaceStone(XY, rule.GetNextType(x, y));
    }
}
