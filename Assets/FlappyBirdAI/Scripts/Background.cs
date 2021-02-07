using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public GameObject otherBG;

    Vector3 startPos;
    private void Start()
    {
        startPos = transform.position;
    }
    private void OnBecameInvisible()
    {
        if (otherBG != null)
        {
            transform.position = otherBG.transform.position + new Vector3(27.3f, 0, 0);
        }
    }

    public void ResetBackground()
    {
        transform.position = startPos;
    }
}
