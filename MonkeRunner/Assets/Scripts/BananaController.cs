using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BananaController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private BoxCollider _boxCollider;
    [SerializeField] private GameObject _model;

    public void Reset()
    {
        _animator.SetTrigger("Reset");
        _boxCollider.enabled = true;
        _model.SetActive(true);
    }

    public void Disable()
    {
        _animator.SetTrigger("Reset");
        _boxCollider.enabled = false;
        _model.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("BananaTrigger"))
        {
            _animator.SetTrigger("Rotate");
        }
    }
}
