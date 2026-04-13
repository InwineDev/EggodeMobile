using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class emotectrl : NetworkBehaviour
{
    public Animator anim;

    public void onanimation(int animnumber)
    {
        if (isLocalPlayer)
        {
            anim.Play(animnumber.ToString());
        }
    }
}
