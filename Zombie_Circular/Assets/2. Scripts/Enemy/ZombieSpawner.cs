using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private float minimumSpawnCooldown = 1.0f; // 최소 스폰 간격
    [SerializeField] private float difficultyRampUpTime = 5.0f; // 최대 난이도 도달 시간
    [SerializeField] private AnimationCurve difficultyCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // 난이도 증가

    [Header("Variation")]
    [SerializeField] private float randomVariation = 0.2f; // 스폰 시간 랜덤 변동 비율

    private float m_gameStartTime;
    private bool m_isSpawning = false;
    private Coroutine m_spawningCoroutine;
    int[] m_availableLayers;
    private float m_curSpawnCoolTime;

    private void Awake()
    {
        m_availableLayers = GetSelectedLayers(zombieLayers);
    }

    private void OnEnable()
    {
        m_curSpawnCoolTime = minimumSpawnCooldown;
        StartSpawning();
    }

    private void OnDisable()
    {
        StopSpawning();
    }

    public void StartSpawning()
    {
        if (m_isSpawning) return;

        m_gameStartTime = Time.time;
        m_isSpawning = true;
        m_spawningCoroutine = StartCoroutine(SpawnRoutine());
    }

    public void StopSpawning()
    {
        if (!m_isSpawning) return;

        m_isSpawning = false;
        if (m_spawningCoroutine != null)
        {
            StopCoroutine(m_spawningCoroutine);
            m_spawningCoroutine = null;
        }
    }

    private float CalculateDifficultyLevel()
    {
        float timeSinceStart = Time.time - m_gameStartTime;
        float normalizedTime = Mathf.Clamp01(timeSinceStart / difficultyRampUpTime);
        return difficultyCurve.Evaluate(normalizedTime);
    }

    private float CalculateSpawnInterval()
    {
        float difficultyLevel = CalculateDifficultyLevel();
        float baseInterval = Mathf.Lerp(m_curSpawnCoolTime, minimumSpawnCooldown, difficultyLevel);

        if (randomVariation > 0)
        {
            float randomFactor = UnityEngine.Random.Range(1 - randomVariation, 1 + randomVariation);
            baseInterval *= randomFactor;
        }

        return Mathf.Max(baseInterval, minimumSpawnCooldown);
    }

    private IEnumerator SpawnRoutine()
    {
        while (m_isSpawning)
        {
            float interval = CalculateSpawnInterval();
            yield return new WaitForSeconds(interval);

            if (m_isSpawning)
            {
                SpawnZombie();
            }
        }
    }

    private void SpawnZombie()
    {
        if (zombiePrefab == null || zombieSpawnPosition == null) return;

        GameObject zombie = ObjectPoolManager.SpawnObject(
            zombiePrefab,
            zombieSpawnPosition.position,
            Quaternion.identity
        );
        AssignRandomLayer(zombie);
    }

    public void AssignRandomLayer(GameObject zombie)
    {
        if (m_availableLayers.Length == 0)
        {
            Debug.LogWarning("No Checked Layer");
            return;
        }

        zombie.layer = m_availableLayers[Random.Range(0, m_availableLayers.Length)];
    }

    private int[] GetSelectedLayers(LayerMask mask)
    {
        var selectedLayers = new List<int>();
        int layerMaskValue = mask.value;

        for (int i = 0; i < 32; i++)
        {
            if ((layerMaskValue & (1 << i)) != 0)
                selectedLayers.Add(i);
        }

        return selectedLayers.ToArray();
    }

}