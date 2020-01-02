using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreBox : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    float counter;
    int counterInt;
    int score;
    public int fullScore;
    bool counting;

    public void StartCounting()
    {
        counter = 0;
        score = 0;
        counterInt = 0;
        counting = true;
    }

    public void StopCounting()
    {
        counting = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (counting)
        {
            counter += Time.deltaTime;
            counterInt = Mathf.RoundToInt(counter) * 10;
            fullScore = counterInt + score;
            scoreText.text = fullScore.ToString();
        }
    }

    public void Score(int scoreToAdd)
    {
        score += scoreToAdd;
    }
}
