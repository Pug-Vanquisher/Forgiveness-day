using UnityEngine;
using TMPro;
using DG.Tweening;

public class ScoreUIManager : MonoBehaviour
{
    public TextMeshProUGUI currentScoreText; // Текущий счет
    public TextMeshProUGUI scorePopupText;   // Всплывающий текст для начисленных очков

    private int displayedScore = 0;
    private Vector3 initialPopupPosition; // Начальная позиция всплывающего текста
    private Sequence popupSequence;       // Последовательность анимации для прерывания

    private void Start()
    {
        UpdateScoreUI(0);
        initialPopupPosition = scorePopupText.transform.localPosition; // Запоминаем начальную позицию
        scorePopupText.alpha = 0;
    }

    public void UpdateScoreUI(int newScore)
    {
        displayedScore = newScore;
        currentScoreText.text = $"Score: {displayedScore}";
    }

    // Показ всплывающего текста с анимацией для серии убийств
    public void ShowScorePopup(int points, int killStreakCount)
    {
        // Прерываем текущую анимацию, если она еще активна
        popupSequence?.Kill();

        // Устанавливаем текст в зависимости от количества убийств подряд
        string killMessage = GetKillMessage(killStreakCount);
        scorePopupText.text = $"+{points} - {killMessage}";
        scorePopupText.alpha = 1;
        scorePopupText.transform.localPosition = initialPopupPosition; // Устанавливаем начальную позицию

        // Анимация появления и исчезновения текста
        popupSequence = DOTween.Sequence();
        popupSequence.Append(scorePopupText.DOFade(1, 0.2f)) // Плавное появление
                     .Append(scorePopupText.transform.DOLocalMoveY(30, 1f).SetRelative(true)) // Подъем вверх
                     .Append(scorePopupText.DOFade(0, 0.3f)) // Плавное исчезновение
                     .OnComplete(() => scorePopupText.transform.localPosition = initialPopupPosition);
    }

    // Показ всплывающего текста с анимацией для других действий
    public void ShowScorePopup(int points, string actionDescription)
    {
        // Прерываем текущую анимацию, если она еще активна
        popupSequence?.Kill();

        // Устанавливаем текст для других действий
        scorePopupText.text = $"+{points} - {actionDescription}";
        scorePopupText.alpha = 1;
        scorePopupText.transform.localPosition = initialPopupPosition; // Устанавливаем начальную позицию

        // Анимация появления и исчезновения текста
        popupSequence = DOTween.Sequence();
        popupSequence.Append(scorePopupText.DOFade(1, 0.2f)) // Плавное появление
                     .Append(scorePopupText.transform.DOLocalMoveY(30, 1f).SetRelative(true)) // Подъем вверх
                     .Append(scorePopupText.DOFade(0, 0.3f)) // Плавное исчезновение
                     .OnComplete(() => scorePopupText.transform.localPosition = initialPopupPosition);
    }

    // Определение сообщения в зависимости от серии убийств
    private string GetKillMessage(int killStreakCount)
    {
        return killStreakCount switch
        {
            0 => "Убийство",
            1 => "Двойное убийство",
            2 => "Тройное убийство",
            _ => "Буйство"
        };
    }
}
