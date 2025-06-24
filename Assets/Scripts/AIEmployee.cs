using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;

public class AIEmployee : MonoBehaviour
{
    public string Name;
    public NNModel NeuralModel;

    private Model model;
    private IWorker worker;
    
    void Start()
    {
        model = ModelLoader.Load(NeuralModel);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model);
    }

   public Rect Recognize(Texture2D image)
    {
        image = resize(image, model.inputs[0].shape[5], model.inputs[0].shape[6]);
        using (var input = new Tensor(image, channels: 3))
        {
            Debug.Log((input.width, input.height));
            var output = worker.Execute(input).PeekOutput("output0");
            var f = output[0];
            int argmax = getArgmax(output);
            float x = output[0, 0, argmax, 0] / input.width;
            float y = output[0, 0, argmax, 1] / input.height;
            float w = output[0, 0, argmax, 2] / input.width;
            float h = output[0, 0, argmax, 3] / input.height;
            return new Rect(x - w / 2, y - h / 2, w, h);
        }
    }

    private int getArgmax(Tensor t)
    {
        float max = 0;
        int argmax = 0;
        for (int i = 0; i < t.width; i++)
        {
            for (int c = 4; c < t.channels; c++)
            {
                if (t[0, 0, i, c] > max)
                {
                    max = t[0, 0, i, c];
                    argmax = i;
                }
            }
        }
        return argmax;
    }

    private Texture2D resize(Texture2D texture2D, int targetX, int targetY)
    {
        RenderTexture rt = new RenderTexture(targetX, targetY, 24);
        RenderTexture.active = rt;
        Graphics.Blit(texture2D, rt);
        Texture2D result = new Texture2D(targetX, targetY);
        result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
        result.Apply();
        return result;
    }
}
