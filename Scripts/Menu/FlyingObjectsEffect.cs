using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingObjectsEffect : MonoBehaviour
{
    [Header("Shockwave Settings")]
    public float waveForce = 50f; // Сила ударной волны
    public float minForce = 5f; // Минимальная сила для дальних объектов
    public float waveRadius = 10f; // Радиус воздействия
    //public AnimationCurve forceFalloff; // Кривая спада силы в зависимости от расстояния

    private Vector3 velocity;
    [SerializeField] private Transform waveOrigin; // Точка центра ударной волны

    void Start()
    {
        // Рассчитываем направление от центра волны к объекту
        Vector3 direction = (transform.position - waveOrigin.position).normalized;
        velocity = direction * waveForce;
    }

    void Update()
    {
        velocity = Vector3.Lerp(velocity, Vector3.zero, (velocity.magnitude / 2) * Time.deltaTime);

        // Перемещаем объект
        transform.position += velocity * Time.deltaTime;

        // Если скорость стала очень маленькой, останавливаем полностью
        if (velocity.magnitude < 0.1f)
        {
            velocity = Vector3.zero;
            enabled = false; // Отключаем скрипт, когда объект остановился
        }
    }
}
