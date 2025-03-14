using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainPanelController : MonoBehaviour
{
    [SerializeField] private GameObject replayListPanel;
    public void OnClickSinglePlayButton()
    {
        GameManager.Instance.ChangeToGameScene(Constants.GameType.SinglePlayer);
    }
    
    public void OnClickDualPlayButton()
    {
        GameManager.Instance.ChangeToGameScene(Constants.GameType.DualPlayer);
    }

    public void OnClickMultiplayButton()
    {
        GameManager.Instance.ChangeToGameScene(Constants.GameType.MultiPlayer);
    }
    
    public void OnClickSettingsButton()
    {
        GameManager.Instance.OpenSettingsPanel();
    }

    public void OnClickReplayListButton()
    {
        // 메인 패널을 비활성화하거나 그대로 둘지 결정
        // 만약 메인 패널을 숨기고 싶다면:
        gameObject.SetActive(false);

        // ReplayListPanel 활성화
        replayListPanel.SetActive(true);
    }
    


}