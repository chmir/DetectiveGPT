using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Main UI")]
    public GameObject mainUI; // ���ξ��� UI �г��� �Ҵ��� �ʵ�

    public string SelectedBot { get; private set; }

    private void Awake()
    {
        // �̱��� ����
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

    // ê�� ���� ���� ���� �޼���
    public void SetSelectedBot(string botName)
    {
        SelectedBot = botName;
    }

    // "���� ����" ��ư ���� �� ȣ�� (���ξ� UI ��Ȱ��ȭ �� ���������� �ε�)
    public void OnStageSelect()
    {
        if (mainUI != null)
            mainUI.SetActive(false);

        SceneManager.LoadScene("StageScene", LoadSceneMode.Additive);
    }

    //�ΰ��� Ȥ�� �������������� �������� ���ƿ� �� ȣ��
    public void OnReturnToMain()
    {
        // ���������� ��ε� (�ε�Ǿ� �ִ� ��쿡��)
        if (IsSceneLoaded("StageScene"))
        {
            SceneManager.UnloadSceneAsync("StageScene");
        }

        // �ΰ��Ӿ� ��ε� (�ε�Ǿ� �ִ� ��쿡��)
        if (IsSceneLoaded("IngameScene"))
        {
            SceneManager.UnloadSceneAsync("IngameScene");
        }

        // ���� UI ��Ȱ��ȭ
        if (mainUI != null)
            mainUI.SetActive(true);
    }

    // �� �ε� ���� üũ �޼���
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

    // �������������� ê�� ���� �� ȣ�� (A,B,C �� �ϳ�)
    public void SelectBotAndGoInGame(string botName)
    {
        SetSelectedBot(botName);
        // �ΰ��Ӿ� �ε带 Additive�� ����
        UnityEngine.SceneManagement.SceneManager.LoadScene("IngameScene", UnityEngine.SceneManagement.LoadSceneMode.Additive);

        // �ʿ��ϴٸ� ���⼭ StageScene�� ��ε��� ���� ����
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("StageScene");
    }
}
