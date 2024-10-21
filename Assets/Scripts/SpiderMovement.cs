using UnityEngine;

public class SpiderLegsController : MonoBehaviour
{
    public Transform[] legTargets; // Масив таргетів ніг
    public Transform spiderBody; // Тіло павука (має бути моделька тіла)
    public float[] stepRadius; // Масив радіусів для кожної ноги
    public float[] raycastDistance; // Масив відстаней raycast для кожної ноги
    public float stepSpeed = 5.0f; // Швидкість переміщення таргета назад до координати
    public float stepHeight = 0.3f; // Висота кроку
    public float movementSpeed = 2.0f; // Швидкість переміщення павука

    private Vector3[] initialLegPositions; // Початкові позиції таргетів відносно тіла
    private bool[] isStepping; // Чи робить нога крок
    private Rigidbody rb; // Rigidbody для павука

    void Start()
    {
        initialLegPositions = new Vector3[legTargets.Length];
        isStepping = new bool[legTargets.Length];
        rb = GetComponent<Rigidbody>(); // Отримуємо компонент Rigidbody

        // Задаємо початкові позиції для кожної ноги
        for (int i = 0; i < legTargets.Length; i++)
        {
            initialLegPositions[i] = legTargets[i].position - spiderBody.position;
        }

        // Ініціалізуємо масиви для stepRadius і raycastDistance, якщо вони не задані
        if (stepRadius.Length != legTargets.Length)
        {
            stepRadius = new float[legTargets.Length];
            for (int i = 0; i < legTargets.Length; i++)
            {
                stepRadius[i] = 0.5f; // Значення за замовчуванням
            }
        }

        if (raycastDistance.Length != legTargets.Length)
        {
            raycastDistance = new float[legTargets.Length];
            for (int i = 0; i < legTargets.Length; i++)
            {
                raycastDistance[i] = 1.0f; // Значення за замовчуванням
            }
        }
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        rb.MovePosition(rb.position + movement * movementSpeed * Time.fixedDeltaTime);

        for (int i = 0; i < legTargets.Length; i++)
        {
            // Перевіряємо, чи сусідні ноги не в процесі руху
            if (IsNeighborStepping(i)) continue;

            Vector3 targetPosition = spiderBody.position + spiderBody.TransformDirection(initialLegPositions[i]);
            targetPosition.y = GetGroundHeight(targetPosition, i); // Встановлюємо висоту на основі рельєфу

            float distance = Vector3.Distance(legTargets[i].position, targetPosition);

            // Якщо таргет виходить за межі радіуса і нога не робить крок
            if (distance > stepRadius[i] && !isStepping[i])
            {
                StartCoroutine(MoveLegToTarget(i, targetPosition));
            }
        }
    }

    // Функція для отримання висоти поверхні під ногою
    private float GetGroundHeight(Vector3 position, int legIndex)
    {
        RaycastHit hit;
        // Виконуємо raycast вниз, щоб отримати висоту поверхні
        if (Physics.Raycast(position + Vector3.up * raycastDistance[legIndex], Vector3.down, out hit, raycastDistance[legIndex] * 2))
        {
            return hit.point.y; // Повертаємо висоту з raycast
        }
        return position.y; // Якщо нічого не знайдено, повертаємо початкову висоту
    }

    // Функція перевірки сусідів для кожної ноги
    private bool IsNeighborStepping(int legIndex)
    {
        int previousLeg = (legIndex - 1 + legTargets.Length) % legTargets.Length; // Сусід ліворуч
        int nextLeg = (legIndex + 1) % legTargets.Length; // Сусід праворуч

        // Якщо будь-яка з сусідніх ніг в процесі руху, то повертаємо true
        return isStepping[previousLeg] || isStepping[nextLeg];
    }

    private System.Collections.IEnumerator MoveLegToTarget(int legIndex, Vector3 targetPosition)
    {
        isStepping[legIndex] = true;

        Vector3 startPosition = legTargets[legIndex].position;
        float stepProgress = 0.0f;

        while (stepProgress < 1.0f)
        {
            stepProgress += Time.deltaTime * stepSpeed;
            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, stepProgress);
            currentPosition.y += Mathf.Sin(stepProgress * Mathf.PI) * stepHeight;

            legTargets[legIndex].position = currentPosition;

            yield return null;
        }

        legTargets[legIndex].position = targetPosition;
        isStepping[legIndex] = false;
    }
}
