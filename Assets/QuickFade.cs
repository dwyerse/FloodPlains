using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickFade : MonoBehaviour
{
    // Start is called before the first frame update
    public RectTransform rect;

    void Start()
    {
        if (!PlayerPrefs.GetString("Select Level").Equals("done"))
        {
            PlayerPrefs.SetString("Select Level", "done");
            LeanTween.alphaCanvas(GetComponent<CanvasGroup>(), 1, 2f).setLoopPingPong();
            LeanTween.scale(rect, new Vector3(1.1f, 1.1f, 1.1f), 2).setLoopPingPong();
        }
    }

}
