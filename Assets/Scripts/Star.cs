using UnityEngine;

public class Star : MonoBehaviour
{
    public bool isGood = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!isGood)
        {
            gameObject.GetComponent<MeshRenderer>().material.color = Color.black;
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            collider.gameObject.GetComponentInParent<CarControl>().Score += isGood ? 1 : -1;
            Destroy(gameObject);
        }
    }
}
