using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Linq;
using UnityEngine.SceneManagement;


public class AudioSourceGetSpectrumDataExample : MonoBehaviour
{

    [SerializeField]
    AudioSource m_micIn;

    [SerializeField]
    AudioSource m_SE;


    const int SAMPLE_NUMBER = 256; //配列のサイズ

    float m_thresholdFreq = 0.04f; //ピッチとして検出する最小の分布

    float m_thresholdVolume = 0.02f; // ボリューム検出の際の閾値
    
    public float[] Spectrum; //FFTされたデータ

    float m_fSample; //サンプリング周波数

    struct EvaluationRange
    {
        public float min;
        public float max;
    }

    const int RANGE_MAX = 10;

    float m_time = 0;

    EvaluationRange GetEvaluationRange(int range)
    {
        Assert.IsTrue(0 <= range && range < RANGE_MAX);

        EvaluationRange ret = new EvaluationRange();
        switch (range)
        {
            case 0:
                ret.min = 430f;
                ret.max = 450f;
                break;
            case 1:
                ret.min = 880f;
                ret.max = 940f;
                break;
            case 2:
                ret.min = 1350f;
                ret.max = 1390f;
                break;
            case 3:
                ret.min = 1760f;
                ret.max = 1840f;
                break;
            case 4:
                ret.min = 2200f;
                ret.max = 2260f;
                break;
            case 5:
                ret.min = 2720f;
                ret.max = 2760f;
                break;
            case 6:
                ret.min = 430f;
                ret.max = 450f;
                break;
            case 7:
                ret.min = 430f;
                ret.max = 450f;
                break;
            case 8:
                ret.min = 430f;
                ret.max = 450f;
                break;
            case 9:
                ret.min = 430f;
                ret.max = 450f;
                break;
            default:
                ret.min = 450f;
                ret.max = 550f;
                break;
        }

        return ret;

    }


    const float PITCH_MIN = 440f;

    const float PITCH_MAX = 460f;

    const float SEC_REQUIER = 0.4f;

    const float TARGET_FREQ = 450f;


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
    CheckMode m_checkMode = CheckMode.Volume;

    void Start()
    {

        //SetMode(CheckMode.Volume);
        SetMode(CheckMode.Freq);

        InitScene();

        Spectrum = new float[SAMPLE_NUMBER];

        // 空の Audio Sourceを取得
        //m_audioSource = GetComponent<AudioSource>();

        int minFreq;
        int maxFreq;
        Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);
        Debug.Log("minFreq:" + minFreq + "  maxFreq:" + maxFreq);
        debugText.text = "minFreq:" + minFreq + "  maxFreq:" + maxFreq;

        //maxFreq = 22050;
        //maxFreq = 11025;

        m_fSample = (float)maxFreq;




        // Audio Source の Audio Clip をマイク入力に設定
        // 引数は、デバイス名（null ならデフォルト）、ループ、何秒取るか、サンプリング周波数
        m_micIn.clip = Microphone.Start(null, true, 1, maxFreq);
        
        // マイクが Ready になるまで待機（一瞬）
        while (Microphone.GetPosition(null) <= 0) { }

        // 再生開始（録った先から再生、スピーカーから出力するとハウリングします）
        m_micIn.Play();
    }


    void Update()
    {

        //m_micIn.GetSpectrumData(Spectrum, 0, FFTWindow.BlackmanHarris);
        m_micIn.GetSpectrumData(Spectrum, 0, FFTWindow.Rectangular);

        float pitch = AnalyzeSound();

        bool checkOk = false;

        switch (m_checkMode)
        {
            case CheckMode.Freq:
                for(int i = 0; i < RANGE_MAX; i++)
                {
                    if (GetEvaluationRange(i).min < pitch && pitch < GetEvaluationRange(i).max)
                    {
                        checkOk = true;
                        break;
                    }
                }
                break;
            default:
                //いんちき
                checkOk = IsPitchOverThreshold(TARGET_FREQ);
                break;
        }

        //float volume = GetAveragedVolume();

        //string str = pitch + "Hz" + (isOverTh ? "○" : "") + "\nvolume:" + volume;
        //string str = pitch + "Hz"  + "\nvolume:" + volume;
        //string str = pitch + "Hz" + "\nsample:" + m_fSample;
        string str = pitch + "Hz";

        if (0 < pitch)
        {
            Debug.Log(str);
        }
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

        for (int i = 0; i < Spectrum.Length-1; i++)
        {
            Debug.DrawLine(new Vector3(i*0.1f , Spectrum[i] + 10, 0), new Vector3((i+1) * 0.1f, Spectrum[i + 1] + 10, 0), Color.red);
            Debug.DrawLine(new Vector3(i * 0.1f, Mathf.Log(Spectrum[i]) + 10, 2), new Vector3((i+1) * 0.1f, Mathf.Log(Spectrum[i+1]) + 10, 2), Color.cyan);
            Debug.DrawLine(new Vector3(Mathf.Log(i), Spectrum[i] - 10, 1), new Vector3(Mathf.Log(i+1), Spectrum[i+1] - 10, 1), Color.green);
            Debug.DrawLine(new Vector3(Mathf.Log(i), Mathf.Log(Spectrum[i]), 3), new Vector3(Mathf.Log(i+1), Mathf.Log(Spectrum[i+1]), 3), Color.blue);


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
            if (Spectrum[i] > maxV && Spectrum[i] > m_thresholdFreq)
            {
                maxV = Spectrum[i];
                maxN = i;
            }
        }

        float freqN = maxN;
        if (maxN > 0 && maxN < SAMPLE_NUMBER - 1)
        {
            //隣のスペクトルも考慮する
            float dL = Spectrum[maxN - 1] / Spectrum[maxN];
            float dR = Spectrum[maxN + 1] / Spectrum[maxN];
            freqN += 0.5f * (dR * dR - dL * dL);
        }
        float pitchValue = freqN * (m_fSample / 2) / SAMPLE_NUMBER;
        return pitchValue;
    }


    bool IsPitchOverThreshold(float freq)
    {
        m_micIn.GetSpectrumData(Spectrum, 0, FFTWindow.BlackmanHarris);
        int sampleNumber = (int)(freq * SAMPLE_NUMBER * 2 / m_fSample);

        if(m_thresholdVolume < Spectrum[sampleNumber])
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

}
 