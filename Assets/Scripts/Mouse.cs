using UnityEngine;

public class Mouse : MonoBehaviour
{
    public WalizkiController controller;

    void OnMouseDown()
    {
        Debug.Log("click!");
        if (controller != null && !controller.movingLuggage)
        {
            controller.SpawnBaggage();
        }
    }
}
