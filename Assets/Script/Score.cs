using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Score : MonoBehaviour
{
    public Text ScoreText;
    static public int score;
    // Start is called before the first frame update
    void Start()
    {
        ScoreText.text = score.ToString();
    }
    void Update()
    {
        ScoreText.text = score.ToString();
    }

    // Update is called once per frame

}
