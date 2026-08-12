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

    [Header("Ângulo Y de Visão para Orientação")]
    [Tooltip("Ângulo Y da bússola que representa a FRENTE perfeita da rua/caminho (ex: 0)")]
    public float anguloCentralFrente = 0f;

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

    // Enum e variável para rastrear os 6 setores angulares planejados
    private enum SetorVisao { Frente, FrenteDireita, TrasDireita, Tras, TrasEsquerda, FrenteEsquerda }
    private SetorVisao setorAtual = SetorVisao.Frente;

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
        VerificarOrientacaoCamera();

        // Mapeamento idêntico ao planejamento visual de 6 setores (2 x 90° e 4 x 45°)
        switch (setorAtual)
        {
            case SetorVisao.Frente:
                // W Proxima | S Anterior
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) ProximaFoto();
                else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) FotoAnterior();
                break;

            case SetorVisao.FrenteDireita:
                // W Proxima | D Anterior | A Proxima | S Anterior
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) ProximaFoto();
                else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) FotoAnterior();
                else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) ProximaFoto();
                else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) FotoAnterior();
                break;

            case SetorVisao.TrasDireita:
                // W Anterior | D Anterior | A Proxima | S Proxima
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) FotoAnterior();
                else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) FotoAnterior();
                else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) ProximaFoto();
                else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) ProximaFoto();
                break;

            case SetorVisao.Tras:
                // W Anterior | S Proxima
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) FotoAnterior();
                else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) ProximaFoto();
                break;

            case SetorVisao.TrasEsquerda:
                // W Anterior | A Anterior | S Proxima | D Proxima
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) FotoAnterior();
                else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) FotoAnterior();
                else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) ProximaFoto();
                else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) ProximaFoto();
                break;

            case SetorVisao.FrenteEsquerda:
                // W Proxima | A Anterior | S Anterior | D Proxima
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) ProximaFoto();
                else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) FotoAnterior();
                else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) FotoAnterior();
                else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) ProximaFoto();
                break;
        }
    }

    private void VerificarOrientacaoCamera()
    {
        if (cameraTransform == null) return;

        float anguloY = cameraTransform.eulerAngles.y;
        anguloY = NormalizarAngulo(anguloY);

        // 1. Calcula a diferença angular direta em relação ao anguloCentralFrente do Inspector
        float anguloRelativo = NormalizarAngulo(anguloY - anguloCentralFrente);

        // 2. Classifica a câmera nos 6 setores partindo do ângulo central
        if (anguloRelativo >= 315f || anguloRelativo < 45f)
        {
            setorAtual = SetorVisao.Frente; // 90°
            olhandoParaFrente = true;
        }
        else if (anguloRelativo >= 45f && anguloRelativo < 90f)
        {
            setorAtual = SetorVisao.FrenteDireita; // 45°
            olhandoParaFrente = true;
        }
        else if (anguloRelativo >= 90f && anguloRelativo < 135f)
        {
            setorAtual = SetorVisao.TrasDireita; // 45°
            olhandoParaFrente = false;
        }
        else if (anguloRelativo >= 135f && anguloRelativo < 225f)
        {
            setorAtual = SetorVisao.Tras; // 90°
            olhandoParaFrente = false;
        }
        else if (anguloRelativo >= 225f && anguloRelativo < 270f)
        {
            setorAtual = SetorVisao.TrasEsquerda; // 45°
            olhandoParaFrente = false;
        }
        else // 270° ate 315°
        {
            setorAtual = SetorVisao.FrenteEsquerda; // 45°
            olhandoParaFrente = true;
        }

        // 3. Interação com a porta do Castelo
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