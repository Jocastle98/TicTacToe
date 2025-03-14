using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReplayPanelController : PanelController
{
    [SerializeField] private BlockController blockController;

    private List<(int row, int col, Block.MarkerType)> gameHistory;
    private int currentIndex = -1;

    public void OpenReplayPanel(List<(int, int, Block.MarkerType)> moves)
    {
        if (moves == null || moves.Count == 0) return;

        gameHistory = moves;
        currentIndex = -1;

        Show(); 
        blockController.InitBlocks();
        OnNextButtonClick();
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


    public void OnCloseButtonClick()
    {
        Hide(() =>
        {
            gameObject.SetActive(false);
        });
    }
}