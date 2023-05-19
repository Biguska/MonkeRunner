using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkController : MonoBehaviour
{
    [SerializeField] private ChunksGeneratorController _chunksGeneratorController;
    
    private void OnTriggerEnter(Collider obj)
    {
        if (obj.gameObject.CompareTag("ChunkSpawn"))
        {
            Debug.Log("Spawn");
            _chunksGeneratorController.AddChunk();
        }

        if (obj.gameObject.CompareTag("ChunkRemove"))
        {
            Debug.Log("Remove");
            _chunksGeneratorController.RemoveChunk(gameObject);
            ResetChunk();
        }
    }

    private void ResetChunk()
    {
    }
}
