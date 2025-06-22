using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Oculus.Interaction;

public class XRayScreen : MonoBehaviour
{
    public RayInteractor Interactor;
    public VRInputManager VRInput;

    public float Thickness = 0.005f;
    public Color LineColor = new Color(1, 0, 0);

    public UnityEvent<Rect> ResultEvent;

    public XRayPlate CurrentPlate;
    public Rect? CurrentRect;

    bool shown = false;
    bool selecting = false;
    Vector2 point1;
    Vector2 point2;

    void Start()
    {
        VRInput.PrimaryButtonEvent.AddListener(onPrimaryButtonEvent);
        VRInput.TriggerEvent.AddListener(onTriggerEvent);
    }

    private void onPrimaryButtonEvent(bool pressed)
    {
        Debug.Log($"{shown} ; {pressed} ; {CurrentRect}");
        if (shown && pressed && CurrentRect is not null)
        {
            Hide();
            ResultEvent.Invoke(CurrentRect.Value);
        }
    }

    private void onTriggerEvent(bool pressed)
    {
        if (!shown) return;
        if (pressed)
        {
            if (Interactor.CollisionInfo is not null)
            {
                selecting = true;
                point1 = onSurface(Interactor.CollisionInfo.Value.Point);

                reset();
            }
        }
        else
        {
            selecting = false;
            CurrentRect = Rect.MinMaxRect(System.Math.Min(point1.x, point2.x), System.Math.Min(point1.y, point2.y),
                System.Math.Max(point1.x, point2.x), System.Math.Max(point1.y, point2.y));

            Material m = gameObject.GetComponent<MeshRenderer>().material;
            m.mainTexture = drawRect(CurrentRect.Value);
        }
    }

    void Update()
    {
        if (selecting && Interactor.CollisionInfo is not null)
        {
            point2 = onSurface(Interactor.CollisionInfo.Value.Point);
        }
    }

    public void Show()
    {
        if (CurrentPlate == null) return;
        reset();
        shown = true;
    }

    public void Hide()
    {
        Material m = gameObject.GetComponent<MeshRenderer>().material;
        m.color = new Color(11.0f/255, 13.0f/255, 13.0f/255);
        m.mainTexture = null;
        shown = false;
    }

    void reset()
    {
        Material m = gameObject.GetComponent<MeshRenderer>().material;
        m.color = new Color(0.5f, 0.5f, 0.5f);
        m.mainTexture = CurrentPlate.Image;
        CurrentRect = null;
    }

    Vector2 onSurface(Vector3 vec)
    {
        var bounds = gameObject.GetComponent<MeshRenderer>().bounds;
        return new Vector2(
                Vector3.Dot(vec - bounds.max, new Vector3(-2*bounds.size.x, 0, 0)),
                Vector3.Dot(vec - bounds.max, new Vector3(0, -2*bounds.size.y, 0))
            );
    }

    Texture2D drawRect(Rect rect)
    {
        Rect inner = new Rect(rect.x + Thickness, rect.y + Thickness,
            rect.width - 2 * Thickness, rect.height - 2 * Thickness);
        Texture2D texture = new Texture2D(CurrentPlate.Image.width, CurrentPlate.Image.height);
        var pixels = CurrentPlate.Image.GetPixels();
        for (int i = 0; i < CurrentPlate.Image.width; i++)
        {
            for (int j = 0; j < CurrentPlate.Image.height; j++)
            {
                var point = new Vector2((float)i / CurrentPlate.Image.width, 1.0f - (float)j / CurrentPlate.Image.height);
                if (rect.Contains(point) && !inner.Contains(point))
                {
                    pixels[i + j * CurrentPlate.Image.width] = LineColor;
                }
            }
        }
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
}
