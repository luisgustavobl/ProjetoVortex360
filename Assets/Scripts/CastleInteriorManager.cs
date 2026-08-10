using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class CastleInteriorManager : MonoBehaviour
{
    [Header("Configuração do Panorama Interno")]
    public Material skyboxMaterial;
    public Cubemap fotoInterior; // Ou Texture2D se usar Panoramic Shader

    [Header("Referência da Câmera")]
    public Transform cameraTransform;

    [Header("Botão de Saída")]
    public GameObject exitCastleButton;

    [Header("Ângulo da Porta de Saída")]
    [Tooltip("Ângulo Y mínimo para enxergar a saída do castelo")]
    public float anguloMinimoSaida = 150f;
    [Tooltip("Ângulo Y máximo para enxergar a saída do castelo")]
    public float anguloMaximoSaida = 210f;


    [Header("Botão de Acordar")]
    public GameObject wakeUpButton;

    [Header("Objeto Ema Dormindo")]
    public GameObject sleepingEma;

    [Header("Objeto Ema Acordada")]
    public GameObject awokenEma;

    [Header("Ângulo da Moema")]
    [Tooltip("Ângulo Y mínimo para enxergar a Moema")]
    public float anguloMinimoEma = 150f;
    [Tooltip("Ângulo Y máximo para enxergar a Moema")]
    public float anguloMaximoEma = 210f;


    [Header("Cena Externa")]
    public string nomeCenaExterna = "SampleScene";

    void Start()
    {
        // Aplica a foto interna no Skybox
        if (skyboxMaterial != null && fotoInterior != null)
        {
            if (skyboxMaterial.HasProperty("_Tex"))
                skyboxMaterial.SetTexture("_Tex", fotoInterior);
            else
                skyboxMaterial.SetTexture("_MainTex", fotoInterior);

            DynamicGI.UpdateEnvironment();
        }

        if (exitCastleButton != null) exitCastleButton.SetActive(false);
        if (wakeUpButton != null) wakeUpButton.SetActive(false);

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        VerificarSaidaCastelo();
        VerificarEmaDormindo();
    }

    private void VerificarSaidaCastelo()
    {
        if (exitCastleButton == null || cameraTransform == null) return;

        float anguloY = cameraTransform.eulerAngles.y;

        // Normaliza qualquer valor negativo ou maior que 360° para a faixa [0, 360)
        anguloY = NormalizarAngulo(anguloY);

        // Verifica se a câmera está virada para a porta de saída
        bool olhandoParaSaida = ChecarAnguloNoIntervalo(anguloY, anguloMinimoSaida, anguloMaximoSaida);
        exitCastleButton.SetActive(olhandoParaSaida);
    }
    private void VerificarEmaDormindo()
    {
        if (wakeUpButton == null || cameraTransform == null) return;

        float anguloY = cameraTransform.eulerAngles.y;

        // Normaliza qualquer valor negativo ou maior que 360° para a faixa [0, 360)
        anguloY = NormalizarAngulo(anguloY);

        // Verifica se a câmera está virada para a porta de saída
        bool olhandoParaEma = ChecarAnguloNoIntervalo(anguloY, anguloMinimoEma, anguloMaximoEma);
        wakeUpButton.SetActive(olhandoParaEma);
    }

    /// <summary>
    /// Garante que o ângulo fique estritamente entre 0° e 360° mesmo com valores negativos
    /// </summary>
    private float NormalizarAngulo(float angulo)
    {
        while (angulo < 0f) angulo += 360f;
        while (angulo >= 360f) angulo -= 360f;
        return angulo;
    }

    private bool ChecarAnguloNoIntervalo(float angulo, float min, float max)
    {
        if (min <= max) return angulo >= min && angulo <= max;
        else return angulo >= min || angulo <= max;
    }

    /// <summary>
    /// Retorna para a cena da área externa. Vincular ao OnClick do ExitCastleButton.
    /// </summary>
    public void SairDoCastelo()
    {
        if (!string.IsNullOrEmpty(nomeCenaExterna))
        {
            SceneManager.LoadScene(nomeCenaExterna);
        }
    }
    public void AcordarEma()
    {
        // Revelar Ema Acordada
        sleepingEma.SetActive(false);

        awokenEma.SetActive(true);
    }
}