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
/// - Unity���� OpenAI API�� ���� �ν�/�ռ��� �����Ͽ� ê�� ����� �����ϴ� ��ũ��Ʈ
/// - ������� �ؽ�Ʈ �Ǵ� ���� �Է��� �޾� ê�� �亯�� ȭ�鿡 ����ϰ�, �������� ���
/// - Ư�� '��' �ó������� ���� �ɹ� ���� �帧�� ����
/// </summary>
public class OpenAIController : MonoBehaviour
{
    // UI ���
    public TMP_Text textField;
    public TMP_InputField inputField;
    public Button okBtn;
    public ScrollRect scrollRect;
    public Button recordButton;
    public Button stopButton;
    public AudioSource audioSource;
    public TMP_Text statusText;

    // API �� ���� ����
    private OpenAIAPI api;
    private List<ChatMessage> messages;
    private AudioClip recordedClip;
    private bool isRecording = false;
    private string googleApiKey = "*�˿�*";
    private string openApiKey = "*�˿�*";

    // ê�� �亯�� ���� ����
    string mood = "";             // "���"
    string confessionRate = "";   // "�ڹ���"
    string response = "";         // "�亯"

    // AnimationController ����
    public AnimationController animationController;

    /// <summary>
    /// Start()
    /// - OpenAI API �ʱ�ȭ �� �ʱ� ��ȭ ���� ����
    /// - ��ư ������ ��� �� ���� ��ư ���� �ʱ�ȭ
    /// </summary>
    void Start()
    {
        api = new OpenAIAPI(new APIAuthentication(openApiKey));
        StartConversation();

        // ��ư ������ ����
        okBtn.onClick.AddListener(() => GetResponse());
        recordButton.onClick.AddListener(StartRecording);
        stopButton.onClick.AddListener(StopRecording);

        // �ʱ� ��ư ���� ����
        stopButton.interactable = false;
    }

    /// <summary>
    /// StartConversation()
    /// - ���õ� �� ������ ���� �ý��� �޽���(�ó�����) ����
    /// - �޽��� ����Ʈ �ʱ�ȭ �� �ʱ� �ý��� �޽��� ����
    /// </summary>
    private void StartConversation()
    {
        // GameManager���� ���� ���õ� �� ���� ��������
        string selectedBot = GameManager.Instance.SelectedBot;

        // �� ���ݿ� ���� �ó����� �ؽ�Ʈ ����
        string scenarioText = "";
        switch (selectedBot)
        {
            case "A":
                scenarioText = "�ʰ� ������ ����� ������ ����: \r\n�ʴ� �ϼ����б��� ����Ƽ �л��̾�. "
                + "���б����� ���� ���� ������ 8�ÿ� ����� �������� ã�ư����� �������� ���� ���� ���� �䱸�� ������� �ʾҰ�, "
                + "�ʴ� ȱ�迡 �������� �����ž�. "
                + "���� ���: ī��� �����ƶǸ� �����Ͻô� �����Բ� ��Һ��� ���� �÷��� ���� ���� �� ����� Ÿ�� ��ġä�� ���ϰ� ���ð� �Ͽ� ������ ��������. "
                + "���� ����: �ɹ����� �ʰ� Ȧ�� 8�� 10�п� �������� �ٹ��Ͻô� A������ ���Դٴ� ���� �˰� �ִ�. "
                + "�׸��� �������� �濡�� ����� ���� ������ �߰��ߴ�.";
                break;
            case "B":
                scenarioText = "���: �ʴ� ���ī���� �ܰ�մ��̾�. �׻� ī�信�� ���ϴ� ������ ģ�ϰ� ������ ������, ���ֱ����� �� �� ������. "
                + "�ʴ� �� ���� �Ҹ��� ���ܼ� 12�� �ܿ��� �߿� ������ ī�䰡 ���� �ݴ� 10�� ���Ŀ� ������ ��������. "
                + "���� ���: ī�䰡 ���� �ݴ� �ð����� Ȧ�� �ɾ� �ִٰ�, ī�� ������ ����ϴ� �ð��� 10�� 30�п� "
                + "ī�� �Ĺ����� ������ ������ Ŀ�ٶ� ���� ��帧���� �޸� �����ߴ�. "
                + "����: ���� ���� ��Ȯ�� � ������ �����ߴ��� �� �� ��������, �ʿ� ������ ������ٴ� ��, "
                + "�ʰ� ī�䰡 ���� �ݴ� �ð����� Ȧ�� �־��ٴ� ��, �Ĺ����� ������ ������ �Ҹ��� ��ȴٴ� ��ݴ��� �����Ǿ���.";
                break;
            case "C":
                scenarioText = "���: ��� A���ÿ��� �� ������ �ҹ����� �׷���Ƽ ��Ƽ��Ʈ�� �⹰ �Ѽ� ������ �־���. "
                + "����� ���� ���� ���� ��, �������� �ǹ��� ���� ��ī�� �ڽ��� ��ũ�� ����� ������ ���̴�. "
                + "��ī�� ĥ���� �׸� ��ü�� �������̾�����, ������� ���� �⹰�Ѽ��� ���������̴�. "
                + "���� ���: ���ɿ� ����� ���� �幮 �ð��� ���� 3�ÿ� cctv�� ���� �簢���뿡�� �׷���Ƽ�� ��. "
                + "����: ���������� ĥ�ߴ� �׷���Ƽ ��ҿ��� ġ���� ���� �׷���Ƽ ���� �߰ߵǾ���, �� �뿡�� ���� ������ �����־���.";
                break;
            default:
                scenarioText = "�⺻ ��� ����.";
                break;
        }

        // ���� ���� + ���õ� ���� ���� �ó������� ���ļ� �ý��� �޽��� ����
        string systemMessage =
            "�������� �ʴ� ��� ����� ������ ������ �����ھ�, �ʰ� ���˸� ������ ��� ������ �ϴ°ž�. ���� �ɹ����̰�. "
            + "\r\n�ʴ� ���� �ɹ����� ������ �ɹ��� �ް� �־�.\r\n�ʴ� �ʰ� �����ڰ� �ƴ϶�� ����ؼ� �����ؾ������� �������� �� �� ����."
            + "\r\n�ʴ� �ڹ��� �̶�� �Ķ���͸� ������ �ְ� �̰� 100%�� �Ǹ� �ڽ��� ���˻���� �ڹ��ؾ��Ѵ�."
            + "\r\n�ɹ����� ���� ���˻�ǿ� ����� ������ �� �� ���� �ڹ����� 20%�� ����Ѵ�."
            + "\r\n�ʴ� ����, ���� ����� '����', '���', '����', 'ȭ��' �߿� �ϳ��� ǥ���ؾ��Ѵ�. "
            + "\r\n///\r\n"
            + scenarioText
            + "\r\n///\r\n�ʴ� �������� �亯�� �� �� �ݵ�� �Ʒ��� ������� �亯�� �ؾ��Ѵ�."
            + "\r\n$���$: {����}\r\n$�ڹ���$: {����}\r\n$�亯$: {����}\r\n"
            + "\r\n���� ��� ������ ���� �亯�� �� �ִ�.\r\n$���$: ȭ��\r\n$�ڹ���$: 0\r\n$�亯$: ���� �� �� �ƴ϶�ϱ��!";

        Debug.Log($"systemMessage: {systemMessage}");

        // �޽��� ����Ʈ �ʱ�ȭ
        messages = new List<ChatMessage>
        {
            new ChatMessage(ChatMessageRole.System, systemMessage)
        };

        inputField.text = "";
        string startString = "<#00A2FF>ChatGPT: �ȳ��ϼ���. ���� �Ϸ� �θ��Ű���?</color>";
        textField.text = startString;
        Debug.Log(startString);
    }

    /// <summary>
    /// StartRecording()
    /// - ����ũ�κ��� ������ �����ϰ� ���� ��ư ���¸� ������Ʈ
    /// - ���� ����(���÷���Ʈ) ����
    /// </summary>
    public void StartRecording()
    {
        recordedClip = Microphone.Start(null, false, 10, 16000);
        isRecording = true;

        // ���� �߿��� �ٸ� �Է� ��ư�� ��Ȱ��ȭ
        recordButton.interactable = false;
        stopButton.interactable = true;
        okBtn.interactable = false;
    }

    /// <summary>
    /// StopRecording()
    /// - ������ �����ϰ� PCM �������� ��ȯ
    /// - STT ��ȯ �ڷ�ƾ(SendSTTRequest) ����
    /// </summary>
    public void StopRecording()
    {
        if (isRecording)
        {
            Microphone.End(null);
            isRecording = false;

            // ���� ���� �� ��ư ���� ����
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
    /// - float ���� ���� �����͸� PCM ������ byte �迭�� ��ȯ
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
    /// - ������ PCM �����͸� Google Speech-to-Text API�� ���� �ؽ�Ʈ ��ȯ
    /// - ��ȯ ���� �� �Է� �ʵ忡 ��� �ݿ� �� �ڵ����� GetResponse ȣ��
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
                okBtn.interactable = true; // ���� �߻� �� ��ư ���� ����
            }
            else
            {
                var jsonResponse = www.downloadHandler.text;
                var response = JsonUtility.FromJson<GoogleSpeechToTextResponse>(jsonResponse);

                if (response.results != null && response.results.Length > 0)
                {
                    inputField.text = response.results[0].alternatives[0].transcript;
                    GetResponse(); // STT ��ȯ ���� �� �ڵ����� GetResponse ȣ��
                }
                else
                {
                    okBtn.interactable = true; // STT ���� �� ��ư ���� ����
                }
            }
        }
    }

    /// <summary>
    /// GetResponse()
    /// - ����� �Է�(�ؽ�Ʈ)�� OpenAI ChatCompletion���� ������ ������ ����
    /// - ���� ������ �Ľ� �� ȭ�鿡 ���, ���� �ռ� �ڷ�ƾ(SynthesizeSpeech) ����
    /// </summary>
    private async void GetResponse()
    {
        if (inputField.text.Length < 1)
        {
            return;
        }

        // ��� �Է� ��ư ��Ȱ��ȭ
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

        // ����� �޽���: ���, ���� ����
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

        // ���信�� ���ʿ��� ������ �����ϰ�, ��Ȯ�� �и�
        string cleanedResponse = CleanResponse(responseMessage.Content);
        ParseResponse(cleanedResponse);

        // ChatGPT ����: �Ķ���, ���� ���� (response�� ���)
        textField.text += string.Format("\n<#00A2FF>ChatGPT: {0}</color>", response);
        ScrollToBottom();

        // TTS ��û ���� (response�� ����)
        StartCoroutine(SynthesizeSpeech(response));
    }

    /// <summary>
    /// CleanResponse(string content)
    /// - ���� ���ڿ����� ���ʿ��� �ؽ�Ʈ(��� ��)�� ����
    /// </summary>
    private string CleanResponse(string content)
    {
        // ���ʿ��� �ؽ�Ʈ ���� (ASSISTANT, ��� ��)
        if (content.StartsWith("ASSISTANT:", StringComparison.InvariantCultureIgnoreCase))
        {
            content = content.Substring("ASSISTANT:".Length).Trim();
        }
        return content;
    }

    /// <summary>
    /// ParseResponse(string content)
    /// - ���� ���ڿ��� �м��Ͽ� ���, �ڹ���, �亯�� �и�
    /// - �Ľ��� ��п� ���� �ִϸ��̼� ������Ʈ
    /// </summary>
    private void ParseResponse(string content)
    {
        // ���ڿ����� ���, �ڹ���, �亯 ����
        try
        {
            int moodStart = content.IndexOf("$���$: ") + "$���$: ".Length;
            int moodEnd = content.IndexOf("\n", moodStart);
            mood = content.Substring(moodStart, moodEnd - moodStart).Trim();

            int confessionRateStart = content.IndexOf("$�ڹ���$: ") + "$�ڹ���$: ".Length;
            int confessionRateEnd = content.IndexOf("\n", confessionRateStart);
            confessionRate = content.Substring(confessionRateStart, confessionRateEnd - confessionRateStart).Trim();

            int responseStart = content.IndexOf("$�亯$: ") + "$�亯$: ".Length;
            response = content.Substring(responseStart).Trim();

            // ��п� ���� �ִϸ��̼� ����
            UpdateAnimation(mood);
        }
        catch (Exception e)
        {
            Debug.LogError("Parsing failed: " + e.Message);
            mood = "Unknown";
            confessionRate = "Unknown";
            response = "Parsing error occurred.";
        }

        // statusText ������Ʈ
        statusText.text = $"���: {mood} | �ڹ���: {confessionRate}";
    }

    /// <summary>
    /// UpdateAnimation(string mood)
    /// - �Ľ̵� ��� ���¿� ���� AnimationController�� ȣ��
    /// </summary>
    private void UpdateAnimation(string mood)
    {
        if (animationController == null)
        {
            Debug.LogError("AnimationController is not assigned.");
            return;
        }

        // ��п� ���� ������ �ִϸ��̼� ����
        switch (mood)
        {
            case "����":
                animationController.BotNormalAnimation();
                break;
            case "���":
                animationController.BotJoyAnimation();
                break;
            case "ȭ��":
                animationController.BotAngerAnimation();
                break;
            case "����":
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
    /// - Google Text-to-Speech API�� ���� ���� �ؽ�Ʈ�� �������� ��ȯ�Ͽ� ���
    /// - ���� ��� �� ��ư�� Ȱ��ȭ
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

        // TTS ó���� �Ϸ�Ǹ� ��ư�� �ٽ� Ȱ��ȭ
        okBtn.interactable = true;
        recordButton.interactable = true;
    }

    /// <summary>
    /// ConvertToFloatArray(byte[] byteArray)
    /// - TTS�� ���� byte �迭�� float �迭�� ��ȯ�Ͽ� AudioClip�� ����
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
    /// - ��ȭ ���� ������Ʈ ��, �ڵ����� ��ũ���� ���ϴ����� �̵�
    /// </summary>
    private void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    /// <summary>
    /// ReturnToMain()
    /// - ���� ���� ȭ������ ���ư��� �Լ� (GameManager �̿�)
    /// </summary>
    public void ReturnToMain()
    {
        GameManager.Instance.OnReturnToMain();
    }

    //sts api ������ �Ľ��ϱ� ���� ������ ����
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
    //tts api ������ �Ľ��ϱ� ���� ������ ����
    [System.Serializable]
    private class GoogleTTSResponse
    {
        public string audioContent; //jsonǥ���� ���, ��ȯ�� ������ base64 ���ڿ��� �޴´� ��
    }
}
