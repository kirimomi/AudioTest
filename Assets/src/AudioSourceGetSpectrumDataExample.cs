using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(AudioSource))]
public class AudioSourceGetSpectrumDataExample : MonoBehaviour
{

    [SerializeField]
    AudioSource m_micIn;

    [SerializeField]
    AudioSource m_SE;


    const int SAMPLE_NUMBER = 256; //配列のサイズ

    float m_thresholdFreq = 0.04f; //ピッチとして検出する最小の分布

    float m_thresholdVolume = 0.02f; // ボリューム検出の際の閾値
    
    float[] m_spectrum; //FFTされたデータ

    float m_fSample; //サンプリング周波数

    const float PITCH_MIN = 240f;

    const float PITCH_MAX = 260;

    const float SEC_REQUIER = 0.4f;

    const float TARGET_FREQ = 910f;


    const float CHARA_SCALE_MAX = 1.0f;
    const float CHARA_SCALE_MIN = 0.7f;

    float count = 0;

    [SerializeField]
    GameObject m_charaRoot;

    [SerializeField]
    GameObject m_charaNormal;

    [SerializeField]
    GameObject m_charaVacuum;

    [SerializeField]
    GameObject m_charaOk;


    [SerializeField]
    GameObject okText;

    [SerializeField]
    Text debugText;

    [SerializeField]
    Text modeButtonText;


    enum CheckMode
    {
        Freq,
        Volume,
    }
    CheckMode m_checkMode = CheckMode.Freq;

    void Start()
    {

        //SetMode(CheckMode.Volume);
        SetMode(CheckMode.Freq);

        InitScene();

        m_spectrum = new float[SAMPLE_NUMBER];

        // 空の Audio Sourceを取得
        //m_audioSource = GetComponent<AudioSource>();

        /*
        int minFreq;
        int maxFreq;
        //Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);
        Debug.Log("minFreq:" + minFreq + "  maxFreq:" + maxFreq);
        debugText.text = "minFreq:" + minFreq + "  maxFreq:" + maxFreq;
        */

        int freq = 24000;

        m_fSample = (float)freq;




        // Audio Source の Audio Clip をマイク入力に設定
        // 引数は、デバイス名（null ならデフォルト）、ループ、何秒取るか、サンプリング周波数
        m_micIn.clip = Microphone.Start(null, true, 10, freq);
        
        //audio_.loop = true;

        //audio.mute = true;
        
        // マイクが Ready になるまで待機（一瞬）
        while (Microphone.GetPosition(null) <= 0) { }
        // 再生開始（録った先から再生、スピーカーから出力するとハウリングします）
        m_micIn.Play();
    }


    void Update()
    {

        m_micIn.GetSpectrumData(m_spectrum, 0, FFTWindow.BlackmanHarris);

        float pitch = AnalyzeSound();

        bool checkOk = false;

        switch (m_checkMode)
        {
            case CheckMode.Freq:
                checkOk = PITCH_MIN < pitch && pitch < PITCH_MAX;
                break;
            default:
                //いんちき
                checkOk = IsPitchOverThreshold(TARGET_FREQ);
                break;
        }

        //float volume = GetAveragedVolume();

        //string str = pitch + "Hz" + (isOverTh ? "○" : "") + "\nvolume:" + volume;
        //string str = pitch + "Hz"  + "\nvolume:" + volume;
        string str = pitch + "Hz" + "\nsample:" + m_fSample;

        Debug.Log(str);
        debugText.text = str;



       if (checkOk)
       {
            count += Time.deltaTime;
            if (!m_charaOk.activeSelf)
            {
                m_charaNormal.SetActive(false);
                m_charaVacuum.SetActive(true);
            }

        }else
        {
            count -= Time.deltaTime;
            if(count < 0)
            {
                count = 0;
            }
            if (!m_charaOk.activeSelf)
            {
                m_charaNormal.SetActive(true);
                m_charaVacuum.SetActive(false);
            }

        }

        float scale = CHARA_SCALE_MIN +  count * (CHARA_SCALE_MAX - CHARA_SCALE_MIN) / SEC_REQUIER;
        if (m_charaOk.activeSelf)
        {
            scale = CHARA_SCALE_MAX;
        }
        m_charaRoot.transform.localScale = Vector3.one * scale;


        if (SEC_REQUIER < count)
        {
            m_charaNormal.SetActive(false);
            m_charaVacuum.SetActive(false);

            if (!m_charaOk.activeSelf)
            {
                m_SE.Play();
            }
            m_charaOk.SetActive(true);
            okText.SetActive(true);
        }



        //m_audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        for (int i = 0; i < m_spectrum.Length-1; i++)
        {
            Debug.DrawLine(new Vector3(i*0.1f , m_spectrum[i] + 10, 0), new Vector3((i+1) * 0.1f, m_spectrum[i + 1] + 10, 0), Color.red);
            Debug.DrawLine(new Vector3(i * 0.1f, Mathf.Log(m_spectrum[i]) + 10, 2), new Vector3((i+1) * 0.1f, Mathf.Log(m_spectrum[i+1]) + 10, 2), Color.cyan);
            Debug.DrawLine(new Vector3(Mathf.Log(i), m_spectrum[i] - 10, 1), new Vector3(Mathf.Log(i+1), m_spectrum[i+1] - 10, 1), Color.green);
            Debug.DrawLine(new Vector3(Mathf.Log(i), Mathf.Log(m_spectrum[i]), 3), new Vector3(Mathf.Log(i+1), Mathf.Log(m_spectrum[i+1]), 3), Color.blue);


        }
    }




    //現在のピッチを返す 取れなかったら0
    float AnalyzeSound()
    {
        //m_micIn.GetSpectrumData(m_spectrum, 0, FFTWindow.BlackmanHarris);
        float maxV = 0;
        int maxN = 0;
        //最大値（ピッチ）を見つける。ただし、閾値は超えている必要がある
        for (int i = 0; i < SAMPLE_NUMBER; i++)
        {
            if (m_spectrum[i] > maxV && m_spectrum[i] > m_thresholdFreq)
            {
                maxV = m_spectrum[i];
                maxN = i;
            }
        }

        float freqN = maxN;
        if (maxN > 0 && maxN < SAMPLE_NUMBER - 1)
        {
            //隣のスペクトルも考慮する
            float dL = m_spectrum[maxN - 1] / m_spectrum[maxN];
            float dR = m_spectrum[maxN + 1] / m_spectrum[maxN];
            freqN += 0.5f * (dR * dR - dL * dL);
        }
        float pitchValue = freqN * (m_fSample / 2) / SAMPLE_NUMBER;
        return pitchValue;
    }


    bool IsPitchOverThreshold(float freq)
    {
        m_micIn.GetSpectrumData(m_spectrum, 0, FFTWindow.BlackmanHarris);
        int sampleNumber = (int)(freq * SAMPLE_NUMBER * 2 / m_fSample);

        if(m_thresholdVolume < m_spectrum[sampleNumber])
        {
            return true;
        }
        return false;
    }

    float GetAveragedVolume()
    {
        float[] data = new float[256];
        float a = 0;
        m_micIn.GetOutputData(data, 0);
        foreach (float s in data)
        {
            a += Mathf.Abs(s);
        }
        return a / 256;
    }


    void InitScene()
    {
        m_charaNormal.SetActive(true);
        m_charaVacuum.SetActive(false);
        m_charaOk.SetActive(false);
        m_charaRoot.transform.localScale = Vector3.one * CHARA_SCALE_MIN;

        okText.SetActive(false);

        count = 0;
    }

    public void OnButtonPressed()
    {
        InitScene();
    }


    public void OnHomePressed()
    {
        SceneManager.LoadScene("Main");
    }

    //評価モード切替
    public void OnModePressed()
    {
        if(m_checkMode == CheckMode.Freq)
        {
            SetMode(CheckMode.Volume);
        }
        else
        {
            SetMode(CheckMode.Freq);
        }

    }

    void SetMode(CheckMode mode)
    {
        switch (mode)
        {
            case CheckMode.Freq:
                m_checkMode = CheckMode.Freq;
                modeButtonText.text = "F";
                break;
            case CheckMode.Volume:
                m_checkMode = CheckMode.Volume;
                modeButtonText.text = "V";
                break;
        }

    }

    void DrawLine(Vector3 startPos, Vector3 endPos, Color color)
    {
        GameObject newLine = new GameObject("Line");
        LineRenderer lRend = newLine.AddComponent<LineRenderer>();
        lRend.positionCount = 2;
        lRend.startWidth = 0.03f;
        lRend.endWidth = 0.03f;
        lRend.startColor = color;
        lRend.endColor = color;
        lRend.SetPosition(0, startPos);
        lRend.SetPosition(1, endPos);
    }

    
}
 