using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TestChunk : MonoBehaviour
{
    void Start()
    {
        transform.DOLocalMoveZ(-50f, 2f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
    }
}
