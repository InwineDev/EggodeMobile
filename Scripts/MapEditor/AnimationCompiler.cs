using EasyTextEffects.Editor.MyBoxCopy.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using static Animat;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class AnimationCompiler : MonoBehaviour
{
    [SerializeField] private strelka_pos stlp;

    public GameObject framePrefab;
    public GameObject contentField;
    public List<GameObject> frames = new List<GameObject>();
    public string loop = "false";
    public Toggle loopToggle;

    public void UpdateAnimations()
    {
        ClearAllFrames();

        scriptor vibron = stlp.vibron.GetComponent<scriptor>();
        if (vibron == null || string.IsNullOrEmpty(vibron.Animation)) return;

        AnimationData animationData = JsonUtility.FromJson<AnimationData>(vibron.Animation);
        if (animationData == null || animationData.frames == null) return;

        loop = animationData.loop;
        loopToggle.isOn = bool.Parse(loop);

        foreach (var item in animationData.frames)
        {
            CreateFrameFromData(item);
        }
    }

    public void ChangeLoop(bool isLooping)
    {
        loop = isLooping.ToString().ToLower();
    }

    public void CreateFrame()
    {
        GameObject prefabLocalFrame = Instantiate(framePrefab, contentField.transform);
        frames.Add(prefabLocalFrame);

        FrameData frameData = prefabLocalFrame.GetComponent<FrameData>();
        if (frameData == null) return;

        Transform targetTransform = stlp.vibron.transform;
        string posStr = $"{targetTransform.position.x}.{targetTransform.position.y}.{targetTransform.position.z}";
        string rotStr = $"{targetTransform.localEulerAngles.x}.{targetTransform.localEulerAngles.y}.{targetTransform.localEulerAngles.z}";
        string scaleStr = $"{targetTransform.localScale.x}.{targetTransform.localScale.y}.{targetTransform.localScale.z}";

        frameData.Apply(posStr, rotStr, scaleStr, "1");
    }

    public string GetCompiledAnimationAsJson()
    {
        List<FramesData> compiledFrames = new List<FramesData>();

        foreach (var frameObj in frames)
        {
            if (frameObj == null) continue;

            FrameData frameData = frameObj.GetComponent<FrameData>();
            if (frameData == null) continue;

            string posStr = frameData.position != null && frameData.position.Length >= 3 ?
                $"{frameData.position[0]}.{frameData.position[1]}.{frameData.position[2]}" : "0.0.0";

            string rotStr = frameData.rotation != null && frameData.rotation.Length >= 3 ?
                $"{frameData.rotation[0]}.{frameData.rotation[1]}.{frameData.rotation[2]}" : "0.0.0";

            string scaleStr = frameData.scale != null && frameData.scale.Length >= 3 ?
                $"{frameData.scale[0]}.{frameData.scale[1]}.{frameData.scale[2]}" : "1.1.1";

            compiledFrames.Add(new FramesData()
            {
                pos = posStr,
                rotate = rotStr,
                scale = scaleStr,
                speed = frameData.speed.text
            });
        }

        AnimationData animationData = new AnimationData()
        {
            frames = compiledFrames,
            loop = loop // Добавляем параметр loop в экспортируемые данные
        };

        return JsonUtility.ToJson(animationData, prettyPrint: true);
    }

    private void ClearAllFrames()
    {
        for (int i = frames.Count - 1; i >= 0; i--)
        {
            if (frames[i] != null)
            {
                Destroy(frames[i]);
            }
        }
        frames.Clear();
    }

    public void ClearFrame(GameObject frame)
    {
        frames.Remove(frame);
        Destroy(frame);
    }

    private void CreateFrameFromData(FramesData frameData)
    {
        GameObject prefabLocalFrame = Instantiate(framePrefab, contentField.transform);
        frames.Add(prefabLocalFrame);

        FrameData frameComponent = prefabLocalFrame.GetComponent<FrameData>();
        if (frameComponent != null)
        {
            frameComponent.Apply(frameData.pos, frameData.rotate, frameData.scale, frameData.speed);
        }
    }
}