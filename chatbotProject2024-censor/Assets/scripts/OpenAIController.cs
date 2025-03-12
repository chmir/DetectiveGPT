using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using OpenAI_API;
using OpenAI_API.Chat;
using System;
using OpenAI_API.Models;
using UnityEngine.Networking;

/// <summary>
/// OpenAIController
/// - Unity에서 OpenAI API와 음성 인식/합성을 연동하여 챗봇 기능을 제공하는 스크립트
/// - 사용자의 텍스트 또는 음성 입력을 받아 챗봇 답변을 화면에 출력하고, 음성으로 재생
/// - 특정 '봇' 시나리오에 따른 심문 게임 흐름을 제어
/// </summary>
public class OpenAIController : MonoBehaviour
{
    // UI 요소
    public TMP_Text textField;
    public TMP_InputField inputField;
    public Button okBtn;
    public ScrollRect scrollRect;
    public Button recordButton;
    public Button stopButton;
    public AudioSource audioSource;
    public TMP_Text statusText;

    // API 및 상태 변수
    private OpenAIAPI api;
    private List<ChatMessage> messages;
    private AudioClip recordedClip;
    private bool isRecording = false;
    private string googleApiKey = "*검열*";
    private string openApiKey = "*검열*";

    // 챗봇 답변에 관한 변수
    string mood = "";             // "기분"
    string confessionRate = "";   // "자백율"
    string response = "";         // "답변"

    // AnimationController 연동
    public AnimationController animationController;

    /// <summary>
    /// Start()
    /// - OpenAI API 초기화 및 초기 대화 시작 설정
    /// - 버튼 리스너 등록 및 녹음 버튼 상태 초기화
    /// </summary>
    void Start()
    {
        api = new OpenAIAPI(new APIAuthentication(openApiKey));
        StartConversation();

        // 버튼 리스너 설정
        okBtn.onClick.AddListener(() => GetResponse());
        recordButton.onClick.AddListener(StartRecording);
        stopButton.onClick.AddListener(StopRecording);

        // 초기 버튼 상태 설정
        stopButton.interactable = false;
    }

    /// <summary>
    /// StartConversation()
    /// - 선택된 봇 정보에 따라 시스템 메시지(시나리오) 구성
    /// - 메시지 리스트 초기화 후 초기 시스템 메시지 설정
    /// </summary>
    private void StartConversation()
    {
        // GameManager에서 현재 선택된 봇 정보 가져오기
        string selectedBot = GameManager.Instance.SelectedBot;

        // 봇 성격에 따른 시나리오 텍스트 결정
        string scenarioText = "";
        switch (selectedBot)
        {
            case "A":
                scenarioText = "너가 저지른 사건은 다음과 같아: \r\n너는 일서대학교의 지피티 학생이야. "
                + "대학교에서 성적 정정 문제로 8시에 나사망 교수님을 찾아갔지만 교수님은 너의 성적 정정 요구를 들어주지 않았고, "
                + "너는 홧김에 교수님을 보낸거야. "
                + "살해 방법: 카라멜 마끼아또를 좋아하시는 교수님께 평소보다 설탕 시럽을 많이 넣은 후 농약을 타서 눈치채지 못하게 마시게 하여 범행을 저질렀다. "
                + "너의 약점: 심문관은 너가 홀로 8시 10분에 교수님이 근무하시던 A동에서 나왔다는 것을 알고 있다. "
                + "그리고 교수님의 방에서 농약이 사용된 흔적을 발견했다.";
                break;
            case "B":
                scenarioText = "사건: 너는 블루카페의 단골손님이야. 항상 카페에서 일하는 점원과 친하게 지내려 했지만, 연애까지는 할 수 없었어. "
                + "너는 그 점에 불만이 생겨서 12월 겨울의 추운 날씨에 카페가 문을 닫는 10시 이후에 범행을 저질렀다. "
                + "살해 방법: 카페가 문을 닫는 시간까지 홀로 앉아 있다가, 카페 점원이 퇴근하는 시각인 10시 30분에 "
                + "카페 후문에서 나오는 점원을 커다란 얼음 고드름으로 뒷목에 가격했다. "
                + "약점: 경찰 측은 정확히 어떤 도구로 살해했는지 알 수 없었지만, 너와 점원이 가까웠다는 점, "
                + "너가 카페가 문을 닫는 시간까지 홀로 있었다는 점, 후문에서 얼음이 깨지는 소리가 들렸다는 목격담이 접수되었다.";
                break;
            case "C":
                scenarioText = "사건: 어느 A도시에서 몇 개월간 불법적인 그래피티 아티스트의 기물 훼손 문제가 있었다. "
                + "사람이 많이 오고 가는 곳, 기념비적인 건물에 유성 락카로 자신의 마크를 남기고 도망간 것이다. "
                + "락카로 칠해진 그림 자체는 예술적이었지만, 허락받지 않은 기물훼손은 범죄행위이다. "
                + "범행 방법: 도심에 사람이 제일 드문 시간인 새벽 3시에 cctv가 없는 사각지대에서 그래피티를 함. "
                + "약점: 마지막으로 칠했던 그래피티 장소에서 치우지 못한 그래피티 통이 발견되었고, 그 통에는 너의 지문이 남아있었다.";
                break;
            default:
                scenarioText = "기본 사건 설정.";
                break;
        }

        // 공통 설정 + 선택된 봇에 따른 시나리오를 합쳐서 시스템 메시지 구성
        string systemMessage =
            "이제부터 너는 어느 사건의 범행을 저지른 용의자야, 너가 범죄를 저지른 사람 역할을 하는거야. 나는 심문관이고. "
            + "\r\n너는 지금 심문관인 나에게 심문을 받고 있어.\r\n너는 너가 범죄자가 아니라고 계속해서 설명해야하지만 거짓말은 할 수 없어."
            + "\r\n너는 자백율 이라는 파라미터를 가지고 있고 이게 100%가 되면 자신의 범죄사실을 자백해야한다."
            + "\r\n심문관이 너의 범죄사실에 가까운 질문을 할 때 마다 자백율이 20%씩 상승한다."
            + "\r\n너는 현재, 너의 기분을 '보통', '기쁨', '슬픔', '화남' 중에 하나로 표현해야한다. "
            + "\r\n///\r\n"
            + scenarioText
            + "\r\n///\r\n너는 이제부터 답변을 할 때 반드시 아래의 양식으로 답변을 해야한다."
            + "\r\n$기분$: {내용}\r\n$자백율$: {내용}\r\n$답변$: {내용}\r\n"
            + "\r\n예를 들어 다음과 같이 답변할 수 있다.\r\n$기분$: 화남\r\n$자백율$: 0\r\n$답변$: 제가 한 게 아니라니까요!";

        Debug.Log($"systemMessage: {systemMessage}");

        // 메시지 리스트 초기화
        messages = new List<ChatMessage>
        {
            new ChatMessage(ChatMessageRole.System, systemMessage)
        };

        inputField.text = "";
        string startString = "<#00A2FF>ChatGPT: 안녕하세요. 무슨 일로 부르신거죠?</color>";
        textField.text = startString;
        Debug.Log(startString);
    }

    /// <summary>
    /// StartRecording()
    /// - 마이크로부터 녹음을 시작하고 관련 버튼 상태를 업데이트
    /// - 녹음 설정(샘플레이트) 지정
    /// </summary>
    public void StartRecording()
    {
        recordedClip = Microphone.Start(null, false, 10, 16000);
        isRecording = true;

        // 녹음 중에는 다른 입력 버튼들 비활성화
        recordButton.interactable = false;
        stopButton.interactable = true;
        okBtn.interactable = false;
    }

    /// <summary>
    /// StopRecording()
    /// - 녹음을 중지하고 PCM 형식으로 변환
    /// - STT 변환 코루틴(SendSTTRequest) 실행
    /// </summary>
    public void StopRecording()
    {
        if (isRecording)
        {
            Microphone.End(null);
            isRecording = false;

            // 녹음 종료 후 버튼 상태 복원
            recordButton.interactable = true;
            stopButton.interactable = false;

            float[] samples = new float[recordedClip.samples];
            recordedClip.GetData(samples, 0);
            byte[] audioData = ConvertToPCM(samples);

            StartCoroutine(SendSTTRequest(audioData));
        }
    }

    /// <summary>
    /// ConvertToPCM(float[] samples)
    /// - float 형식 샘플 데이터를 PCM 형식의 byte 배열로 변환
    /// </summary>
    private byte[] ConvertToPCM(float[] samples)
    {
        short[] intData = new short[samples.Length];
        byte[] bytesData = new byte[samples.Length * 2];

        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * short.MaxValue);
            byte[] byteArray = BitConverter.GetBytes(intData[i]);
            byteArray.CopyTo(bytesData, i * 2);
        }
        return bytesData;
    }

    /// <summary>
    /// SendSTTRequest(byte[] audioData)
    /// - 녹음된 PCM 데이터를 Google Speech-to-Text API에 보내 텍스트 변환
    /// - 변환 성공 시 입력 필드에 결과 반영 후 자동으로 GetResponse 호출
    /// </summary>
    private IEnumerator SendSTTRequest(byte[] audioData)
    {
        string url = "https://speech.googleapis.com/v1/speech:recognize?key=" + googleApiKey;
        string jsonRequest = "{ " +
            "\"config\": { " +
            "\"encoding\": \"LINEAR16\", " +
            "\"sampleRateHertz\": 16000, " +
            "\"languageCode\": \"ko-KR\"" +
            "}, " +
            "\"audio\": { " +
            "\"content\": \"" + System.Convert.ToBase64String(audioData) + "\"" +
            "}" +
            "}";

        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(url, jsonRequest))
        {
            www.method = UnityWebRequest.kHttpVerbPOST;
            www.SetRequestHeader("Content-Type", "application/json");
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonRequest));
            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                okBtn.interactable = true; // 에러 발생 시 버튼 상태 복원
            }
            else
            {
                var jsonResponse = www.downloadHandler.text;
                var response = JsonUtility.FromJson<GoogleSpeechToTextResponse>(jsonResponse);

                if (response.results != null && response.results.Length > 0)
                {
                    inputField.text = response.results[0].alternatives[0].transcript;
                    GetResponse(); // STT 변환 성공 시 자동으로 GetResponse 호출
                }
                else
                {
                    okBtn.interactable = true; // STT 실패 시 버튼 상태 복원
                }
            }
        }
    }

    /// <summary>
    /// GetResponse()
    /// - 사용자 입력(텍스트)을 OpenAI ChatCompletion으로 전달해 응답을 받음
    /// - 받은 응답을 파싱 및 화면에 출력, 음성 합성 코루틴(SynthesizeSpeech) 실행
    /// </summary>
    private async void GetResponse()
    {
        if (inputField.text.Length < 1)
        {
            return;
        }

        // 모든 입력 버튼 비활성화
        okBtn.interactable = false;
        recordButton.interactable = false;

        ChatMessage userMessage = new ChatMessage
        {
            Role = ChatMessageRole.User,
            Content = inputField.text.Length > 100 ? inputField.text.Substring(0, 100) : inputField.text
        };

        Debug.Log($"{userMessage.Role} : {userMessage.Content}");
        messages.Add(userMessage);
        inputField.text = "";

        // 사용자 메시지: 흰색, 우측 정렬
        textField.text += string.Format("\n<#47C83E>You: {0}</color>", userMessage.Content);
        ScrollToBottom();

        var chatResult = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
        {
            Model = Model.GPT4,
            Temperature = 0.1,
            MaxTokens = 200,
            Messages = messages
        });

        ChatMessage responseMessage = chatResult.Choices[0].Message;
        Debug.Log($"{responseMessage.Role}: {responseMessage.Content}");
        messages.Add(responseMessage);

        // 응답에서 불필요한 정보를 제거하고, 정확히 분리
        string cleanedResponse = CleanResponse(responseMessage.Content);
        ParseResponse(cleanedResponse);

        // ChatGPT 응답: 파란색, 좌측 정렬 (response만 출력)
        textField.text += string.Format("\n<#00A2FF>ChatGPT: {0}</color>", response);
        ScrollToBottom();

        // TTS 요청 시작 (response만 전달)
        StartCoroutine(SynthesizeSpeech(response));
    }

    /// <summary>
    /// CleanResponse(string content)
    /// - 응답 문자열에서 불필요한 텍스트(헤더 등)를 제거
    /// </summary>
    private string CleanResponse(string content)
    {
        // 불필요한 텍스트 제거 (ASSISTANT, 헤더 등)
        if (content.StartsWith("ASSISTANT:", StringComparison.InvariantCultureIgnoreCase))
        {
            content = content.Substring("ASSISTANT:".Length).Trim();
        }
        return content;
    }

    /// <summary>
    /// ParseResponse(string content)
    /// - 응답 문자열을 분석하여 기분, 자백율, 답변을 분리
    /// - 파싱한 기분에 따라 애니메이션 업데이트
    /// </summary>
    private void ParseResponse(string content)
    {
        // 문자열에서 기분, 자백율, 답변 추출
        try
        {
            int moodStart = content.IndexOf("$기분$: ") + "$기분$: ".Length;
            int moodEnd = content.IndexOf("\n", moodStart);
            mood = content.Substring(moodStart, moodEnd - moodStart).Trim();

            int confessionRateStart = content.IndexOf("$자백율$: ") + "$자백율$: ".Length;
            int confessionRateEnd = content.IndexOf("\n", confessionRateStart);
            confessionRate = content.Substring(confessionRateStart, confessionRateEnd - confessionRateStart).Trim();

            int responseStart = content.IndexOf("$답변$: ") + "$답변$: ".Length;
            response = content.Substring(responseStart).Trim();

            // 기분에 따라 애니메이션 실행
            UpdateAnimation(mood);
        }
        catch (Exception e)
        {
            Debug.LogError("Parsing failed: " + e.Message);
            mood = "Unknown";
            confessionRate = "Unknown";
            response = "Parsing error occurred.";
        }

        // statusText 업데이트
        statusText.text = $"기분: {mood} | 자백율: {confessionRate}";
    }

    /// <summary>
    /// UpdateAnimation(string mood)
    /// - 파싱된 기분 상태에 따라 AnimationController를 호출
    /// </summary>
    private void UpdateAnimation(string mood)
    {
        if (animationController == null)
        {
            Debug.LogError("AnimationController is not assigned.");
            return;
        }

        // 기분에 따라 적절한 애니메이션 실행
        switch (mood)
        {
            case "보통":
                animationController.BotNormalAnimation();
                break;
            case "기쁨":
                animationController.BotJoyAnimation();
                break;
            case "화남":
                animationController.BotAngerAnimation();
                break;
            case "슬픔":
                animationController.BotSadnessAnimation();
                break;
            default:
                Debug.LogWarning($"Unknown mood: {mood}. No animation triggered.");
                animationController.BotNormalAnimation();
                break;
        }
    }

    /// <summary>
    /// SynthesizeSpeech(string text)
    /// - Google Text-to-Speech API를 통해 받은 텍스트를 음성으로 변환하여 재생
    /// - 음성 재생 후 버튼들 활성화
    /// </summary>
    private IEnumerator SynthesizeSpeech(string text)
    {
        //ko-KR-Standard-A, B, C, D
        string url = "https://texttospeech.googleapis.com/v1/text:synthesize?key=" + googleApiKey;
        string jsonRequest = "{ " +
            "\"input\": { \"text\": \"" + text + "\" }, " +
            "\"voice\": { \"languageCode\": \"ko-KR\", \"name\": \"ko-KR-Standard-D\" }, " +
            "\"audioConfig\": { \"audioEncoding\": \"LINEAR16\" }" +
        "}";

        using (UnityWebRequest www = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonRequest);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                var jsonResponse = www.downloadHandler.text;
                var audioContent = JsonUtility.FromJson<GoogleTTSResponse>(jsonResponse).audioContent;

                byte[] audioData = System.Convert.FromBase64String(audioContent);
                AudioClip clip = AudioClip.Create("TTS", audioData.Length, 1, 24000, false);
                float[] floatData = ConvertToFloatArray(audioData);
                clip.SetData(floatData, 0);

                audioSource.clip = clip;
                audioSource.Play();
            }
        }

        // TTS 처리가 완료되면 버튼들 다시 활성화
        okBtn.interactable = true;
        recordButton.interactable = true;
    }

    /// <summary>
    /// ConvertToFloatArray(byte[] byteArray)
    /// - TTS로 받은 byte 배열을 float 배열로 변환하여 AudioClip에 적용
    /// </summary>
    private float[] ConvertToFloatArray(byte[] byteArray)
    {
        float[] floatArray = new float[byteArray.Length / 2];
        for (int i = 0; i < floatArray.Length; i++)
        {
            floatArray[i] = (short)((byteArray[i * 2] | (byteArray[i * 2 + 1] << 8))) / 32768.0f;
        }
        return floatArray;
    }

    /// <summary>
    /// ScrollToBottom()
    /// - 대화 내용 업데이트 시, 자동으로 스크롤을 최하단으로 이동
    /// </summary>
    private void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    /// <summary>
    /// ReturnToMain()
    /// - 게임 메인 화면으로 돌아가는 함수 (GameManager 이용)
    /// </summary>
    public void ReturnToMain()
    {
        GameManager.Instance.OnReturnToMain();
    }

    //sts api 응답을 파싱하기 위한 데이터 구조
    [System.Serializable]
    public class GoogleSpeechToTextResponse
    {
        public Result[] results;

        [System.Serializable]
        public class Result
        {
            public Alternative[] alternatives;

            [System.Serializable]
            public class Alternative
            {
                public string transcript;
            }
        }
    }
    //tts api 응답을 파싱하기 위한 데이터 구조
    [System.Serializable]
    private class GoogleTTSResponse
    {
        public string audioContent; //json표현의 경우, 변환된 음성을 base64 문자열로 받는다 함
    }
}
