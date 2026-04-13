using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "NewRandomEvent", menuName = "ScriptableObjects/Random event")]
public class RandomEventObject : ScriptableObject
{
    public float chance;
    public UnityEvent OnEventRaised;

    public void Raise()
    {
        OnEventRaised?.Invoke();
    }
}