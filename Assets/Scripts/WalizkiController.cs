using UnityEngine;

public class WalizkiController : MonoBehaviour
{
    [Header("Assign model prefabs here")]
    public GameObject[] modelPrefabs;

    public AIEmployee[] AIEmployees;

    public GameObject myszka;

    public VRInputManager VRInput;

    public XRayScreen Screen;

    public ImageLoader XRayLoader;

    public ScoreTracker Score;

    [Header("Movement settings")]
    public float moveSpeed = 2f;

    private readonly float StartX = -5f;

    private readonly float StopX = -2f;
    private readonly float EndX = 5f;

    private GameObject incomingBaggage;
    private GameObject outcomingBaggage;

    private bool started = false;

    public bool movingLuggage = false;

    public XRayPlate CurrentPlate;

    void Start()
    {
        if (modelPrefabs == null || modelPrefabs.Length == 0)
        {
            Debug.LogError("No model prefabs assigned!");
            return;
        }

        Score.AddPlayer("Cz�owiek");
        foreach (var employee in AIEmployees)
        {
            Score.AddPlayer(employee.Name);
        }
        Score.Show();

        VRInput.PrimaryButtonEvent.AddListener(onPrimaryButtonEvent);
        XRayLoader.LoadingFinishedEvent.AddListener(onLoadingFinishedEvent);
        Screen.ResultEvent.AddListener(onResultEvent);
    }

    private void onPrimaryButtonEvent(bool pressed)
    {
        if (!started && pressed && !movingLuggage)
        {
            XRayLoader.LoadRandom();
            SpawnBaggage();
            started = true;
        }
    }

    private void onLoadingFinishedEvent(XRayPlate plate)
    {
        CurrentPlate = plate;
        Screen.CurrentPlate = plate;
        foreach (var employee in AIEmployees)
        {
            var rect = employee.Recognize(CurrentPlate.Image);
            Score.Evaluate(employee.Name, rect, CurrentPlate.Groundtruth);
        }
    }

    private void onResultEvent(Rect rect)
    {
        if (CurrentPlate == null) return;
        Score.Evaluate("Cz�owiek", rect, CurrentPlate.Groundtruth);
        Score.Show();
        XRayLoader.LoadRandom();
        SpawnBaggage();
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
                Screen.Show();
            }
        }
        
    }
}
