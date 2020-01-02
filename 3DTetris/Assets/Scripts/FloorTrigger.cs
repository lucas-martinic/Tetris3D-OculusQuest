using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTrigger : MonoBehaviour
{
    public int fullFloor = 49;
    public int currentBoxes;
    public List<GameObject> blocksColliding;
    // Start is called before the first frame update
    void Start()
    {
        blocksColliding = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        currentBoxes++;
        blocksColliding.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        currentBoxes--;
        blocksColliding.Remove(other.gameObject);
    }
}
