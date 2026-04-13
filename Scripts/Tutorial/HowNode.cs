using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HowNode : MonoBehaviour
{
    [SerializeField] private GameObject how;
    public void OnPointerEnter(PointerEventData eventData)
    {
        how.SetActive(true);
    }    

    public void OnPointerUp(PointerEventData eventData)
    {
        how.SetActive(false);
    }
}
