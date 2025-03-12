using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; } // 싱글톤 패턴 구현

    public int SelectedStageId { get; private set; } // 선택된 스테이지 ID 저장

    private void Awake()
    {
        // 싱글톤 패턴 설정
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // GameController 유지
    }

    // 선택된 스테이지 ID 설정
    public void SetStageId(int stageId)
    {
        SelectedStageId = stageId;
    }

    // 씬 전환 메서드
    public void SwitchScene(string currentScene, string nextScene)
    {
        // 현재 씬 언로드 (SceneA는 유지)
        if (!string.IsNullOrEmpty(currentScene) && SceneManager.GetSceneByName(currentScene).isLoaded)
        {
            SceneManager.UnloadSceneAsync(currentScene);
        }

        // 다음 씬 로드
        if (!SceneManager.GetSceneByName(nextScene).isLoaded)
        {
            SceneManager.LoadScene(nextScene, LoadSceneMode.Additive);
        }
    }

    // 씬 로드 메서드 (SceneB와 SceneC 배타적 관리)
    public void LoadScene(string sceneName)
    {
        if (sceneName == "SceneB")
        {
            if (SceneManager.GetSceneByName("SceneC").isLoaded)
            {
                SceneManager.UnloadSceneAsync("SceneC");
            }
        }
        else if (sceneName == "SceneC")
        {
            if (SceneManager.GetSceneByName("SceneB").isLoaded)
            {
                SceneManager.UnloadSceneAsync("SceneB");
            }
        }

        // 씬 로드
        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }
    }

    // 씬 언로드 메서드
    public void UnLoadScene(string sceneName)
    {
        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }
    }
}
