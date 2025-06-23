using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreTracker : MonoBehaviour
{
    public TMP_Text ScoreTextPrefab;
    public GridLayoutGroup Grid;

    private Dictionary<string, int> scores = new Dictionary<string, int>();

    public void AddPlayer(string player)
    {
        scores.Add(player, 0);
    }

    public void Evaluate(string player, Rect rect, Rect gt)
    {
        float iou = calculateIoU(rect, gt);
        int score = (int)System.Math.Round(10 * iou);
        if (scores.ContainsKey(player))
        {
            scores[player] += score;
        }
        else
        {
            scores.Add(player, score);
        }
    }

    public void Show()
    {
        foreach (Transform child in Grid.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (var item in scores.OrderByDescending(x => x.Value))
        {
            var name = Instantiate(ScoreTextPrefab, Grid.transform);
            name.text = item.Key;
            var score = Instantiate(ScoreTextPrefab, Grid.transform);
            score.text = $"<align=\"right\">{item.Value}";
        }
    }

    private static float calculateIoU(Rect rect1, Rect rect2)
    {
        float xInter1 = System.Math.Max(rect1.xMin, rect2.xMin);
        float yInter1 = System.Math.Max(rect1.yMin, rect2.yMin);
        float xInter2 = System.Math.Min(rect1.xMax, rect2.xMax);
        float yInter2 = System.Math.Min(rect1.yMax, rect2.yMax);
        float wInter = xInter2 - xInter1;
        float hInter = yInter2 - yInter1;
        float areaInter = wInter * hInter;
        float areaUnion = rect1.width * rect1.height + rect2.width * rect2.height - areaInter;
        return System.Math.Clamp(areaInter / areaUnion, 0, 1);
    }
}
