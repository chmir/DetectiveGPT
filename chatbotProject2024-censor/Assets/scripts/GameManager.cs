using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Main UI")]
    public GameObject mainUI; // 메인씬의 UI 패널을 할당할 필드

    public string SelectedBot { get; private set; }

    private void Awake()
    {
        // 싱글톤 패턴
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // 챗봇 선택 정보 설정 메서드
    public void SetSelectedBot(string botName)
    {
        SelectedBot = botName;
    }

    // "게임 시작" 버튼 누를 때 호출 (메인씬 UI 비활성화 및 스테이지씬 로드)
    public void OnStageSelect()
    {
        if (mainUI != null)
            mainUI.SetActive(false);

        SceneManager.LoadScene("StageScene", LoadSceneMode.Additive);
    }

    //인게임 혹은 스테이지씬에서 메인으로 돌아올 때 호출
    public void OnReturnToMain()
    {
        // 스테이지씬 언로드 (로드되어 있는 경우에만)
        if (IsSceneLoaded("StageScene"))
        {
            SceneManager.UnloadSceneAsync("StageScene");
        }

        // 인게임씬 언로드 (로드되어 있는 경우에만)
        if (IsSceneLoaded("IngameScene"))
        {
            SceneManager.UnloadSceneAsync("IngameScene");
        }

        // 메인 UI 재활성화
        if (mainUI != null)
            mainUI.SetActive(true);
    }

    // 씬 로드 여부 체크 메서드
    private bool IsSceneLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == sceneName && scene.isLoaded)
            {
                return true;
            }
        }
        return false;
    }

    // 스테이지씬에서 챗봇 선택 시 호출 (A,B,C 중 하나)
    public void SelectBotAndGoInGame(string botName)
    {
        SetSelectedBot(botName);
        // 인게임씬 로드를 Additive로 변경
        UnityEngine.SceneManagement.SceneManager.LoadScene("IngameScene", UnityEngine.SceneManagement.LoadSceneMode.Additive);

        // 필요하다면 여기서 StageScene을 언로드할 수도 있음
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("StageScene");
    }
}
