using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRayScreen : MonoBehaviour
{
    public XRayPlate CurrentPlate;

    public void Show()
    {
        if (CurrentPlate == null) return;
        Material m = gameObject.GetComponent<MeshRenderer>().material;
        m.color = new Color(0.5f, 0.5f, 0.5f, 1);
        m.mainTexture = CurrentPlate.Image;
        gameObject.GetComponent<MeshRenderer>().material = m;
    }

    public void Hide()
    {
        Material m = gameObject.GetComponent<MeshRenderer>().material;
        m.color = new Color(0, 0, 0, 1);
        gameObject.GetComponent<MeshRenderer>().material = m;
    }
}
