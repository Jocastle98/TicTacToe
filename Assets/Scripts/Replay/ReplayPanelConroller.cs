using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReplayPanelController : PanelController
{
    [SerializeField] private GameObject mainPanel; // 메인 패널
    [SerializeField] private GameObject replayPanel; // 기보 패널
    [SerializeField] private BlockController blockController;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;

    private List<(int row, int col, Block.MarkerType)> gameHistory;
    private int currentIndex = -1;

    public void OpenReplayPanel(List<(int, int, Block.MarkerType)> history)
    {
        if (history == null || history.Count == 0) return; // 기보 데이터 없으면 리턴

        gameHistory = history;
        currentIndex = -1;

        replayPanel.SetActive(true);
        mainPanel.SetActive(false); 

        blockController.InitBlocks();
        OnNextButtonClick();
    }

    public void OnCloseButtonClick()
    {
        Hide(() =>
        {
            mainPanel.SetActive(true); 
        });
    }

    public void OnNextButtonClick()
    {
        if (currentIndex + 1 < gameHistory.Count)
        {
            currentIndex++;
            var move = gameHistory[currentIndex];
            blockController.PlaceMarker(move.Item3, move.row, move.col);
        }
    }

    public void OnPrevButtonClick()
    {
        if (currentIndex >= 0)
        {
            var move = gameHistory[currentIndex];
            blockController.PlaceMarker(Block.MarkerType.None, move.row, move.col);
            currentIndex--;
        }
    }

   

    
}
