using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class opener : MonoBehaviour
{
    public GameObject open1;
    public bool going;
    [SerializeField] private bool goUp;
    //[SerializeField] private bool oneButton;
    [SerializeField] private UIDOTweenAnimator doAnimator;

    public int bgIn = -1;
    public int bgOut = 0;

    private void Start()
    {
        try 
        { 
            doAnimator = open1.GetComponent<UIDOTweenAnimator>();
            doAnimator.onOutroComplete.AddListener(realClose);
        } 
        catch
        {
            Debug.LogError("[opener system memesa] We havent animator");
        }
        if (going)
        {
            open1.SetActive(false);
            //Invoke("close", 0.1f);
        }
    }

    public void OpenOrClose()
    {
        if (open1.activeSelf)
        {
            close();
        } else
        {
            open();
        }
    }

    public void open()
    {
        if (goUp)
        {
            open1.SetActive(true);
            open1.transform.localPosition = new Vector3(0, 0, 0);
        }
        else 
        {
            open1.SetActive(true);
        }
        if(doAnimator) doAnimator.PlayIntroAnimation();
        if (bgIn != -1) BackgroundController.ChangeBG?.Invoke(bgIn);
    }

    public void close()
    {
        if (doAnimator) 
        { 
            doAnimator.PlayOutroAnimation(); 
        } else
        {
            realClose();
        }

    }
    private void realClose()
    {
        if (goUp)
        {
            open1.transform.localPosition = new Vector3(0, 1500, 0);
        }
        else
        {
            open1.SetActive(false);
        }
        if (bgIn != -1) BackgroundController.ChangeBG?.Invoke(bgOut);
    }
}
