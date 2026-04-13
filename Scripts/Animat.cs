using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animat : NetworkBehaviour
{
    public string animtext;
    private bool isAnimating = false;

    public void animstart()
    {
        if (!isAnimating)
        {
            print("ANIMSTARTED");
            isAnimating = true;
            StartCoroutine(Animate());
        }
        else
        {
            Debug.Log("Анимация уже запущена");
        }
    }

    private IEnumerator Animate()
    {
        AnimationData data = JsonUtility.FromJson<AnimationData>(animtext);

        foreach (var card in data.frames)
        {
            Vector3 targetPos = ParseVector3(card.pos);
            Vector3 targetRotation = ParseVector33(card.rotate);
            Vector3 targetScale = ParseVector32(card.scale);
            float speed;

            if (float.TryParse(card.speed, out speed))
            {

                while (transform.position != targetPos || transform.rotation != Quaternion.Euler(targetRotation) || transform.localScale != targetScale)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * speed);
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRotation), Time.deltaTime * speed);
                    transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, Time.deltaTime * speed);
                    yield return null;
                }

            }
            else
            {
                Debug.Log("Не удалось преобразовать скорость в число: " + card.speed);
            }
        }

        if (data.loop == "true")
        {
            StartCoroutine(Animate());
        } else
        {
            isAnimating = false;
        }

    }

    private Vector3 ParseVector3(string vectorString)
    {
        string[] values = vectorString.Split('.');
        if (values.Length != 3)
        {
            Debug.Log("Invalid vector format: " + vectorString);
            return Vector3.zero;
        }

        float x = float.Parse(values[0]);
        float y = float.Parse(values[1]);
        float z = float.Parse(values[2]);

        return new Vector3(x, y, z);
    }

    private Vector3 ParseVector32(string vectorString)
    {
        string[] values = vectorString.Split('.');
        if (values.Length != 3)
        {
            Debug.Log("Invalid vector format: " + vectorString);
            return Vector3.zero;
        }

        float x = float.Parse(values[0]);
        float y = float.Parse(values[1]);
        float z = float.Parse(values[2]);

        return new Vector3(x, y, z);
    }

    private Vector3 ParseVector33(string vectorString)
    {
        string[] values = vectorString.Split('.');
        if (values.Length != 3)
        {
            Debug.Log("Invalid vector format: " + vectorString);
            return Vector3.zero;
        }

        float y = float.Parse(values[0]);
        float x = float.Parse(values[1]);
        float z = float.Parse(values[2]);

        return new Vector3(x, y, z);
    }

    [System.Serializable]
    public class AnimationData
    {
        public string loop;
        public List<FramesData> frames;
    }

    [System.Serializable]
    public class FramesData
    {
        public string pos;
        public string rotate;
        public string scale;
        public string speed;
    }
}