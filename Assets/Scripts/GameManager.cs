using System.Collections.Generic;
using UnityEngine;

class GameManager : MonoBehaviour
{
    public CarControl carControl;

    public readonly Color[] colors =
    {
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow,
        // Color.magenta,
        // Color.cyan,
        // Color.white,
        // Color.black,
        // Color.gray,
    };

    public readonly Color penaltyColor = Color.gray;

    public Color currentColor = Color.red;

    public int score = 0;

    public bool isPenalty = false;

    void Start()
    {

    }


    public void PickColor()
    {
        var newColor = colors[Random.Range(0, colors.Length)];
        carControl.OnChangeColor(newColor);
    }

    public void StartPenalty()
    {
        if (isPenalty) return;

        isPenalty = true;
        carControl.OnChangeColor(Color.gray);
        Invoke("EndPenalty", 2);
    }

    public void EndPenalty()
    {
        isPenalty = false;
        PickColor();
    }

}