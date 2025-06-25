using UnityEngine;
using System.Collections.Generic;

public class OptimizedWallHider : MonoBehaviour
{
    [Header("Основные настройки")]
    [SerializeField] private Transform player; // Игрок
    [SerializeField] private Camera mainCamera; // Камера
    [SerializeField] private LayerMask wallLayer; // Слой стен
    [SerializeField] private float maxCheckDistance = 15f; // Макс. расстояние проверки (можно увеличить)
    [SerializeField] private float wallWidthMargin = 2f; // Учёт ширины стен (2 метра)

    [Header("Дополнительные лучи")]
    [SerializeField] private int extraRays = 2; // Кол-во дополнительных лучей (по бокам)
    [SerializeField] private float raySpread = 0.5f; // Разброс лучей (чтобы охватить края стены)

    private List<Renderer> hiddenWalls = new List<Renderer>(); // Список скрытых стен

    private void Update()
    {
        // Восстанавливаем видимость всех ранее скрытых стен
        foreach (var wall in hiddenWalls)
        {
            if (wall != null) 
                wall.enabled = true;
        }
        hiddenWalls.Clear();

        // Основной луч от камеры к игроку
        Vector3 camToPlayer = player.position - mainCamera.transform.position;
        float distance = Mathf.Min(camToPlayer.magnitude, maxCheckDistance);
        Vector3 direction = camToPlayer.normalized;

        // Пускаем основной луч + дополнительные по бокам
        for (int i = 0; i <= extraRays; i++)
        {
            Vector3 rayOrigin = mainCamera.transform.position;
            Vector3 rayDirection = direction;

            // Если это боковой луч – смещаем направление
            if (i > 0)
            {
                float spread = (i % 2 == 1) ? raySpread : -raySpread; // Чередуем влево/вправо
                rayDirection = Quaternion.Euler(0, spread * 10f, 0) * direction; // Немного разводим лучи
            }

            RaycastHit[] hits = Physics.RaycastAll(rayOrigin, rayDirection, distance, wallLayer);

            // Скрываем все стены на пути лучей
            foreach (var hit in hits)
            {
                Renderer wallRenderer = hit.collider.GetComponent<Renderer>();
                if (wallRenderer != null && !hiddenWalls.Contains(wallRenderer)) // Чтобы не дублировать
                {
                    wallRenderer.enabled = false;
                    hiddenWalls.Add(wallRenderer);
                }
            }
        }
    }

    // Визуализация лучей в редакторе
    private void OnDrawGizmosSelected()
    {
        if (player == null || mainCamera == null) return;

        Gizmos.color = Color.cyan;
        Vector3 direction = (player.position - mainCamera.transform.position).normalized;
        float distance = Vector3.Distance(mainCamera.transform.position, player.position);

        // Основной луч
        Gizmos.DrawLine(mainCamera.transform.position, mainCamera.transform.position + direction * distance);

        // Дополнительные лучи (если включены)
        if (extraRays > 0)
        {
            Gizmos.color = Color.yellow;
            for (int i = 1; i <= extraRays; i++)
            {
                float spread = (i % 2 == 1) ? raySpread : -raySpread;
                Vector3 spreadDirection = Quaternion.Euler(0, spread * 10f, 0) * direction;
                Gizmos.DrawLine(mainCamera.transform.position, mainCamera.transform.position + spreadDirection * distance);
            }
        }
    }
}