using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiMusicSystem : NetworkBehaviour
{
    public List<AudioClip> clips = new List<AudioClip>();
    private AudioSource source;
    [Range(0.0F, 1.0F)]
    public float volume = 1;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        source.spatialBlend = 1;
        source.volume = volume;
    }
    public AudioClip GetRandomAudio()
    {
        return clips[Random.Range(0, clips.Count)];
    }

    public void PlayClip()
    {
        CmdPlayClip();
    }

    [Command]
    void CmdPlayClip()
    {
        RpcPlayClip();
    }

    [ClientRpc]
    void RpcPlayClip()
    {
        source.clip = clips[Random.Range(0, clips.Count)];
        source.Play(0);
    }
}
