using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChatOBJController : NetworkBehaviour
{
    [SerializeField] private TMP_Text me;
    [SerializeField] private Animator anim;

    [SerializeField] private TTSChatController ttska;
    public void StartSpeech()
  => ttsrust_say(me.text);

#if !UNITY_EDITOR && (UNITY_IOS || UNITY_WEBGL)
    const string _dll = "__Internal";
#else
    const string _dll = "ttsrust";
#endif

    [DllImport(_dll)] static extern void ttsrust_say(string text);

    void OnEnable()
    {
        StartSpeech();
        StartCoroutine(WaitForSpeechEnd());
        anim.SetBool("GO", true);
    }

    IEnumerator WaitForSpeechEnd()
    {
        float duration = me.text.Length / 2;

        yield return new WaitForSeconds(duration);
        anim.SetBool("GO", false);
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);

    }

}