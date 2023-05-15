using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ChunksGeneratorController : MonoBehaviour
{
    [SerializeField] private List<GameObject> _chunksBoofer;
    [SerializeField] private Transform _rotateObject;

    private void Start()
    {
        _rotateObject.DORotate(new Vector3(-360, 0, 0), 250, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear);
    }

    private void AddChunk()
    {
        
    }
}
