using UnityEngine;

public class ScorePanelController : PanelController
{
    [Header("Score Containers")]
    [SerializeField] private Transform addScoreObjects;    // 왼쪽
    [SerializeField] private Transform deleteScoreObjects; // 오른쪽
    [Header("Score Image Prefab")]
    [SerializeField] private GameObject scoreImagePrefab;

    /// <summary>
    /// ScorePanel 열릴 때, GameManager가 아이콘 개수를 전달
    /// </summary>
    public void InitializeIcons(int leftCount, int rightCount)
    {
        // 패널에 들어있는 기존 아이콘 제거 (중복 생성 방지)
        foreach (Transform child in addScoreObjects) Destroy(child.gameObject);
        foreach (Transform child in deleteScoreObjects) Destroy(child.gameObject);

        // 왼쪽 아이콘 개수만큼 생성
        for (int i = 0; i < leftCount; i++)
        {
            Instantiate(scoreImagePrefab, addScoreObjects);
        }
        // 오른쪽 아이콘 개수만큼 생성
        for (int i = 0; i < rightCount; i++)
        {
            Instantiate(scoreImagePrefab, deleteScoreObjects);
        }
    }

    public void OnCloseButtonClick()
    {
        Hide(() =>
        {
            // 패널 파괴
            Destroy(gameObject);
        });
    }
}