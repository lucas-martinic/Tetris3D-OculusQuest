using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int baseWidth = 7;

    public Transform allBlocks;

    //OPTIONS
    public int difficulty = 1;
    public TextMeshProUGUI difficultyText;
    public TextMeshProUGUI highscoreText;
    public Animator optionsPanel;

    public GameObject[] blocks;

    public GameObject floorTriggersObj;
    public FloorTrigger[] floorTriggers;

    public List<GameObject> blocksList;

    ScoreBox scoreBox;

    //SOUNDS
    public AudioSource blockFallSound;
    public AudioSource clearSound;
    public AudioSource gameOverSound;

    //Effects
    public ParticleSystem clearEffect;

    bool gameOver;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        floorTriggers = new FloorTrigger[floorTriggersObj.transform.childCount];
        for (int i = 0; i < floorTriggers.Length; i++)
        {
            floorTriggers[i] = floorTriggersObj.transform.GetChild(i).GetComponent<FloorTrigger>();
        }
        scoreBox = GameObject.FindObjectOfType<ScoreBox>();
        optionsPanel.SetTrigger("show");
        PlayerPrefs.GetInt("HighScore", 0);
        highscoreText.text = "Highscore: " + PlayerPrefs.GetInt("HighScore", 0).ToString();
    }

    // Update is called once per frame
    public void StartGame()
    {
        gameOver = false;
        optionsPanel.SetTrigger("hide");
        scoreBox.StartCounting();
        GameObject block = Instantiate(blocks[Random.Range(0, blocks.Length)], transform.position, Quaternion.identity);
        block.GetComponent<BlockController>().speed = difficulty;
    }

    public void ChangeDifficulty()
    {
        if(difficulty == 1)
        {
            difficulty++;
            difficultyText.text = "Difficulty: Normal";
        }
        else if(difficulty == 2)
        {
            difficulty++;
            difficultyText.text = "Difficulty: Hard";
        }
        else if(difficulty == 3)
        {
            difficulty = 1;
            difficultyText.text = "Difficulty: Easy";
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void Spawn(GameObject blockThatCollided)
    {
        if (blockThatCollided.transform.position == transform.position)
            EndGame();
        if (gameOver) return;
        StartCoroutine(CoSpawn());
        StartCoroutine(CheckFloors());
    }

    IEnumerator CoSpawn()
    {
        blockFallSound.Play();
        yield return new WaitForSeconds(0.2f);
        GameObject block = Instantiate(blocks[Random.Range(0, blocks.Length)], transform.position, Quaternion.identity);
        block.GetComponent<BlockController>().speed = difficulty;
    }

    IEnumerator CheckFloors()
    {
        yield return new WaitForFixedUpdate();
        for (int i = 0; i < floorTriggers.Length; i++)
        {
            if(floorTriggers[i].currentBoxes == 49)
            {
                StartCoroutine(Score(i));
            }
        }
    }

    IEnumerator Score(int n)
    {
        clearEffect.transform.position = floorTriggers[n].transform.position;
        clearEffect.Play();
        scoreBox.Score(100);
        clearSound.Play();
        blocksList = new List<GameObject>();
        for (int i = 0; i < floorTriggers[n].blocksColliding.Count; i++)
        {
            blocksList.Add(floorTriggers[n].blocksColliding[i]);
        }
        for (int i = 0; i < blocksList.Count; i++)
        {
            floorTriggers[n].blocksColliding.Remove(blocksList[i]);
            Destroy(blocksList[i]);
        }
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(StationaryBlocksDown(n));
    }

    IEnumerator StationaryBlocksDown(int n)
    {
        Debug.Log("stationaryBlocksDown happens, n:" + n);
        for (int i = n + 1; i < floorTriggers.Length; i++)
        {
            Debug.Log(i);
            for (int j = 0; j < floorTriggers[i].blocksColliding.Count; j++)
            {
                Debug.Log(j);
                floorTriggers[i].blocksColliding[j].transform.position -= Vector3.up;
                Debug.Log(floorTriggers[i].blocksColliding[j].gameObject.name);
            }
            yield return new WaitForSeconds(0.01f);
        }
        StartCoroutine(CheckFloors());
    }

    public void EndGame()
    {
        StartCoroutine(CoEndGame());
        gameOver = true;
        gameOverSound.Play();
        scoreBox.StopCounting();
        if(scoreBox.fullScore > PlayerPrefs.GetInt("HighScore"))
            PlayerPrefs.SetInt("HighScore", scoreBox.fullScore);
        highscoreText.text = "Highscore: " + PlayerPrefs.GetInt("HighScore", 0).ToString();
        optionsPanel.SetTrigger("show");

    }

    IEnumerator CoEndGame()
    {
        yield return new WaitForSeconds(1f);
        int i = 0;
        GameObject[] allChildren = new GameObject[allBlocks.childCount];

        //Find all child obj and store to that array
        foreach (Transform child in allBlocks)
        {
            allChildren[i] = child.gameObject;
            i += 1;
        }

        //Now destroy them
        foreach (GameObject child in allChildren)
        {
            DestroyImmediate(child.gameObject);
            yield return new WaitForSeconds(0.03f);
        }
    }
}
