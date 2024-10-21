using UnityEngine;

public class SpiderController : MonoBehaviour
{
    public float movementSpeed = 2.0f; // Швидкість переміщення павука
    public Transform cameraTransform; // Трансформ камери

    private Rigidbody rb; // Rigidbody 

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Отримуємо компонент Rigidbody
    }

    void FixedUpdate()
    {
        // Отримуємо вхідні дані для руху
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Обчислюємо напрямок руху від камери
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // Ігноруємо висоту
        forward.y = 0;
        right.y = 0;

        // Нормалізуємо вектори
        forward.Normalize();
        right.Normalize();

        // Створюємо вектор руху
        Vector3 movement = forward * moveVertical + right * moveHorizontal;

        // Рухаємо 
        rb.MovePosition(rb.position + movement * movementSpeed * Time.fixedDeltaTime);

        // Якщо є рух, повертайте в сторону руху
        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * 5f); // Швидкість обертання
        }
    }
}
