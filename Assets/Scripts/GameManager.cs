using System.Collections.Generic;
using UnityEngine;
using TMPro;

class GameManager : MonoBehaviour
{
    public CarControl carControl;

    public AudioSource starSound;
    public TMP_Text scoreText;

    public int starsCount = 0;

    private int _score;
    public int Score
    {
        get => _score;
        set
        {
            _score = value;
            scoreText.SetText(value.ToString() + " / " + starsCount.ToString());
            if (value == starsCount)
            {
                Application.Quit(0);
            }
        }
    }


    void Start()
    {
    }

}