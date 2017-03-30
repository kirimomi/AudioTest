using System.Collections;using System.Collections.Generic;using UnityEngine;using UnityEngine.UI;[RequireComponent(typeof(AudioSource))]public class AudioSourceGetSpectrumDataExample : MonoBehaviour{    AudioSource audio_;    const int SAMPLE_NUMBER = 1024; //配列のサイズ    const float THRESHOLD = 0.04f; //ピッチとして検出する最小の分布    float pitchValue; //ピッチの周波数

    float[] spectrum; //FFTされたデータ    float fSample; //サンプリング周波数

    const float PITCH_MIN = 450f;    const float PITCH_MAX = 480;    const float SEC_REQUIER = 1f;

    float count = 0;

    [SerializeField]
    GameObject mamiNormal;

    [SerializeField]
    GameObject mamiSmile;

    [SerializeField]
    GameObject okText;

    [SerializeField]
    Text debugText;    


    void Start()    {        mamiSmile.SetActive(false);        okText.SetActive(false);        mamiNormal.transform.localScale = Vector3.zero;        spectrum = new float[SAMPLE_NUMBER];        // 空の Audio Sourceを取得        audio_ = GetComponent<AudioSource>();        int minFreq;        int maxFreq;        Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);        Debug.Log("minFreq:" + minFreq + "  maxFreq:" + maxFreq);        debugText.text = "minFreq:" + minFreq + "  maxFreq:" + maxFreq;        fSample = (float)maxFreq;        // Audio Source の Audio Clip をマイク入力に設定        // 引数は、デバイス名（null ならデフォルト）、ループ、何秒取るか、サンプリング周波数        audio_.clip = Microphone.Start(null, true, 10, maxFreq);        audio_.loop = true;        //audio.mute = true;        // マイクが Ready になるまで待機（一瞬）        while (Microphone.GetPosition(null) <= 0) { }        // 再生開始（録った先から再生、スピーカーから出力するとハウリングします）        audio_.Play();    }    void Update()    {        float pitch = AnalyzeSound();

        if (0 < pitch)        {            Debug.Log(pitch + "Hz");
            debugText.text = pitch + "Hz";            if (PITCH_MIN< pitch && pitch < PITCH_MAX)
            {
                count += Time.deltaTime;
            }        }else
        {
            count -= Time.deltaTime;
            if(count < 0)
            {
                count = 0;
            }
        }

        float scale = count / SEC_REQUIER;
        mamiNormal.transform.localScale = Vector3.one * scale;


        if (SEC_REQUIER < count)
        {
            mamiNormal.SetActive(false);
            mamiSmile.SetActive(true);
            okText.SetActive(true);
        }







        audio_.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);        for (int i = 1; i < spectrum.Length - 1; i++)        {            Debug.DrawLine(new Vector3(i - 1, spectrum[i] + 10, 0), new Vector3(i, spectrum[i + 1] + 10, 0), Color.red);            Debug.DrawLine(new Vector3(i - 1, Mathf.Log(spectrum[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(spectrum[i]) + 10, 2), Color.cyan);            Debug.DrawLine(new Vector3(Mathf.Log(i - 1), spectrum[i - 1] - 10, 1), new Vector3(Mathf.Log(i), spectrum[i] - 10, 1), Color.green);            Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 3), Color.blue);        }
    }




    //現在のピッチを返す 取れなかったら0
    float AnalyzeSound()    {        audio_.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);        float maxV = 0;        int maxN = 0;        //最大値（ピッチ）を見つける。ただし、閾値は超えている必要がある        for (int i = 0; i < SAMPLE_NUMBER; i++)        {            if (spectrum[i] > maxV && spectrum[i] > THRESHOLD)            {                maxV = spectrum[i];                maxN = i;            }        }        float freqN = maxN;        if (maxN > 0 && maxN < SAMPLE_NUMBER - 1)        {            //隣のスペクトルも考慮する            float dL = spectrum[maxN - 1] / spectrum[maxN];            float dR = spectrum[maxN + 1] / spectrum[maxN];            freqN += 0.5f * (dR * dR - dL * dL);        }        pitchValue = freqN * (fSample / 2) / SAMPLE_NUMBER;        return pitchValue;    }}