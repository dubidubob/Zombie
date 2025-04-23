using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 시간에 따라 좀비 스폰 속도가 점진적으로 증가하는 스포너
/// </summary>
public class ZombieSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private Transform zombieSpawnPosition;
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private LayerMask zombieLayers;

    [Header("Difficulty Curve")]
    [SerializeField] private float initialSpawnCooldown = 5.0f; // 시작 스폰 간격 (초)
    [SerializeField] private float minimumSpawnCooldown = 1.0f; // 최소 스폰 간격 (초)
    [SerializeField] private float difficultyRampUpTime = 5.0f; // 최대 난이도 도달 시간 (초)
    [SerializeField] private AnimationCurve difficultyCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // 난이도 증가 곡선

    [Header("Variation")]
    [SerializeField] private float randomVariation = 0.2f; // 스폰 시간 무작위 변동 비율 (0.2 = ±20%)

    // 내부 변수
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
    /// 좀비 스폰 시작
    /// </summary>
    public void StartSpawning()
    {
        if (isSpawning) return;

        gameStartTime = Time.time;
        isSpawning = true;
        spawningCoroutine = StartCoroutine(SpawnRoutine());
    }

    /// <summary>
    /// 좀비 스폰 중단
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
    /// 현재 난이도 레벨 계산 (0-1 범위)
    /// </summary>
    private float CalculateDifficultyLevel()
    {
        float timeSinceStart = Time.time - gameStartTime;
        float normalizedTime = Mathf.Clamp01(timeSinceStart / difficultyRampUpTime);
        return difficultyCurve.Evaluate(normalizedTime);
    }

    /// <summary>
    /// 현재 스폰 간격 계산
    /// </summary>
    private float CalculateSpawnInterval()
    {
        float difficultyLevel = CalculateDifficultyLevel();

        // 난이도에 따라 스폰 간격 선형 보간
        float baseInterval = Mathf.Lerp(initialSpawnCooldown, minimumSpawnCooldown, difficultyLevel);

        // 무작위 변동 적용
        if (randomVariation > 0)
        {
            float randomFactor = UnityEngine.Random.Range(1 - randomVariation, 1 + randomVariation);
            baseInterval *= randomFactor;
        }

        return Mathf.Max(baseInterval, minimumSpawnCooldown);
    }

    /// <summary>
    /// 좀비 스폰 코루틴
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
    /// 좀비 생성
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