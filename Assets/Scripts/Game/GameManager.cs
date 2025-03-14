using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// ------------------------------------------------------
// 기보 한 판의 기록
// ------------------------------------------------------
[Serializable]
public class GameRecord
{
    public string recordId;           // 예) "20230330_153045" 등
    public List<int> rows = new List<int>();
    public List<int> cols = new List<int>();
    public List<int> markers = new List<int>();  // Block.MarkerType을 int로 저장

    public GameRecord(string id)
    {
        recordId = id;
    }
    
    public GameRecord() { } // 디폴트 생성자
}

// ------------------------------------------------------
// 여러 판의 기보(게임 기록)를 담는 컨테이너
// ------------------------------------------------------
[Serializable]
public class ReplayData
{
    public List<GameRecord> records = new List<GameRecord>();
}

// ------------------------------------------------------
// GameManager 본체
// ------------------------------------------------------
public class GameManager : Singleton<GameManager>
{
    [Header("UI Prefabs / Panels")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private GameObject signinPanel;
    [SerializeField] private GameObject signupPanel;
    [Header("Replay Prefab")]
    [SerializeField] private GameObject replayPanelPrefab; 
    // ↑ 프리팹으로 만든 ReplayPanel (BlockController 포함)
    public GameObject mainPanel;

    private GameUIController _gameUIController;
    private Canvas _canvas;
    private Constants.GameType _gameType;
    private GameLogic _gameLogic;
    
    // ----------------------------------------
    // 기보(여러 게임 기록)를 저장하는 데이터
    // ----------------------------------------
    private ReplayData replayData = new ReplayData();
    private const string ReplayDataKey = "ReplayDataKey";

    private void Start()
    {
        // 로그인 패널 필요시 호출
        // OpenSigninPanel();

        // PlayerPrefs에서 기보 데이터 로드
        LoadReplayData();
    }

    public void ChangeToGameScene(Constants.GameType gameType)
    {
        _gameType = gameType;
        SceneManager.LoadScene("Game");
    }

    public void ChangeToMainScene()
    {
        _gameLogic?.Dispose();
        _gameLogic = null;
        SceneManager.LoadScene("Main");
    }

    // -------------------------------
    //  패널 열기 관련 (기존 코드 유지)
    // -------------------------------
    public void OpenSettingsPanel()
    {
        if (_canvas != null)
        {
            var settingsPanelObject = Instantiate(settingsPanel, _canvas.transform);
            settingsPanelObject.GetComponent<PanelController>().Show();
        }
    }

    public void OpenConfirmPanel(string message, ConfirmPanelController.OnConfirmButtonClick onConfirmButtonClick)
    {
        if (_canvas != null)
        {
            var confirmPanelObject = Instantiate(confirmPanel, _canvas.transform);
            confirmPanelObject.GetComponent<ConfirmPanelController>()
                .Show(message, onConfirmButtonClick);
        }
    }

    public void OpenSigninPanel()
    {
        if (_canvas != null)
        {
            var signinPanelObject = Instantiate(signinPanel, _canvas.transform);
        }
    }

    public void OpenSignupPanel()
    {
        if (_canvas != null)
        {
            var signupPanelObject = Instantiate(signupPanel, _canvas.transform);
        }
    }

    public void OpenGameOverPanel()
    {
        if (_gameUIController != null)
            _gameUIController.SetGameUIMode(GameUIController.GameUIMode.GameOver);
    }

    // ------------------------------------------------------------------
    //  게임 씬 로드 시 BlockController & GameUIController 설정
    // ------------------------------------------------------------------
    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            var blockController = GameObject.FindObjectOfType<BlockController>();
            _gameUIController = GameObject.FindObjectOfType<GameUIController>();

            blockController.InitBlocks();
            _gameUIController.SetGameUIMode(GameUIController.GameUIMode.Init);

            if (_gameLogic != null) _gameLogic.Dispose();
            _gameLogic = new GameLogic(blockController, _gameType);
        }
        
        _canvas = GameObject.FindObjectOfType<Canvas>();
    }

    private void OnApplicationQuit()
    {
        _gameLogic?.Dispose();
        _gameLogic = null;
    }

    // ==========================================
    //  아래부터 기보(ReplayData) 저장/로드 로직
    // ==========================================
    private void LoadReplayData()
    {
        string json = PlayerPrefs.GetString(ReplayDataKey, "");
        if (!string.IsNullOrEmpty(json))
        {
            replayData = JsonUtility.FromJson<ReplayData>(json);
            if (replayData == null)
                replayData = new ReplayData();
        }
        else
        {
            replayData = new ReplayData();
        }
    }

    private void SaveReplayData()
    {
        string json = JsonUtility.ToJson(replayData);
        PlayerPrefs.SetString(ReplayDataKey, json);
        PlayerPrefs.Save();
    }

    // 게임 종료 시, GameLogic에서 수순(기보) 넘겨줄 때 호출
    public void AddGameRecord(List<(int row, int col, Block.MarkerType)> moves)
    {
        // 새 기록 생성
        GameRecord record = new GameRecord(DateTime.Now.ToString("yyyyMMdd_HHmmss"));
        
        foreach (var move in moves)
        {
            record.rows.Add(move.row);
            record.cols.Add(move.col);
            record.markers.Add((int)move.Item3);
        }

        replayData.records.Add(record);
        SaveReplayData(); // PlayerPrefs에 반영
    }

    // 모든 기보 목록 반환 (ReplayListPanel에서 사용)
    public List<GameRecord> GetAllRecords()
    {
        return replayData.records;
    }

    // -----------------------------
    //  특정 기보 재생 패널 열기
    // -----------------------------
    
    public GameObject CreateReplayPanel()
    {
        return Instantiate(replayPanelPrefab, _canvas.transform);
    }
    public void OpenReplayPanel(GameRecord record)
    {
        if (record == null)
        {
            Debug.LogWarning("열 기보가 없습니다!");
            return;
        }

        // 1) ReplayPanel 프리팹 Instantiate
        var replayPanelObject = Instantiate(replayPanelPrefab, _canvas.transform);

        // 2) ReplayPanelController에 기보 전달
        var replayController = replayPanelObject.GetComponent<ReplayPanelController>();
        if (replayController == null)
        {
            Debug.LogError("ReplayPanelController가 없습니다!");
            return;
        }

        // record → (int row, int col, Block.MarkerType) 변환
        var moves = new List<(int, int, Block.MarkerType)>();
        for (int i = 0; i < record.rows.Count; i++)
        {
            moves.Add((record.rows[i], record.cols[i], (Block.MarkerType)record.markers[i]));
        }

        // 패널 열기
        replayController.OpenReplayPanel(moves);
    }
}
