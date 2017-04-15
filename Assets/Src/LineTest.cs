using UnityEngine;

public class LineTest : MonoBehaviour
{
    // When added to an object, draws colored rays from the
    // transform position.
    int m_lineCount = 1;
    float radius = 3.0f;

    [SerializeField]
    GameObject m_main;

    AudioSourceGetSpectrumDataExample m_asgstde;

    void Start()
    {
        m_asgstde = m_main.GetComponent<AudioSourceGetSpectrumDataExample>();
    }

    void Update()
    {
        m_lineCount += 1;
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

        const float SCALE_X = 0.04f;
        const float SCALE_Y = 0.3f;
        float OFFSET_X = -1f * SCALE_X * m_asgstde.Spectrum.Length / 2f ;

        for (int i = 0; i < m_asgstde.Spectrum.Length - 1; i++)
        {
            GL.Color(Color.green);

            GL.Vertex3(i * SCALE_X + OFFSET_X, Mathf.Log(m_asgstde.Spectrum[i]) * SCALE_Y, 0);
            GL.Vertex3((i + 1) * SCALE_X + OFFSET_X, Mathf.Log(m_asgstde.Spectrum[i + 1]) * SCALE_Y, 0);

            //Debug.DrawLine(new Vector3(i * 0.1f, Mathf.Log(m_asgstde.Spectrum[i]) + 10, 2), new Vector3((i + 1) * 0.1f, Mathf.Log(m_asgstde.Spectrum[i + 1]) + 10, 2), Color.cyan);
        }


        /*
        for (int i = 0; i < m_lineCount; ++i)
        {
            float a = i / (float)m_lineCount;
            float angle = a * Mathf.PI * 2;
            // Vertex colors change from red to green
            GL.Color(new Color(a, 1 - a, 0, 0.8F));
            // One vertex at transform position
            GL.Vertex3(0, 0, 0);
            // Another vertex at edge of circle
            GL.Vertex3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
        }*/
        GL.End();
        GL.PopMatrix();
    }
}