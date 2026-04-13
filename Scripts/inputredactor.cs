using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class inputredactor : MonoBehaviour
{
    public TMP_InputField inputField;
    public float scrollSpeed = 3f;
    public TMP_InputField iron;
    public bool pon;

    private void Start()
    {
        if(iron) scrollSpeed = float.Parse(iron.text);
        inputField.onValueChanged.AddListener(OnInputValueChanged);
    }

    public void POPKANEGRA()
    {
        pon = true;
    }


    public void DESPOPKANEGRA()
    {
        pon = false;
    }

    private void OnInputValueChanged(string value)
    {
        // При изменении значения в поле ввода обновляем значение переменной или другого объекта
        // Например, если у вас есть переменная для хранения числа, вы можете обновить ее здесь
    }

    private void Update()
    {
        if (pon)
        {
            if (iron) scrollSpeed = float.Parse(iron.text);
            // Получаем значение колесика мыши
            float scrollWheel = Input.GetAxis("Mouse ScrollWheel");

            // Если колесико мыши двигается, изменяем значение в поле ввода
            if (Mathf.Abs(scrollWheel) > 0.0f)
            {
                // Получаем текущее значение в поле ввода
                float currentValue;
                if (float.TryParse(inputField.text, out currentValue))
                {
                    // Изменяем значение на основе колесика мыши с учетом скорости
                    currentValue += scrollWheel * scrollSpeed;

                    // Устанавливаем новое значение в поле ввода
                    inputField.text = currentValue.ToString();
                }
            }
        }
    }
}