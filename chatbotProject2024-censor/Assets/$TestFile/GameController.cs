using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; } // �̱��� ���� ����

    public int SelectedStageId { get; private set; } // ���õ� �������� ID ����

    private void Awake()
    {
        // �̱��� ���� ����
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // GameController ����
    }

    // ���õ� �������� ID ����
    public void SetStageId(int stageId)
    {
        SelectedStageId = stageId;
    }

    // �� ��ȯ �޼���
    public void SwitchScene(string currentScene, string nextScene)
    {
        // ���� �� ��ε� (SceneA�� ����)
        if (!string.IsNullOrEmpty(currentScene) && SceneManager.GetSceneByName(currentScene).isLoaded)
        {
            SceneManager.UnloadSceneAsync(currentScene);
        }

        // ���� �� �ε�
        if (!SceneManager.GetSceneByName(nextScene).isLoaded)
        {
            SceneManager.LoadScene(nextScene, LoadSceneMode.Additive);
        }
    }

    // �� �ε� �޼��� (SceneB�� SceneC ��Ÿ�� ����)
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

        // �� �ε�
        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }
    }

    // �� ��ε� �޼���
    public void UnLoadScene(string sceneName)
    {
        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }
    }
}
