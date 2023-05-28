using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class ChunksGeneratorController : MonoBehaviour
{
    [SerializeField] private List<GameObject> _chunksBoofer;
    [SerializeField] private Transform _rotateObject;
    [SerializeField] private Transform _chunksParent;

    private void Start()
    {
        StartRotate();
    }

    private void StartRotate()
    {
        _rotateObject.DORotate(new Vector3(-360f, 0f, 0f), 100f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                _rotateObject.eulerAngles = new(0f,0f,0f);
                StartRotate();
            });
    }
    
    public void AddChunk()
    {
        var chunk = _chunksBoofer[Random.Range(0, _chunksBoofer.Count)];
        _chunksBoofer.Remove(chunk);
        chunk.transform.SetParent(_rotateObject);
        chunk.transform.eulerAngles = new Vector3(57.6f, 0f,0f);

    }

    public void RemoveChunk(GameObject chunk)
    {
        chunk.transform.SetParent(_chunksParent);
        _chunksBoofer.Add(chunk);
    }

    public void GameOver()
    {
        _rotateObject.DOKill();
    }
}
