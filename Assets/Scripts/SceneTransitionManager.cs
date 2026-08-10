using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [Header("Referência da UI de Transição")]
    public CanvasGroup transitionCanvasGroup;
    public float duracaoFade = 0.5f;

    [Header("Som de Transição de Cena")]
    public AudioClip somTransicaoCena;
    [Range(0f, 1f)]
    public float volumeTransicao = 0.6f;

    private bool emTransicao = false;

    void Awake()
    {
        // Garante que exista apenas uma instância do gerenciador no jogo todo
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Sobrevive à troca de cenas
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Garante que o painel inicie visível e faça o Fade In ao entrar na primeira cena
        if (transitionCanvasGroup != null)
        {
            transitionCanvasGroup.alpha = 1.0f;
            StartCoroutine(FadeIn());
        }
    }

    /// <summary>
    /// Método público para carregar qualquer cena com efeito de Fade In / Fade Out
    /// </summary>
    public void CarregarCena(string nomeDaCena)
    {
        if (!emTransicao)
        {
            StartCoroutine(RotinaTrocaCena(nomeDaCena));
        }
    }

    private IEnumerator RotinaTrocaCena(string nomeDaCena)
    {
        emTransicao = true;

        // Toca o som de transição se configurado
        if (AudioManager.Instance != null && somTransicaoCena != null)
        {
            AudioManager.Instance.audioSource.PlayOneShot(somTransicaoCena, volumeTransicao);
        }

        // 1. Fade Out (Tela escurece)
        yield return StartCoroutine(FadeOut());

        // 2. Carrega a nova cena de forma assíncrona
        AsyncOperation operacao = SceneManager.LoadSceneAsync(nomeDaCena);
        while (!operacao.isDone)
        {
            yield return null;
        }

        // 3. Fade In (Tela clareia na nova cena)
        yield return StartCoroutine(FadeIn());

        emTransicao = false;
    }

    private IEnumerator FadeOut()
    {
        transitionCanvasGroup.blocksRaycasts = true; // Bloqueia cliques na UI antiga
        float tempo = 0;

        while (tempo < duracaoFade)
        {
            tempo += Time.deltaTime;
            transitionCanvasGroup.alpha = Mathf.Lerp(0f, 1f, tempo / duracaoFade);
            yield return null;
        }

        transitionCanvasGroup.alpha = 1.0f;
    }

    private IEnumerator FadeIn()
    {
        float tempo = 0;

        while (tempo < duracaoFade)
        {
            tempo += Time.deltaTime;
            transitionCanvasGroup.alpha = Mathf.Lerp(1f, 0f, tempo / duracaoFade);
            yield return null;
        }

        transitionCanvasGroup.alpha = 0.0f;
        transitionCanvasGroup.blocksRaycasts = false; // Libera cliques na nova UI
    }
}