using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;

public class AIEmployee : MonoBehaviour
{
    public string Name;
    public NNModel NeuralModel;

    private IWorker worker;
    
    void Start()
    {
        worker = NeuralModel.CreateWorker();
    }

   public Rect Recognize(Texture2D image)
    {
        using (var input = new Tensor(image, channels: 3))
        {
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
}
