using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneCController : MonoBehaviour
{
    public GameObject sceneCUI; // SceneC UI 그룹
    public Button[] stageButtons; // 스테이지 버튼 배열

    private void Start()
    {
        if (sceneCUI != null)
        {
            sceneCUI.SetActive(true); // SceneC UI 활성화
        }

        AssignStageButtonListeners(); // 스테이지 버튼 리스너 추가
    }

    // 각 스테이지 버튼에 클릭 리스너를 추가하는 메서드
    private void AssignStageButtonListeners()
    {
        for (int i = 0; i < stageButtons.Length; i++)
        {
            int stageId = i + 1; // 스테이지 ID는 1부터 시작
            stageButtons[i].onClick.AddListener(() => SelectStage(stageId));
        }
    }

    // 스테이지 선택 시 호출되는 메서드
    private void SelectStage(int stageId)
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.SetStageId(stageId); // 선택된 스테이지 ID 저장

            if (sceneCUI != null)
            {
                sceneCUI.SetActive(false); // SceneC UI 숨김
            }

            GameController.Instance.SwitchScene("SceneC", "SceneB"); // SceneC 언로드 후 SceneB 로드
        }
    }

    // SceneA로 돌아가는 메서드
    public void GoToSceneA()
    {
        if (GameController.Instance != null)
        {
            Debug.Log("Switching from SceneC to SceneA");
            GameController.Instance.SwitchScene("SceneC", "SceneA");
        }

        if (SceneAController.Instance != null)
        {
            Debug.Log("Calling RestoreUI on SceneAController");
            SceneAController.Instance.OnReturnToSceneA();
        }
        else
        {
            Debug.LogError("SceneAController.Instance is null! SceneA may not be loaded yet.");
        }
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "SceneA" && SceneAController.Instance != null)
        {
            SceneAController.Instance.OnReturnToSceneA(); // SceneA UI 활성화
            SceneManager.sceneLoaded -= OnSceneLoaded; // 이벤트 구독 해제
        }
    }

}
