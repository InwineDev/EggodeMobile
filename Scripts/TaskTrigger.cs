using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskTrigger : MonoBehaviour
{
    [Header("Task Settings")]
    [TextArea()]
    public string taskText;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TaskController.showTask?.Invoke(taskText);
            Destroy(this);
        }
    }
}
