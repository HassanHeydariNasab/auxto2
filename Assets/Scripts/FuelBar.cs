using UnityEngine;
using UnityEngine.UI;

class Fuel : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private RectTransform _rectTransform;

    public void OnChangeScore(int score)
    {
        _rectTransform.localScale = new Vector2(score * 0.01f, 0);
    }

    public void OnChangeColor(Color color)
    {
        _image.color = color;
    }
}