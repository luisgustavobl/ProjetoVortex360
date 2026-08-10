using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StreetViewManager : MonoBehaviour
{
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

    void Start()
    {
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

        // Garante que ambos os botões fiquem visíveis na interface
        if (stepUpButton != null) stepUpButton.gameObject.SetActive(true);
        if (stepBackButton != null) stepBackButton.gameObject.SetActive(true);

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        // Configura os ouvintes de clique nos botões via script
        ConfigurarEventosDeClique();
    }

    private void ConfigurarEventosDeClique()
    {
        if (stepUpButton != null)
        {
            // Limpa qualquer evento antigo e vincula a lógica com inversão de ângulo
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
        // Navegação por teclado (WASD / Setas)
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

        // Verifica se está dentro da faixa de frente (-90° a 90°, ou seja, 270° a 90°)
        olhandoParaFrente = ChecarAnguloNoIntervalo(anguloY, anguloMinimoFrente, anguloMaximoFrente);

        // Checagem para o botão de entrar no Castelinho
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
        TocarSomClique();

        if (olhandoParaFrente) ProximaFoto();
        else FotoAnterior();
    }

    public void AoClicarSetaBaixo()
    {
        TocarSomClique();

        if (olhandoParaFrente) FotoAnterior();
        else ProximaFoto();
    }

    private void TocarSomClique()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.TocarCliqueBotao();
        }
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
            return;
        }

        indiceAtual++;
        AtualizarFoto();
    }

    public void FotoAnterior()
    {
        if (listaDeFotos.Length == 0) return;

        if (indiceAtual <= 0)
        {
            DispararAlertaLimite();
            return;
        }

        indiceAtual--;
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
                SceneManager.LoadScene(nomeCenaInteriorCastelo);
            }
            else
            {
                JournalManager.Instance.ExibirAvisoBloqueado();
            }
        }
        else
        {
            SceneManager.LoadScene(nomeCenaInteriorCastelo);
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
    }

    private void AtualizarVisibilidadeFragmentos()
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