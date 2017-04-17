using UnityEngine;

public class LineTest : MonoBehaviour
{

    [SerializeField]
    GameObject m_main;

    AudioSourceGetSpectrumDataExample m_asgstde;

    void Start()
    {
        m_asgstde = m_main.GetComponent<AudioSourceGetSpectrumDataExample>();
    }

    void Update()
    {
        for (int i = 0; i < m_asgstde.Spectrum.Length - 1; i++)
        {
            Debug.DrawLine(new Vector3(i * 0.1f, Mathf.Log(m_asgstde.Spectrum[i]) + 10, 2), new Vector3((i + 1) * 0.1f, Mathf.Log(m_asgstde.Spectrum[i + 1]) + 10, 2), Color.cyan);
        }
    }

    static Material lineMaterial;
    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    // Will be called after all regular rendering is done
    public void OnRenderObject()
    {
        GL.Clear(false, false, new Color(0, 0, 0, 0));

        CreateLineMaterial();
        // Apply the line material
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix(transform.localToWorldMatrix);

        // Draw lines
        GL.Begin(GL.LINES);

        const float SCALE_X = 0.02f;
        const float SCALE_Y = 0.3f;
        float offsetX = -1f * SCALE_X * m_asgstde.Spectrum.Length / 2f ;

        for (int i = 0; i < m_asgstde.Spectrum.Length - 1; i++)
        {
            GL.Color(Color.green);

            GL.Vertex3(i * SCALE_X + offsetX, Mathf.Log(m_asgstde.Spectrum[i]) * SCALE_Y, 0);
            GL.Vertex3((i + 1) * SCALE_X + offsetX, Mathf.Log(m_asgstde.Spectrum[i + 1]) * SCALE_Y, 0);

            //Debug.DrawLine(new Vector3(i * 0.1f, Mathf.Log(m_asgstde.Spectrum[i]) + 10, 2), new Vector3((i + 1) * 0.1f, Mathf.Log(m_asgstde.Spectrum[i + 1]) + 10, 2), Color.cyan);
        }


        GL.End();
        GL.PopMatrix();
    }
}