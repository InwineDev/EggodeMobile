using UnityEngine;

[CreateAssetMenu(fileName = "ConsoleCommand", menuName = "ScriptableObjects/NewConsoleCommand")]
public class ConsoleCommand : ScriptableObject
{
    public string command;

    public string method;
}