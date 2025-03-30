using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class StoneAnimation : MonoBehaviour
{
    bool isInitialized;
    Sequence pop;


    void OnEnable()
    {
        if (pop == null)
            CreatePopTween().Forget();
        else
            pop.Restart();
    }


    async UniTask CreatePopTween()
    {
        await UniTask.Yield();
        await UniTask.Yield();

        var rect = GetComponent<RectTransform>();
        var originSize = rect.sizeDelta;

        pop = DOTween.Sequence();
        pop.Append(rect.DOSizeDelta(originSize * 1.2f, 0.2f))
           .Append(rect.DOSizeDelta(originSize, 0.2f))
           .SetEase(Ease.Linear)
           .SetAutoKill(false).Restart();
    }

}
