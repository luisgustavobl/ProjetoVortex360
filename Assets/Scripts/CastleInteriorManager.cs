using System.Collections;
using UnityEngine;
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

    [Header("Objetos da Ema")]
    public GameObject sleepingEma;
    public GameObject awokenEma;

    [Header("Configuração de Acordar & Delays")]
    [Tooltip("Tempo em segundos de espera após o clique até a Ema acordar")]
    public float delayParaAcordar = 1.0f;
    [Tooltip("Tempo em segundos de espera após a Ema acordar até exibir a tela de vitória")]
    public float delayPainelVitoria = 2.0f;
    private bool emaJaAcordou = false;

    [Header("Efeito Sonoro (Áudios)")]
    public AudioSource audioSource;
    public AudioClip somAcordar;
    [Tooltip("Som reproduzido assim que a tela de vitória aparece")]
    public AudioClip somVitoria;

    [Header("Ângulo da Moema")]
    [Tooltip("Ângulo Y mínimo para enxergar a Moema")]
    public float anguloMinimoEma = 60f;
    [Tooltip("Ângulo Y máximo para enxergar a Moema")]
    public float anguloMaximoEma = 100f;

    [Header("Telas e Botões de Fim de Jogo")]
    public GameObject gameWonPanel;             // Painel Pop-up de Vitória
    public GameObject inGameReturnToMenuButton; // Botão fixo na tela ativado após explorar

    [Header("Nomes das Cenas")]
    public string nomeCenaExterna = "SampleScene";
    public string nomeCenaMenu = "MenuScene";

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
        if (gameWonPanel != null) gameWonPanel.SetActive(false);
        if (inGameReturnToMenuButton != null) inGameReturnToMenuButton.SetActive(false);

        // Garante o estado inicial das Emas
        if (sleepingEma != null) sleepingEma.SetActive(true);
        if (awokenEma != null) awokenEma.SetActive(false);

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        // Caso o AudioSource não tenha sido atribuído manualmente, tenta pegar do objeto
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
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
        anguloY = NormalizarAngulo(anguloY);

        bool olhandoParaSaida = ChecarAnguloNoIntervalo(anguloY, anguloMinimoSaida, anguloMaximoSaida);
        exitCastleButton.SetActive(olhandoParaSaida);
    }

    private void VerificarEmaDormindo()
    {
        if (wakeUpButton == null || cameraTransform == null) return;

        // Se a Ema já acordou, não exibe mais o botão de acordar
        if (emaJaAcordou)
        {
            if (wakeUpButton.activeSelf) wakeUpButton.SetActive(false);
            return;
        }

        float anguloY = cameraTransform.eulerAngles.y;
        anguloY = NormalizarAngulo(anguloY);

        bool olhandoParaEma = ChecarAnguloNoIntervalo(anguloY, anguloMinimoEma, anguloMaximoEma);
        wakeUpButton.SetActive(olhandoParaEma);
    }

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

    public void SairDoCastelo()
    {
        CarregarCenaTransicao(nomeCenaExterna);
    }

    /// <summary>
    /// Vincular ao OnClick() do WakeUpButton na Unity.
    /// </summary>
    public void AcordarEma()
    {
        if (!emaJaAcordou)
        {
            StartCoroutine(RotinaAcordarEmaComDelay());
        }
    }

    private IEnumerator RotinaAcordarEmaComDelay()
    {
        if (wakeUpButton != null) wakeUpButton.SetActive(false);

        // Delay antes de acordar
        yield return new WaitForSeconds(delayParaAcordar);

        // Troca os sprites
        if (sleepingEma != null) sleepingEma.SetActive(false);
        if (awokenEma != null) awokenEma.SetActive(true);

        // Toca o som de bocejo / despertar
        if (audioSource != null && somAcordar != null)
        {
            audioSource.PlayOneShot(somAcordar);
        }

        emaJaAcordou = true;

        // Delay até abrir o painel de vitória
        yield return new WaitForSeconds(delayPainelVitoria);

        if (gameWonPanel != null)
        {
            gameWonPanel.SetActive(true);
        }

        // Toca o som da vitória quando o painel é exibido
        if (audioSource != null && somVitoria != null)
        {
            audioSource.PlayOneShot(somVitoria);
        }
    }

    /// <summary>
    /// Vincular ao OnClick() do ReturnToMenuButton (no painel de vitória ou na HUD).
    /// </summary>
    public void VoltarAoMenuPrincipal()
    {
        CarregarCenaTransicao(nomeCenaMenu);
    }

    /// <summary>
    /// Carrega a cena desejada utilizando o SceneTransitionManager (com suporte a Fade Out/In)
    /// ou utiliza o SceneManager tradicional como fallback para testes locais.
    /// </summary>
    private void CarregarCenaTransicao(string nomeCena)
    {
        if (!string.IsNullOrEmpty(nomeCena))
        {
            if (SceneTransitionManager.Instance != null)
            {
                SceneTransitionManager.Instance.CarregarCena(nomeCena);
            }
            else
            {
                SceneManager.LoadScene(nomeCena);
            }
        }
    }

    /// <summary>
    /// Vincular ao OnClick() do ReturnToGameButton (no GameWonPanel).
    /// Permite ao jogador continuar navegando no panorama interno.
    /// </summary>
    public void ContinuarExplorando()
    {
        if (gameWonPanel != null)
        {
            gameWonPanel.SetActive(false);
        }

        // Ativa o botão fixo na tela para retornar ao menu quando o jogador desejar
        if (inGameReturnToMenuButton != null)
        {
            inGameReturnToMenuButton.SetActive(true);
        }
    }
}