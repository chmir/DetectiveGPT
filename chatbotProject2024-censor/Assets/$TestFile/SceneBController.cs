using TMPro;
using UnityEngine;

public class SceneBController : MonoBehaviour
{
    public TextMeshProUGUI stageIdText; // 선택된 스테이지 ID 표시 텍스트
    public GameObject sceneBUI; // SceneB UI 그룹

    private void Start()
    {
        if (sceneBUI != null)
        {
            sceneBUI.SetActive(true); // SceneB UI 활성화
        }

        UpdateStageIdDisplay(); // 선택된 스테이지 ID를 화면에 표시
    }

    // SceneC로 이동
    public void GoToSceneC()
    {
        if (sceneBUI != null)
        {
            sceneBUI.SetActive(false); // SceneB UI 숨김
        }

        if (GameController.Instance != null)
        {
            GameController.Instance.SwitchScene("SceneB", "SceneC"); // SceneB 언로드 후 SceneC 로드
        }
    }

    // SceneA로 돌아가는 메서드
    public void GoToSceneA()
    {
        if (sceneBUI != null)
        {
            sceneBUI.SetActive(false); // SceneB UI 숨김
        }

        if (GameController.Instance != null)
        {
            GameController.Instance.SwitchScene("SceneB", "SceneA"); // SceneB 언로드 후 SceneA 유지
        }
    }

    // 선택된 스테이지 ID를 화면에 표시하는 메서드
    private void UpdateStageIdDisplay()
    {
        if (GameController.Instance != null && stageIdText != null)
        {
            stageIdText.text = "Stage ID: " + GameController.Instance.SelectedStageId; // 선택된 스테이지 ID 표시
        }
    }
}
