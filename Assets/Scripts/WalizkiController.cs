using UnityEngine;

public class WalizkiController : MonoBehaviour
{
    [Header("Assign model prefabs here")]
    public GameObject[] modelPrefabs;

    public GameObject myszka;

    [Header("Movement settings")]
    public float moveSpeed = 2f;

    private readonly float StartX = -5f;

    private readonly float StopX = -2f;
    private readonly float EndX = 5f;

    private GameObject incomingBaggage;
    private GameObject outcomingBaggage;

    public bool movingLuggage = false;

    void Start()
    {
        if (modelPrefabs == null || modelPrefabs.Length == 0)
        {
            Debug.LogError("No model prefabs assigned!");
            return;
        }

    }


    public void SpawnBaggage()
    {
        // Select a random model
        int index = Random.Range(0, modelPrefabs.Length);
        int rotated = Random.Range(0, 2);
        Vector3 startPosition = modelPrefabs[index].transform.position;
        startPosition.x = StartX;
        incomingBaggage = Instantiate(modelPrefabs[index], startPosition, modelPrefabs[index].transform.rotation);
        if (rotated == 1)
        {
            incomingBaggage.transform.Rotate(new Vector3(0, 0, 180));
        }

        movingLuggage = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && !movingLuggage)
        {
            SpawnBaggage();
        }

        if (movingLuggage)
        {
            if (incomingBaggage != null)
            {
                incomingBaggage.transform.position += moveSpeed * Time.deltaTime * Vector3.right;
            }

            if (outcomingBaggage != null)
            {
                outcomingBaggage.transform.position += moveSpeed * Time.deltaTime * Vector3.right;
            }

            if (incomingBaggage != null & incomingBaggage.transform.position.x >= StopX)
            {
                if (outcomingBaggage != null)
                {
                    Destroy(outcomingBaggage);
                }
                outcomingBaggage = incomingBaggage;
                movingLuggage = false;
            }
        }
        
    }
}
