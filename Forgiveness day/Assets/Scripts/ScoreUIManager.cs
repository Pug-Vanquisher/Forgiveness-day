using UnityEngine;
using TMPro;
using DG.Tweening;

public class ScoreUIManager : MonoBehaviour
{
    public TextMeshProUGUI currentScoreText; 
    public TextMeshProUGUI scorePopupText;   

    private int displayedScore = 0;
    private Vector3 initialPopupPosition;
    private Sequence popupSequence;       

    private void Start()
    {
        UpdateScoreUI(0);
        initialPopupPosition = scorePopupText.transform.localPosition; 
        scorePopupText.alpha = 0;
    }

    public void UpdateScoreUI(int newScore)
    {
        displayedScore = newScore;
        currentScoreText.text = $"Score: {displayedScore}";
    }

    public void ShowScorePopup(int points, int killStreakCount)
    {
        popupSequence?.Kill();

        string killMessage = GetKillMessage(killStreakCount);
        scorePopupText.text = $"+{points} - {killMessage}";
        scorePopupText.alpha = 1;
        scorePopupText.transform.localPosition = initialPopupPosition;

        // Анимация появления и исчезновения текста
        popupSequence = DOTween.Sequence();
        popupSequence.Append(scorePopupText.DOFade(1, 0.2f)) 
                     .Append(scorePopupText.transform.DOLocalMoveY(30, 1f).SetRelative(true)) 
                     .Append(scorePopupText.DOFade(0, 0.3f)) 
                     .OnComplete(() => scorePopupText.transform.localPosition = initialPopupPosition);
    }


    public void ShowScorePopup(int points, string actionDescription)
    {

        popupSequence?.Kill();

        scorePopupText.text = $"+{points} - {actionDescription}";
        scorePopupText.alpha = 1;
        scorePopupText.transform.localPosition = initialPopupPosition; 

        popupSequence = DOTween.Sequence();
        popupSequence.Append(scorePopupText.DOFade(1, 0.2f)) 
                     .Append(scorePopupText.transform.DOLocalMoveY(30, 1f).SetRelative(true)) 
                     .Append(scorePopupText.DOFade(0, 0.3f)) 
                     .OnComplete(() => scorePopupText.transform.localPosition = initialPopupPosition);
    }

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
