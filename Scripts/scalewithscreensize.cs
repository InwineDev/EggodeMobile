using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scalewithscreensize : MonoBehaviour
{
    private float _initialScale;

    private void Start()
    {
        _initialScale = transform.localScale.x;
    }

    private void Update()
    {
        float newScale = _initialScale * (Screen.width / 1920f);
        float newScale2 = _initialScale * (Screen.width / 2100f);

        transform.localScale = new Vector3(newScale, newScale2, newScale);
    }
}
