using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class EventsStarter : NetworkBehaviour
{
    public List<RandomEventObject> events = new List<RandomEventObject>();

    private void Start()
    {
        Invoke("Event", Random.Range(11, 120));
    }
    [Server]
    void Event()
    {
        if (serverProperties.instance.isEvents) return;
        int count = Random.Range(0, events.Count);
        events[count].Raise();
        Invoke("Event", Random.Range(0, 120));
    }

}
