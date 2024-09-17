using System.Collections;
using System.Collections.Generic;
using System.IO;
using Ionic.Zip;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;


[System.Serializable]
public class VoskFinalResult
{
    public VoskAlternative[] alternatives;
}

[System.Serializable]
public class VoskAlternative
{
    public float confidence;
    public VoskResult[] result;
    public string text;
}

[System.Serializable]
public class VoskResult
{
    public float end;
    public float start;
    public string word;
}

public class SpeechRecognizer : MonoBehaviour
{
    const string m_ModelName = "vosk-model-small-de-0.15";
    private string m_ModelPath;
    private bool m_IsModelDecompressed = false;
    //private bool m_AreVoiceSettingsSet = false;
    private bool m_IsVoskLoaded = false;
    private bool m_HasLoadingFailed = false;
    private int m_SampleRate;
    private int m_SampleSize;
    private int m_RecordingLengthSec;
    private Vosk.Model m_VoskModel;
    private Vosk.VoskRecognizer m_VoskRecognizer;
    private int m_RecordingCursor;
    private AudioClip m_AudioClip;
    private float[] m_RecordedData;
    private short[] m_ChunkData;
    private UnityAction<string> m_OnFinishedRecording;
    public bool HasLoadingFailed => m_HasLoadingFailed;


    void Start()
    {
#if UNITY_EDITOR_LINUX
#else
        m_IsModelDecompressed = false;
        //m_AreVoiceSettingsSet = false;
        m_IsVoskLoaded = false;
        m_HasLoadingFailed = false;
        m_AudioClip = null;
        m_ModelPath = Path.Combine(Application.persistentDataPath, m_ModelName);
        StartCoroutine(DecompressModel());
#endif
    }

    private IEnumerator DecompressModel()
    {
        if (Directory.Exists(m_ModelPath))
        {
            Debug.Log("Already decompressed");
            m_IsModelDecompressed = true;
            yield break;
        }
        string compressedFilePath = Path.Combine(Application.streamingAssetsPath, $"{m_ModelName}.zip");
        Debug.Log($"Decompressing {compressedFilePath} to {Application.persistentDataPath}");
        Stream dataStream;
        if (compressedFilePath.Contains("://"))
        {
            UnityWebRequest www = UnityWebRequest.Get(compressedFilePath);
            www.SendWebRequest();
            while (!www.isDone)
            {
                yield return null;
            }
            dataStream = new MemoryStream(www.downloadHandler.data);
        }
        else
        {
            dataStream = File.OpenRead(compressedFilePath);
        }
        var zipFile = ZipFile.Read(dataStream);
        Debug.Log("Reading Zip File");
        zipFile.ExtractAll(Application.persistentDataPath);
        zipFile.Dispose();
        Debug.Log("Decompress Finished"); 
        m_IsModelDecompressed = true;
    }

    void InitVosk()
    {
        Debug.Log("Start initializing Vosk");
        m_VoskModel = new Vosk.Model(m_ModelPath);
        if (m_VoskModel == null)
        {
            m_HasLoadingFailed = true;
            Debug.Log("Vosk Model loading has failed");
            return;
        } 
        else
        {
            Debug.Log("Vosk Model successfully loaded");
        }
        if (Microphone.devices.Length == 0)
        {
            Debug.Log("No microphone has been found");
            m_HasLoadingFailed = true;
        }
        Microphone.GetDeviceCaps(null, out int minFreq, out int maxFreq);
        m_SampleRate = System.Math.Clamp(44100, minFreq, maxFreq);
        string grm = "[";
        foreach (GameState.CommandState cmd in GameStateManager.Instance.gameState.dogs[0].commands)
        {
            var commandPhrase = cmd.phrases.ToLower();
            grm += $"\"{commandPhrase}\", ";
        }
        grm += "\"[unk]\"]";
        Debug.Log($"Init Vosk with the commands {grm}");
        m_VoskRecognizer = new Vosk.VoskRecognizer(m_VoskModel, m_SampleRate, grm);
        m_VoskRecognizer.SetMaxAlternatives(1);
        m_VoskRecognizer.SetWords(true);
        m_VoskRecognizer.SetPartialWords(true);
        m_AudioClip = null;
        m_RecordingLengthSec = 2;
        m_SampleSize = m_SampleRate * m_RecordingLengthSec;
        m_RecordedData = new float[m_SampleSize];
        m_ChunkData = new short[m_SampleSize];

        Debug.Log("Vosk is ready!");
        m_IsVoskLoaded = true;
    }

    public void StartRecording(UnityAction<string> onFinishedRecording)
    {
        if (m_IsVoskLoaded)
        {
            m_OnFinishedRecording = onFinishedRecording;
            m_AudioClip = Microphone.Start(null, false, m_RecordingLengthSec, m_SampleRate);
            Debug.Log("Recording...");
        }
        else
        {
            Debug.Log("Vosk hasn't been loaded");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsModelDecompressed && !m_IsVoskLoaded && !m_HasLoadingFailed)
        {
            InitVosk();
        }
        if (m_AudioClip != null && !Microphone.IsRecording(null))
        {
            Debug.Log("Finished recording");
            m_AudioClip.GetData(m_RecordedData, 0);
            for (int i = 0; i < m_SampleSize; i += 1)
            {
                m_ChunkData[i] = (short)(m_RecordedData[i] * 65536f);
            }
            m_VoskRecognizer.AcceptWaveform(m_ChunkData, m_SampleSize);
            m_AudioClip = null;
            string result = m_VoskRecognizer.FinalResult();
            var finalResult = JsonUtility.FromJson<VoskFinalResult>(result);
            if (finalResult.alternatives.Length == 0)
            {
                m_OnFinishedRecording.Invoke("");
            }
            else
            {
                m_OnFinishedRecording.Invoke(finalResult.alternatives[0].text);
            }
        }
    }
}
