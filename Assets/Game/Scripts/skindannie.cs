using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class skindannie : MonoBehaviour
{
    public int id;
    public Sprite skinimage;
    public string nam1e;
    public Image sus;
    public TMP_Text sus2;
    public userSettingNotCam userSettingNotCam;

    private void Start()
    {
        Starting();
    }

    public void Starting()
    {
        userSettingNotCam = FindObjectOfType<userSettingNotCam>();
        nam1e = userSettingNotCam.eggData.eggs[id].name;
        skinimage = userSettingNotCam.eggData.eggSprites[id];
        sus2.text = nam1e;
        sus.sprite = skinimage;
    }

    public void setskin()
    {
        userSettingNotCam.OnSkinChangedNoMP(id);
    }
}
