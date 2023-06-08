using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkController : MonoBehaviour
{
    [SerializeField] private ChunksGeneratorController _chunksGeneratorController;
    [SerializeField] private List<BananaController> _bananaList;
    [SerializeField] private bool _isStart = false;
    
    private void OnTriggerEnter(Collider obj)
    {
        if (obj.gameObject.CompareTag("ChunkSpawn"))
        {
            _chunksGeneratorController.AddChunk();
        }

        if (obj.gameObject.CompareTag("ChunkRemove"))
        {
            if (!_isStart)
            {
                _chunksGeneratorController.RemoveChunk(gameObject);
                ResetChunk();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private void ResetChunk()
    {
        _bananaList.ForEach(banana => banana.Reset());
    }
}
