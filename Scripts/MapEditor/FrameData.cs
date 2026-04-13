using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class FrameData : MonoBehaviour
{
    public TMP_InputField[] xyz;
    public TMP_InputField speed;
    public string[] position = new string[3];
    public string[] rotation = new string[3];
    public string[] scale = new string[3];
    public float speedf;

    private void Awake()
    {
        position = position ?? new string[3];
        rotation = rotation ?? new string[3];
        scale = scale ?? new string[3];
    }

    public void DeleteFrame()
    {

    }

    public void Apply(string poss, string rotatee, string scalee, string speedd)
    {
        try
        {
            print(poss);
            position = ParseVector3(poss);
            rotation = ParseVector3(rotatee);
            scale = ParseVector3(scalee);
            speedf = float.Parse(speedd, CultureInfo.InvariantCulture);

            UpdateInputFields();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка в методе Apply: {e.Message}");
            SetDefaultValues();
        }
    }

    private void UpdateInputFields()
    {
        if (xyz == null || xyz.Length < 9)
        {
            Debug.LogError("Массив xyz не инициализирован или имеет недостаточную длину");
            return;
        }

        xyz[0].text = position[0].ToString(CultureInfo.InvariantCulture);
        xyz[1].text = position[1].ToString(CultureInfo.InvariantCulture);
        xyz[2].text = position[2].ToString(CultureInfo.InvariantCulture);

        xyz[3].text = rotation[0].ToString(CultureInfo.InvariantCulture);
        xyz[4].text = rotation[1].ToString(CultureInfo.InvariantCulture);
        xyz[5].text = rotation[2].ToString(CultureInfo.InvariantCulture);

        xyz[6].text = scale[0].ToString(CultureInfo.InvariantCulture);
        xyz[7].text = scale[1].ToString(CultureInfo.InvariantCulture);
        xyz[8].text = scale[2].ToString(CultureInfo.InvariantCulture);

        speed.text = speedf.ToString(CultureInfo.InvariantCulture);
    }

    private void SetDefaultValues()
    {
        position = new string[3];
        rotation = new string[3];
        scale = new string[3];
        speedf = 1f;
    }

    private string[] ParseVector3(string vectorString)
    {
        if (string.IsNullOrEmpty(vectorString))
            return new string[3];
        string[] values = vectorString.Split(new char[] { '.' }, System.StringSplitOptions.RemoveEmptyEntries);

        if (values.Length < 3)
        {
            Debug.LogWarning($"Недостаточно значений в строке вектора: {vectorString}");
            return new string[3];
        }

        /*float[] result = new float[3];
        for (int i = 0; i < 3; i++)
        {
            if (!float.TryParse(values[i].Trim(), NumberStyles.Any,
                      CultureInfo.InvariantCulture, out result[i]))
            {
                Debug.LogWarning($"Не удалось распарсить значение: {values[i]}");
                result[i] = 0f;
            }
        }*/

        return values;
    }
}