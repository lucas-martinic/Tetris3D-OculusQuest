using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonScript : MonoBehaviour
{
    bool pressed;
    Animator anim;
    public UnityEvent onPush;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Push()
    {
        if(!pressed)
            StartCoroutine(CoPush());
    }

    IEnumerator CoPush()
    {
        pressed = true;
        anim.SetTrigger("push");
        onPush.Invoke();

        yield return new WaitForSeconds(1f);
        pressed = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Push();
    }
}
