using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    public float speed;
    float counter = 0;
    public bool falling = true;
    bool inside;
    Vector3 currentPos;
    Quaternion currentRot;

    Transform triggerCollider;
    GameObject renderers;

    int totalBlocks;
    int blocksInside;

    bool didCollide;

    bool moving;

    Coroutine cor;

    float initialDelay = 0.5f;
    bool initialPause = true;

    //Touch controllers
    bool leftThumbstickCentered = true;
    bool rightThumbstickCentered = true;

    // Start is called before the first frame update
    void Start()
    {
        triggerCollider = transform.GetChild(0);
        renderers = transform.GetChild(1).gameObject;
        totalBlocks = triggerCollider.transform.childCount;
    }

    // Update is called once per frame
    void Update()
    {
        counter += Time.deltaTime;
        if (counter >= initialDelay) initialPause = false;
        if (initialPause) return;
        if (!falling) return;

        Vector2 leftThumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        Vector2 rightThumbstick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);


        //Oculus Input

        //MOVEMENT
        if(leftThumbstick.x > 0.8f && leftThumbstickCentered)
        {
            if (moving) return;
            leftThumbstickCentered = false;
            cor = StartCoroutine(TryToMove(Vector3.right));
        }
        if (leftThumbstick.x < -0.8f && leftThumbstickCentered)
        {
            if (moving) return;
            leftThumbstickCentered = false;
            cor = StartCoroutine(TryToMove(Vector3.left));
        }
        if (leftThumbstick.y > 0.8f && leftThumbstickCentered)
        {
            if (moving) return;
            leftThumbstickCentered = false;
            cor = StartCoroutine(TryToMove(Vector3.forward));
        }
        if (leftThumbstick.y < -0.8f && leftThumbstickCentered)
        {
            if (moving) return;
            leftThumbstickCentered = false;
            cor = StartCoroutine(TryToMove(Vector3.back));
        }
        if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick))
        {
            if (moving) return;
            StartCoroutine(TryToFall());
        }

        //ROTATION
        if (rightThumbstick.x < -0.8f && rightThumbstickCentered)
        {
            if (moving) return;
            rightThumbstickCentered = false;
            cor = StartCoroutine(TryToRotate(Vector3.forward, -90));
        }
        if (rightThumbstick.x > 0.8f && rightThumbstickCentered)
        {
            if (moving) return;
            rightThumbstickCentered = false;
            cor = StartCoroutine(TryToRotate(Vector3.forward, 90));
        }
        if (rightThumbstick.y > 0.8f && rightThumbstickCentered)
        {
            if (moving) return;
            rightThumbstickCentered = false;
            cor = StartCoroutine(TryToRotate(Vector3.right, 90));
        }
        if (rightThumbstick.y < -0.8f && rightThumbstickCentered)
        {
            if (moving) return;
            rightThumbstickCentered = false;
            cor = StartCoroutine(TryToRotate(Vector3.right, -90));
        }
        if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick))
        {
            if (moving) return;
            cor = StartCoroutine(TryToRotate(Vector3.up, 90));
        }

        if (leftThumbstick.magnitude < 0.7f)
        {
            leftThumbstickCentered = true;
        }

        if (rightThumbstick.magnitude < 0.7f)
        {
            rightThumbstickCentered = true;
        }

        if (counter >= 1/speed)
        {
            StartCoroutine(TryToFall());
        }


        //Keyboard Input
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (moving) return;
            cor = StartCoroutine(TryToMove(Vector3.right));
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (moving) return;
            cor = StartCoroutine(TryToMove(Vector3.left));
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (moving) return;
            cor = StartCoroutine(TryToMove(Vector3.forward));
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (moving) return;
            cor = StartCoroutine(TryToMove(Vector3.back));
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (moving) return;
            cor = StartCoroutine(TryToRotate(Vector3.right));
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            if (moving) return;
            cor = StartCoroutine(TryToRotate(Vector3.forward));
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (moving) return;
            cor = StartCoroutine(TryToRotate(Vector3.up));
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (moving) return;
            StartCoroutine(TryToFall());
        }
    }

    IEnumerator TryToFall()
    {
        counter = 0;
        yield return new WaitForFixedUpdate();
        if (cor != null)
        {
            StopCoroutine(cor);
            triggerCollider.transform.localPosition = Vector3.zero;
            triggerCollider.transform.localRotation = Quaternion.identity;
        }
        moving = false;
        /*triggerCollider.transform.position = currentPos;
        triggerCollider.transform.rotation = currentRot;*/
        currentPos = triggerCollider.transform.position;
        triggerCollider.transform.position += -Vector3.up;
        yield return new WaitForFixedUpdate();
        if (didCollide)
        {
            triggerCollider.transform.position = currentPos;
            StartCoroutine(OnCollision());
        }
        else
        {
            triggerCollider.transform.position = currentPos;
            transform.position += -Vector3.up;
        }
    }

    IEnumerator TryToMove(Vector3 direction)
    {
        moving = true;
        yield return new WaitForFixedUpdate();
        currentPos = triggerCollider.transform.position;
        triggerCollider.transform.position += direction;
        yield return new WaitForFixedUpdate();
        if (!inside || didCollide)
        {
            triggerCollider.transform.position = currentPos;
        }
        else
        {
            triggerCollider.transform.position = currentPos;
            transform.position += direction;
        }
        didCollide = false;
        moving = false;
        didCollide = false;
    }

    IEnumerator TryToRotate(Vector3 axis)
    {
        moving = true;
        yield return new WaitForFixedUpdate();
        currentRot = triggerCollider.transform.rotation;
        triggerCollider.transform.Rotate(axis, 90);
        yield return new WaitForFixedUpdate();
        if (!inside)
        {
            triggerCollider.transform.rotation = currentRot;
        }
        else
        {
            triggerCollider.transform.rotation = currentRot;
            transform.Rotate(axis, 90);
        }
        moving = false;
    }

    IEnumerator TryToRotate(Vector3 axis, int angle)
    {
        moving = true;
        yield return new WaitForFixedUpdate();
        currentRot = triggerCollider.transform.rotation;
        triggerCollider.transform.Rotate(axis, angle);
        yield return new WaitForFixedUpdate();
        if (!inside)
        {
            triggerCollider.transform.rotation = currentRot;
        }
        else
        {
            triggerCollider.transform.rotation = currentRot;
            transform.Rotate(axis, angle);
        }
        moving = false;
    }

    IEnumerator OnCollision()
    {
        yield return new WaitForFixedUpdate();
        if (falling)
            GameManager.instance.Spawn(gameObject);
        falling = false;
        triggerCollider.transform.position = currentPos;
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < triggerCollider.transform.childCount; i++)
        {
            triggerCollider.GetChild(i).GetComponent<Renderer>().enabled = true;
        }
        yield return new WaitForEndOfFrame();
        transform.SetParent(GameManager.instance.allBlocks);
        this.enabled = false;
        renderers.SetActive(false);
    }

    public void IsOutside()
    {
        blocksInside--;
        if(blocksInside < totalBlocks)
            inside = false;
    }

    public void IsInside()
    {
        blocksInside++;
        if (blocksInside == totalBlocks)
            inside = true;
    }

    public void DidCollide()
    {
        didCollide = true;
    }
}
