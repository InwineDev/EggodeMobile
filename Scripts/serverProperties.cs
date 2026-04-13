using Mirror;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;

public class serverProperties : NetworkBehaviour
{
    public static serverProperties instance;

    [SyncVar]
    public bool hp = true;

    [SyncVar]
    public bool spawnn = true;

    [SyncVar]
    public bool destroy = true;

    [SyncVar]
    public string versionServer;

    [SyncVar]
    public bool isEvents = false;

    [SyncVar]
    public SyncList<string> serverMods = new SyncList<string>();

    [SyncVar]
    public SyncList<name24> allBlocks = new SyncList<name24>();

    [SyncVar]
    public bool survival = false;

    public GameObject item228;

    public Material[] skyboxes;

    [SyncVar(hook = nameof(SetSkybox))]
    public int skybox;

    [SyncVar(hook = nameof(SetDieCord))]
    public string dieCord;

    public Vector3 dieCordReally =  new Vector3(0,2,0);

    [SerializeField] private Volume skyVolume;

    private void Start()
    {
        instance = this;
    }

/*    private void Update()
    {
        hp = false;
    }*/


    private void SetSkybox(int oldv, int newv)
    {
        /*if (skyVolume == null || skyVolume.sharedProfile == null || skyboxes == null)
        {
            return;
        }

        if (newv < 0 || newv >= skyboxes.Length)
        {
            return;
        }

        if (skyVolume.sharedProfile.TryGet<HDRISky>(out var sky) && sky != null)
        {
            sky.hdriSky = skyboxes[newv];
        }*/
        /*        // Find or Add the VisualEnvironment and HDRISky
                VisualEnvironment visualEnv;
                HDRISky hdriSky;

                if (!skyVolume.profile.TryGet(out visualEnv))
                    visualEnv = skyVolume.profile.Add<VisualEnvironment>(false);

                if (!skyVolume.profile.TryGet(out hdriSky))
                    hdriSky = skyVolume.profile.Add<HDRISky>(false);

                // Change the sky type to HDRI Sky
                visualEnv.skyType.value = (int)SkyType.HDRI;

                // Change the HDRISky component's texture and intensity
                hdriSky.hdriSky.value = skyboxes[newv];
                hdriSky.exposure.value = 16f; // Example exposure value
                hdriSky.multiplier.value = 1f; // Example intensity multiplier
        *//*
                // Enable Overrides
                visualEnv.overrideSkyType.value = true;
                hdriSky.overrideHDRI.value = true;
                hdriSky.overrideExposure.value = true;
                hdriSky.overrideMultiplier.value = true;*/
        RenderSettings.skybox = skyboxes[newv];
    }

    private void SetDieCord(string oldv, string newv)
    {
        string cleanedString = newv.TrimEnd('\u200B');
        string[] elements = cleanedString.Split(',');

        if (elements.Length == 3)
        {
            if (float.TryParse(elements[0].Trim(), out float x) &&
                float.TryParse(elements[1].Trim(), out float y) &&
                float.TryParse(elements[2].Trim(), out float z))
            {
                Vector3 sus = new Vector3(x, y, z);
                dieCordReally = sus;
            }
            else
            {
                print("Ошибка: Некорректный формат координат.");
            }
        }
    }

}
