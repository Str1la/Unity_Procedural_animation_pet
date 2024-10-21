using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Ціль (ваш павук)
    public Vector3 offset; // Відстань між камерою та павуком
    public float smoothSpeed = 0.125f; // Швидкість плавності слідування
    public float mouseSensitivity = 2.0f; // Чутливість миші

    private float rotationX = 0f; // Верт. обертання
    private float rotationY = 0f; // Гор. обертання

    void Start()
    {
        // Блокуємо курсор, щоб він не виходив за межі екрана
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        // Обробка вводу миші
        rotationX += Input.GetAxis("Mouse X") * mouseSensitivity;
        rotationY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        rotationY = Mathf.Clamp(rotationY, -30f, 30f); // Обмеження вертикального обертання

        // Обчислюємо нову позицію камери
        Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0);
        Vector3 desiredPosition = target.position + rotation * offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Слідкуємо за павуком
        transform.LookAt(target);
    }
}
