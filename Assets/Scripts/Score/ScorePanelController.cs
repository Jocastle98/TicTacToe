using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScorePanelController : PanelController
{
    [Header("Score Containers")]
    [SerializeField] private Transform addScoreObjects;    
    [SerializeField] private Transform deleteScoreObjects; 

    [Header("Score Image Prefab")]
    [SerializeField] private GameObject scoreImagePrefab;

    [Header("UI Text")]
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TextMeshProUGUI upgradeText;

    public void InitializePanel(int currentScore, bool isWin, int gainedOrLost)
    {
        Show();

        // 1) 메시지: 승리/패배, ±1점
        if (isWin)
        {
            messageText.text = $"게임에서 승리했습니다.\n{gainedOrLost}점을 획득했습니다.";
        }
        else
        {
            messageText.text = $"게임에서 패배했습니다.\n{Mathf.Abs(gainedOrLost)}점이 깎였습니다.";
        }

        // 2) 아이콘 표시
        RefreshIcons(currentScore);

        // 3) upgradeText 등
        if (currentScore >= 30)
        {
            upgradeText.text = $"현재 점수: {currentScore}\n최대치에 도달!";
        }
        else if (currentScore <= -30)
        {
            upgradeText.text = $"현재 점수: {currentScore}\n최소치에 도달!";
        }
        else
        {
            int remainToMax = 30 - currentScore;
            int remainToMin = 30 + currentScore;
            upgradeText.text = $"현재 점수: {currentScore}\n"
                               + $"{remainToMax}점 더 얻으면 최대치,\n"
                               + $"{remainToMin}점 더 잃으면 최소치.";
        }
    }

    private void RefreshIcons(int currentScore)
    {
        // 기존 아이콘 제거
        foreach (Transform child in addScoreObjects) Destroy(child.gameObject);
        foreach (Transform child in deleteScoreObjects) Destroy(child.gameObject);

        // 양수 => 왼쪽 아이콘
        if (currentScore > 0)
        {
            for (int i = 0; i < currentScore; i++)
            {
                Instantiate(scoreImagePrefab, addScoreObjects);
            }
        }
        // 음수 => 오른쪽 아이콘
        else if (currentScore < 0)
        {
            int absVal = Mathf.Abs(currentScore);
            for (int i = 0; i < absVal; i++)
            {
                Instantiate(scoreImagePrefab, deleteScoreObjects);
            }
        }
        // 0 => 아무것도 표시 안 함
    }

    public void OnCloseButtonClick()
    {
        Hide(() =>
        {
            Destroy(gameObject);
        });
    }
}
