using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// �ð��� ���� ���� ���� �ӵ��� ���������� �����ϴ� ������
/// </summary>
public class ZombieSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private Transform zombieSpawnPosition;
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private LayerMask zombieLayers;

    [Header("Difficulty Curve")]
    [SerializeField] private float initialSpawnCooldown = 5.0f; // ���� ���� ���� (��)
    [SerializeField] private float minimumSpawnCooldown = 1.0f; // �ּ� ���� ���� (��)
    [SerializeField] private float difficultyRampUpTime = 5.0f; // �ִ� ���̵� ���� �ð� (��)
    [SerializeField] private AnimationCurve difficultyCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // ���̵� ���� �

    [Header("Variation")]
    [SerializeField] private float randomVariation = 0.2f; // ���� �ð� ������ ���� ���� (0.2 = ��20%)

    // ���� ����
    private float gameStartTime;
    private bool isSpawning = false;
    private Coroutine spawningCoroutine;

    private void OnEnable()
    {
        StartSpawning();
    }

    private void OnDisable()
    {
        StopSpawning();
    }

    /// <summary>
    /// ���� ���� ����
    /// </summary>
    public void StartSpawning()
    {
        if (isSpawning) return;

        gameStartTime = Time.time;
        isSpawning = true;
        spawningCoroutine = StartCoroutine(SpawnRoutine());
    }

    /// <summary>
    /// ���� ���� �ߴ�
    /// </summary>
    public void StopSpawning()
    {
        if (!isSpawning) return;

        isSpawning = false;
        if (spawningCoroutine != null)
        {
            StopCoroutine(spawningCoroutine);
            spawningCoroutine = null;
        }
    }

    /// <summary>
    /// ���� ���̵� ���� ��� (0-1 ����)
    /// </summary>
    private float CalculateDifficultyLevel()
    {
        float timeSinceStart = Time.time - gameStartTime;
        float normalizedTime = Mathf.Clamp01(timeSinceStart / difficultyRampUpTime);
        return difficultyCurve.Evaluate(normalizedTime);
    }

    /// <summary>
    /// ���� ���� ���� ���
    /// </summary>
    private float CalculateSpawnInterval()
    {
        float difficultyLevel = CalculateDifficultyLevel();

        // ���̵��� ���� ���� ���� ���� ����
        float baseInterval = Mathf.Lerp(initialSpawnCooldown, minimumSpawnCooldown, difficultyLevel);

        // ������ ���� ����
        if (randomVariation > 0)
        {
            float randomFactor = UnityEngine.Random.Range(1 - randomVariation, 1 + randomVariation);
            baseInterval *= randomFactor;
        }

        return Mathf.Max(baseInterval, minimumSpawnCooldown);
    }

    /// <summary>
    /// ���� ���� �ڷ�ƾ
    /// </summary>
    private IEnumerator SpawnRoutine()
    {
        while (isSpawning)
        {
            float interval = CalculateSpawnInterval();
            yield return new WaitForSeconds(interval);

            if (isSpawning)
            {
                SpawnZombie();
            }
        }
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    private void SpawnZombie()
    {
        if (zombiePrefab == null || zombieSpawnPosition == null) return;

        GameObject zombie = ObjectPoolManager.SpawnObject(
            zombiePrefab,
            zombieSpawnPosition.position,
            Quaternion.identity
        );
    }
}