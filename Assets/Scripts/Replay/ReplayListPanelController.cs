using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReplayListPanelController : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private GameObject replayButtonPrefab; // 프리팹(각 기보 버튼)

    void OnEnable()
    {
        RefreshList();
    }

    private void RefreshList()
    {
        // // 기존 버튼 제거
        // foreach (Transform child in content)
        // {
        //     Destroy(child.gameObject);
        // }

        // GameManager에서 기보 목록 받아오기
        var records = GameManager.Instance.GetAllRecords();
        foreach (var record in records)
        {
            var buttonObj = Instantiate(replayButtonPrefab, content);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = $"기록: {record.recordId}";

            // 클릭 시 ReplayPanel 열기
            buttonObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                OpenReplay(record);
            });
        }
    }

    private void OpenReplay(GameRecord record)
    {
        // 1) ReplayListPanel 닫기
        gameObject.SetActive(false);

        // 2) ReplayPanel 프리팹 Instantiate
        var replayPanelObj = GameManager.Instance.CreateReplayPanel();
        // ↑ transform.parent: 같은 Canvas 아래에 두기

        // 3) ReplayPanelController 찾기
        var replayPanel = replayPanelObj.GetComponent<ReplayPanelController>();

        // 4) record → (row, col, marker) 형태로 변환
        var moves = new List<(int, int, Block.MarkerType)>();
        for (int i = 0; i < record.rows.Count; i++)
        {
            moves.Add((record.rows[i], record.cols[i], (Block.MarkerType)record.markers[i]));
        }

        // 5) ReplayPanel 열기
        replayPanel.OpenReplayPanel(moves);
    }



    public void OnClickCloseButton()
    {
        // 리스트 패널 닫기
        gameObject.SetActive(false);
    }
}