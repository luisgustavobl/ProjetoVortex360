using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StreetViewManager : MonoBehaviour
{
    [Header("Configuração de Fotos")]
    public Material skyboxMaterial;
    public Cubemap[] listaDeFotos;
    private int indiceAtual = 0;

    [Header("Referência da Câmera")]
    public Transform cameraTransform;

    [Header("Efeitos de Limite de Mapa")]
    public CanvasGroup flashOverlay;
    public CanvasGroup warningCanvasGroup;

    [Header("Fragmentos do Diário")]
    public JournalFragment[] fragmentosDoDiario;

    [Header("Tempos de Animação")]
    public float opacidadeFlash = 0.8f;
    public float tempoExibicaoTexto = 1.5f;
    public float duracaoFadeOut = 1.0f;

    [Header("Interação com o Castelinho (Panorama 15)")]
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

    void Start()
    {
        // Recupera o último panorama visitado se o jogador estiver voltando do castelo
        if (PlayerPrefs.HasKey("UltimoPanoramaIndex"))
        {
            indiceAtual = PlayerPrefs.GetInt("UltimoPanoramaIndex");
        }

        AtualizarFoto();

        if (flashOverlay != null) flashOverlay.alpha = 0;
        if (warningCanvasGroup != null) warningCanvasGroup.alpha = 0;
        if (enterCastleButton != null) enterCastleButton.SetActive(false);

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        // Navegação via teclado (WASD / Setas)
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            ProximaFoto();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            FotoAnterior();
        }

        // Checagem do ângulo de visão para o EnterCastleButton
        VerificarInteracaoCastelinho();
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

    private void VerificarInteracaoCastelinho()
    {
        if (enterCastleButton == null || cameraTransform == null) return;

        // Verifica se o jogador está no panorama do Castelinho
        if (indiceAtual == indicePanoramaCastelinho)
        {
            float anguloY = cameraTransform.eulerAngles.y;

            if (anguloY < 0) anguloY += 360f;

            bool olhandoParaOCastelo = ChecarAnguloNoIntervalo(anguloY, anguloMinimoCastelo, anguloMaximoCastelo);

            enterCastleButton.SetActive(olhandoParaOCastelo);
        }
        else
        {
            if (enterCastleButton.activeSelf)
            {
                enterCastleButton.SetActive(false);
            }
        }
    }

    private bool ChecarAnguloNoIntervalo(float angulo, float min, float max)
    {
        if (min <= max)
        {
            return angulo >= min && angulo <= max;
        }
        else
        {
            return angulo >= min || angulo <= max;
        }
    }

    public void EntrarNoCastelo()
    {
        // Salva a posição atual antes de transitar para o castelo
        PlayerPrefs.SetInt("UltimoPanoramaIndex", indiceAtual);
        PlayerPrefs.Save();

        // Verifica a regra de gamificação via JournalManager
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
            // Fallback caso o JournalManager não esteja na cena
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

        // Gerencia quais fragmentos aparecem no panorama atual
        AtualizarVisibilidadeFragmentos();
    }

    private void AtualizarVisibilidadeFragmentos()
    {
        if (fragmentosDoDiario == null) return;

        foreach (var fragmento in fragmentosDoDiario)
        {
            if (fragmento != null)
            {
                // Ativa apenas se o panorama do fragmento for igual ao panorama atual
                bool deveAparecer = (fragmento.panoramaIndex == indiceAtual);
                fragmento.gameObject.SetActive(deveAparecer);
            }
        }
    }
}