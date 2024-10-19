using UnityEngine;
using TMPro; 
using DG.Tweening;

public class NotificationManager : MonoBehaviour
{
    public TextMeshProUGUI enemyNotification;  
    public TextMeshProUGUI healthNotification;

    private void Start()
    {
        // Подписка
        EventManager.Instance.Subscribe("NoEnemies", ShowEnemyNotification);
        EventManager.Instance.Subscribe("Heal", ShowHealthNotification);
    }

    private void OnDestroy()
    {
        // Отписка
        EventManager.Instance.Unsubscribe("NoEnemies", ShowEnemyNotification);
        EventManager.Instance.Unsubscribe("Heal", ShowHealthNotification);
    }

    private void ShowEnemyNotification()
    {
        enemyNotification.gameObject.SetActive(true);
        enemyNotification.transform.localPosition = new Vector3(0, Screen.height, 0);
        enemyNotification.text = "Новый дрон прибыл";

        // Анимация
        enemyNotification.transform.DOLocalMoveY(Screen.height / 4, 1.5f).OnComplete(() =>
            enemyNotification.transform.DOLocalMoveY(Screen.height, 0.3f).OnComplete(() =>
                enemyNotification.gameObject.SetActive(false)));
    }

    private void ShowHealthNotification()
    {
        healthNotification.gameObject.SetActive(true);
        healthNotification.transform.localPosition = new Vector3(0, -Screen.height, 0); 
        healthNotification.text = "Здоровье восстановлено";

        // Анимация
        healthNotification.transform.DOLocalMoveY(-Screen.height / 4, 1.5f).OnComplete(() =>
            healthNotification.transform.DOLocalMoveY(-Screen.height, 0.3f).OnComplete(() =>
                healthNotification.gameObject.SetActive(false)));
    }
}
