using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StreetViewManager : MonoBehaviour
{
    public static StreetViewManager Instance;

    [Header("Configuração de Fotos")]
    public Material skyboxMaterial;
    public Cubemap[] listaDeFotos;

    [Tooltip("Índice do panorama onde o jogador iniciará ao começar um Novo Jogo (ex: 5 para a 6ª foto)")]
    public int indiceInicialDefault = 0;

    private int indiceAtual = 0;

    [Header("Referência da Câmera")]
    public Transform cameraTransform;

    [Header("Botões de Navegação Contextual (UI)")]
    public Button stepUpButton;   // Botão da Seta de Cima (Seguir)
    public Button stepBackButton; // Botão da Seta de Baixo (Recuar)

    [Header("Ângulos de Visão para Orientação")]
    [Tooltip("Ângulo Y mínimo na bússola para considerar FRENTE (-90° normalizado vira 270°)")]
    public float anguloMinimoFrente = 270f;
    [Tooltip("Ângulo Y máximo na bússola para considerar FRENTE (90°)")]
    public float anguloMaximoFrente = 90f;

    [Header("Efeitos de Limite de Mapa")]
    public CanvasGroup flashOverlay;
    public CanvasGroup warningCanvasGroup;

    [Header("Fragmentos do Diário")]
    public JournalFragment[] fragmentosDoDiario;

    [Header("Tempos de Animação")]
    public float opacidadeFlash = 0.8f;
    public float tempoExibicaoTexto = 1.5f;
    public float duracaoFadeOut = 1.0f;

    [Header("Interação com o Castelinho")]
    [Tooltip("Índice do panorama do Castelinho (ex: 14 para a foto 15 no vetor base 0)")]
    public int indicePanoramaCastelinho = 14;
    public GameObject enterCastleButton;

    [Header("Ângulo de Visão do Castelo")]
    [Tooltip("Ângulo Y mínimo na bússola do jogo para ver o castelo")]
    public float anguloMinimoCastelo = 30f;
    [Tooltip("Ângulo Y máximo na bússola do jogo para ver o castelo")]
    public float anguloMaximoCastelo = 70f;
    [Tooltip("Nome da cena do interior do castelo")]
    public string nomeCenaInteriorCastelo = "CastleInteriorScene";

    private Coroutine coroutineAlerta;
    private bool olhandoParaFrente = true;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 1. Carrega o panorama inicial ou salvo no PlayerPrefs
        if (PlayerPrefs.HasKey("UltimoPanoramaIndex"))
        {
            indiceAtual = PlayerPrefs.GetInt("UltimoPanoramaIndex");
        }
        else
        {
            indiceAtual = Mathf.Clamp(indiceInicialDefault, 0, Mathf.Max(0, listaDeFotos.Length - 1));
        }

        AtualizarFoto();

        if (flashOverlay != null) flashOverlay.alpha = 0;
        if (warningCanvasGroup != null) warningCanvasGroup.alpha = 0;
        if (enterCastleButton != null) enterCastleButton.SetActive(false);

        if (stepUpButton != null) stepUpButton.gameObject.SetActive(true);
        if (stepBackButton != null) stepBackButton.gameObject.SetActive(true);

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        ConfigurarEventosDeClique();

        // 2. Aguarda um frame para que todos os fragmentos e managers tenham rodado o Awake/Start após a transição de cena
        StartCoroutine(InicializarFragmentosAposCarregamento());
    }

    private IEnumerator InicializarFragmentosAposCarregamento()
    {
        yield return null; // Aguarda 1 frame completo do Unity

        if (fragmentosDoDiario == null || fragmentosDoDiario.Length == 0)
        {
            fragmentosDoDiario = FindObjectsByType<JournalFragment>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        }

        AtualizarVisibilidadeFragmentos();
    }

    public int ObterIndiceAtual()
    {
        return indiceAtual;
    }

    private void ConfigurarEventosDeClique()
    {
        if (stepUpButton != null)
        {
            stepUpButton.onClick.RemoveAllListeners();
            stepUpButton.onClick.AddListener(AoClicarSetaCima);
        }

        if (stepBackButton != null)
        {
            stepBackButton.onClick.RemoveAllListeners();
            stepBackButton.onClick.AddListener(AoClicarSetaBaixo);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            ProximaFoto();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            FotoAnterior();
        }

        VerificarOrientacaoCamera();
    }

    private void VerificarOrientacaoCamera()
    {
        if (cameraTransform == null) return;

        float anguloY = cameraTransform.eulerAngles.y;
        anguloY = NormalizarAngulo(anguloY);

        olhandoParaFrente = ChecarAnguloNoIntervalo(anguloY, anguloMinimoFrente, anguloMaximoFrente);

        if (enterCastleButton != null)
        {
            if (indiceAtual == indicePanoramaCastelinho)
            {
                bool olhandoCastelo = ChecarAnguloNoIntervalo(anguloY, anguloMinimoCastelo, anguloMaximoCastelo);
                enterCastleButton.SetActive(olhandoCastelo);
            }
            else if (enterCastleButton.activeSelf)
            {
                enterCastleButton.SetActive(false);
            }
        }
    }

    public void AoClicarSetaCima()
    {
        if (olhandoParaFrente) ProximaFoto();
        else FotoAnterior();
    }

    public void AoClicarSetaBaixo()
    {
        if (olhandoParaFrente) FotoAnterior();
        else ProximaFoto();
    }

    private float NormalizarAngulo(float angulo)
    {
        while (angulo < 0f) angulo += 360f;
        while (angulo >= 360f) angulo -= 360f;
        return angulo;
    }

    private bool ChecarAnguloNoIntervalo(float angulo, float min, float max)
    {
        if (min <= max)
            return angulo >= min && angulo <= max;
        else
            return angulo >= min || angulo <= max;
    }

    public void ProximaFoto()
    {
        if (listaDeFotos.Length == 0) return;

        if (indiceAtual >= listaDeFotos.Length - 1)
        {
            DispararAlertaLimite();
            if (AudioManager.Instance != null) AudioManager.Instance.TocarErroBloqueio();
            return;
        }

        indiceAtual++;
        if (AudioManager.Instance != null) AudioManager.Instance.TocarSomPassoPanorama();
        AtualizarFoto();
    }

    public void FotoAnterior()
    {
        if (listaDeFotos.Length == 0) return;

        if (indiceAtual <= 0)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.TocarErroBloqueio();
            DispararAlertaLimite();
            return;
        }

        indiceAtual--;
        if (AudioManager.Instance != null) AudioManager.Instance.TocarSomPassoPanorama();
        AtualizarFoto();
    }

    public void EntrarNoCastelo()
    {
        PlayerPrefs.SetInt("UltimoPanoramaIndex", indiceAtual);
        PlayerPrefs.Save();

        if (JournalManager.Instance != null)
        {
            if (JournalManager.Instance.PodeEntrarNoCastelo())
            {
                JournalManager.Instance.OcultarProgressoEntradaCastelo();
                CarregarCenaTransicao(nomeCenaInteriorCastelo);
            }
            else
            {
                JournalManager.Instance.ExibirAvisoBloqueado();
            }
        }
        else
        {
            CarregarCenaTransicao(nomeCenaInteriorCastelo);
        }
    }

    private void CarregarCenaTransicao(string nomeCena)
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

    private void DispararAlertaLimite()
    {
        if (coroutineAlerta != null) StopCoroutine(coroutineAlerta);
        coroutineAlerta = StartCoroutine(EfeitoAlertaBarreira());
    }

    private IEnumerator EfeitoAlertaBarreira()
    {
        if (flashOverlay != null) flashOverlay.alpha = opacidadeFlash;
        if (warningCanvasGroup != null) warningCanvasGroup.alpha = 1.0f;

        yield return new WaitForSeconds(tempoExibicaoTexto);

        float tempo = 0;
        float alphaInicialFlash = flashOverlay != null ? flashOverlay.alpha : 0;
        float alphaInicialTexto = warningCanvasGroup != null ? warningCanvasGroup.alpha : 0;

        while (tempo < duracaoFadeOut)
        {
            tempo += Time.deltaTime;
            float fatorLinear = tempo / duracaoFadeOut;

            if (flashOverlay != null)
                flashOverlay.alpha = Mathf.Lerp(alphaInicialFlash, 0, fatorLinear);

            if (warningCanvasGroup != null)
                warningCanvasGroup.alpha = Mathf.Lerp(alphaInicialTexto, 0, fatorLinear);

            yield return null;
        }

        if (flashOverlay != null) flashOverlay.alpha = 0;
        if (warningCanvasGroup != null) warningCanvasGroup.alpha = 0;
    }

    private void AtualizarFoto()
    {
        if (skyboxMaterial != null && listaDeFotos.Length > 0 && listaDeFotos[indiceAtual] != null)
        {
            if (skyboxMaterial.HasProperty("_Tex"))
            {
                skyboxMaterial.SetTexture("_Tex", listaDeFotos[indiceAtual]);
            }
            else
            {
                skyboxMaterial.SetTexture("_MainTex", listaDeFotos[indiceAtual]);
            }

            DynamicGI.UpdateEnvironment();
        }

        AtualizarVisibilidadeFragmentos();

        // Atualiza o minimapa com o índice do panorama atual
        if (MinimapManager.Instance != null)
        {
            MinimapManager.Instance.AtualizarPosicaoMinimapa(indiceAtual);
        }
    }

    public void AtualizarVisibilidadeFragmentos()
    {
        if (fragmentosDoDiario == null) return;

        foreach (var fragmento in fragmentosDoDiario)
        {
            if (fragmento != null)
            {
                bool ehOPanoramaCorreto = (fragmento.panoramaIndex == indiceAtual);
                bool jaFoiColetado = JournalManager.Instance != null && JournalManager.Instance.FragmentoJaColetado(fragmento.fragmentIndex);

                fragmento.gameObject.SetActive(ehOPanoramaCorreto && !jaFoiColetado);
            }
        }
    }
}