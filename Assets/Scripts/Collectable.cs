using UnityEngine;

public class Collectable : MonoBehaviour
{
    public Color color = Color.red;
    private GameManager gameManager;
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        PickColor();

        transform.Translate(Vector3.up);

    }

    void Update()
    {
        transform.Translate(Vector3.up * Time.deltaTime * Mathf.Sin(Time.time * 10) * 0.5f);
    }

    void PickColor()
    {
        color = gameManager.colors[Random.Range(0, gameManager.colors.Length)];
        GetComponent<Light>().color = color;
        GetComponent<MeshRenderer>().materials[0].color = color;
    }


    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            if (gameManager.currentColor == color)
            {
                gameManager.score += 1;
            }
            else
            {
                gameManager.StartPenalty();
            }
            Destroy(gameObject);
        }
    }
}
