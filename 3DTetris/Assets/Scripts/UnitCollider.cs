using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCollider : MonoBehaviour
{
    BlockController block;

    private void Awake()
    {
        block = GetComponentInParent<BlockController>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayArea"))
        {
            block.IsOutside();
            //Debug.Log("exit from: "+ other.gameObject.name);
        }
        if (other.CompareTag("Colliders"))
        {
            block.IsOutside();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayArea"))
        {
            block.IsInside();
            //Debug.Log("staying in: " + other.gameObject.name);
        }
        if (other.CompareTag("Colliders"))
        {
            block.DidCollide();
        }
    }
}
