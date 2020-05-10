using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LeanTween.move(gameObject, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 0.5f, gameObject.transform.position.z), 3f).setLoopPingPong()
                .setEase(LeanTweenType.easeInOutBack);
    }

}
