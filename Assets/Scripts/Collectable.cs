using UnityEngine;

public class Collectable : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        transform.Translate(Vector3.back);
        transform.Rotate(0, 0, Random.Range(0, 360));
    }

    void Update()
    {
        transform.Rotate(0, 0, Time.deltaTime * 300f);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            gameManager.starSound.Play();
            gameManager.Score += 1;
            Destroy(gameObject);
        }
    }
}
