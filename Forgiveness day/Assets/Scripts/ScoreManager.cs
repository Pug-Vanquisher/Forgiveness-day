using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public ScoreUIManager scoreUIManager;
    public int currentScore = 0;
    private int killStreakCount = 0;
    private float timeSinceLastKill = 0f;
    private const float killStreakTimeout = 5f;

    public float actionInterval = 10f;
    private float timeSinceLastDash = Mathf.NegativeInfinity;
    private float timeSinceLastWeaponSwitch = Mathf.NegativeInfinity;

    private float superAbilityCooldown = 10f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        EventManager.Instance.Subscribe("EnemyKilled", OnEnemyKilled);
        EventManager.Instance.Subscribe("DashPerformed", OnDashPerformed);
        EventManager.Instance.Subscribe("WeaponSwitched", OnWeaponSwitched);
    }

    private void Update()
    {
        if (killStreakCount > 0)
        {
            timeSinceLastKill += Time.deltaTime;
            if (timeSinceLastKill > killStreakTimeout)
            {
                ResetKillStreak();
            }
        }
    }

    public void StartSuperAbilityCooldown(System.Action onCooldownComplete)
    {
        StartCoroutine(SuperAbilityCooldownRoutine(onCooldownComplete));
    }

    private IEnumerator SuperAbilityCooldownRoutine(System.Action onCooldownComplete)
    {
        yield return new WaitForSeconds(superAbilityCooldown);
        onCooldownComplete?.Invoke();
        TriggerSuperAbilityReadyEvent(); // �������� ������� ��������� ��������
    }

    private void TriggerSuperAbilityReadyEvent()
    {
        EventManager.Instance.TriggerEvent("SuperAbilityReady");
        Debug.Log("����� ������ �����");
    }

    private void OnEnemyKilled()
    {
        int scoreToAdd = CalculateScore();
        currentScore += scoreToAdd;
        scoreUIManager.UpdateScoreUI(currentScore);
        scoreUIManager.ShowScorePopup(scoreToAdd, killStreakCount);

        killStreakCount++;
        timeSinceLastKill = 0f;
    }

    private void OnDashPerformed()
    {
        if (Time.time - timeSinceLastDash >= actionInterval)
        {
            currentScore += 10;
            timeSinceLastDash = Time.time;
            scoreUIManager.UpdateScoreUI(currentScore);
            scoreUIManager.ShowScorePopup(10, "���");
            Debug.Log($"Dash Score: +10 | Total Score: {currentScore}");
        }
    }

    private void OnWeaponSwitched()
    {
        if (Time.time - timeSinceLastWeaponSwitch >= actionInterval)
        {
            currentScore += 10;
            timeSinceLastWeaponSwitch = Time.time;
            scoreUIManager.UpdateScoreUI(currentScore);
            scoreUIManager.ShowScorePopup(10, "����� ������");
            Debug.Log($"Weapon Switch Score: +10 | Total Score: {currentScore}");
        }
    }

    private int CalculateScore()
    {
        switch (killStreakCount)
        {
            case 0: return 5;
            case 1: return 15;
            case 2: return 30;
            default: return 50;
        }
    }

    private void ResetKillStreak()
    {
        killStreakCount = 0;
        timeSinceLastKill = 0f;
        Debug.Log("Kill streak ended.");
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        EventManager.Instance.Unsubscribe("EnemyKilled", OnEnemyKilled);
        EventManager.Instance.Unsubscribe("DashPerformed", OnDashPerformed);
        EventManager.Instance.Unsubscribe("WeaponSwitched", OnWeaponSwitched);
    }
}
