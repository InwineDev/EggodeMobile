using UnityEngine;
using System;
using System.IO;

public class ExceptionLogger : MonoBehaviour
{
   /* private string logFilePath;

    void Awake()
    {
        // Убедимся, что только один экземпляр этого скрипта существует
        if (FindObjectsOfType<ExceptionLogger>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        // Создаем путь к файлу лога
        logFilePath = Path.Combine(Application.persistentDataPath, "game_crashes_log.txt");

        // Подписываемся на событие логгирования исключений
        Application.logMessageReceived += HandleException;

        Debug.Log("ExceptionLogger initialized. Log file at: " + logFilePath);
    }

    void HandleException(string logString, string stackTrace, LogType type)
    {
        // Нас интересуют только ошибки и исключения
        if (type == LogType.Exception || type == LogType.Error)
        {
            // Формируем запись для лога
            string logEntry = $"[{DateTime.Now}]\n" +
                              $"Type: {type}\n" +
                              $"Message: {logString}\n" +
                              $"Stack Trace: {stackTrace}\n" +
                              "----------------------------------------\n";

            // Записываем в файл
            try
            {
                File.AppendAllText(logFilePath, logEntry);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to write to log file: " + e.Message);
            }
        }
    }

    void OnDestroy()
    {
        // Отписываемся от события при уничтожении объекта
        Application.logMessageReceived -= HandleException;
    }*/
}