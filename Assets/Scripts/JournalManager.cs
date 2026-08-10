using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class JournalManager : MonoBehaviour
{
    public static JournalManager Instance;

    [Header("Progresso do Diário")]
    public int totalFragmentos = 5;
    private int fragmentosColetados = 0;

    [Header("Interface do Usuário (UI)")]
    public GameObject progressUI; // Antigo PainelProgressoUI
    public TextMeshProUGUI progressText; // Antigo TextoProgresso (Ex: "3/5")
    public GameObject painelDiarioCompleto;
    public TextMeshProUGUI blockedTextWarning; // Antigo TextoAvisoBloqueio

    [Header("Efeitos Sonoros dos Fragmentos")]
    public AudioSource audioSource;

    [Tooltip("Lista de efeitos sonoros sorteados aleatoriamente a cada coleta")]
    public AudioClip[] sonsColetaFragmento; // Array para os áudios (bookCollect1, bookCollect2, etc.)

    private Coroutine coroutineAviso;

    /// <summary>
    /// Oculta o painel do diário completo quando o jogador clica no botão "X"
    /// </summary>
    public void FecharPainelDiario()
    {
        if (painelDiarioCompleto != null)
        {
            painelDiarioCompleto.SetActive(false);
        }
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Carrega o número de fragmentos já salvos
        fragmentosColetados = PlayerPrefs.GetInt("FragmentosColetados", 0);

        if (blockedTextWarning != null) blockedTextWarning.gameObject.SetActive(false);
        if (painelDiarioCompleto != null) painelDiarioCompleto.SetActive(false);

        // Tenta buscar o AudioSource no próprio GameObject caso não tenha sido atribuído no Inspector
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        // Se já coletou pelo menos um fragmento, mantém a UI visível ao retornar à cena
        if (progressUI != null)
        {
            progressUI.SetActive(fragmentosColetados > 0);
        }

        AtualizarUI();
    }

    /// <summary>
    /// Verifica se um fragmento específico já foi coletado e salvo previamente
    /// </summary>
    public bool FragmentoJaColetado(int index)
    {
        return PlayerPrefs.GetInt($"Fragmento_{index}", 0) == 1;
    }

    /// <summary>
    /// Coleta um fragmento por índice garantindo persistência do progresso
    /// </summary>
    public void ColetarFragmento(int index)
    {
        if (FragmentoJaColetado(index)) return;

        PlayerPrefs.SetInt($"Fragmento_{index}", 1);

        fragmentosColetados++;
        PlayerPrefs.SetInt("FragmentosColetados", fragmentosColetados);
        PlayerPrefs.Save();

        // Toca um som sorteado da lista
        TocarSomColetaAleatorio();

        if (progressUI != null && !progressUI.activeSelf)
        {
            progressUI.SetActive(true);
        }

        AtualizarUI();

        if (fragmentosColetados >= totalFragmentos)
        {
            CompletarDiario();
        }
    }

    /// <summary>
    /// Método antigo sem parâmetro mantido para garantir compatibilidade
    /// </summary>
    public void ColetarFragmento()
    {
        fragmentosColetados++;
        PlayerPrefs.SetInt("FragmentosColetados", fragmentosColetados);
        PlayerPrefs.Save();

        TocarSomColetaAleatorio();

        if (progressUI != null && !progressUI.activeSelf)
        {
            progressUI.SetActive(true);
        }

        AtualizarUI();

        if (fragmentosColetados >= totalFragmentos)
        {
            CompletarDiario();
        }
    }

    /// <summary>
    /// Sorteia um áudio aleatório da lista e toca sem alterar o pitch
    /// </summary>
    private void TocarSomColetaAleatorio()
    {
        if (audioSource != null && sonsColetaFragmento != null && sonsColetaFragmento.Length > 0)
        {
            // Sorteia um índice do array de sons
            int indiceSorteado = Random.Range(0, sonsColetaFragmento.Length);
            AudioClip somSorteado = sonsColetaFragmento[indiceSorteado];

            if (somSorteado != null)
            {
                audioSource.PlayOneShot(somSorteado);
            }
        }
    }

    private void AtualizarUI()
    {
        if (progressText != null)
        {
            progressText.text = $"{fragmentosColetados}/{totalFragmentos}";
        }
    }

    private void CompletarDiario()
    {
        if (painelDiarioCompleto != null)
        {
            painelDiarioCompleto.SetActive(true);
        }

        Debug.Log("Todos os fragmentos foram coletados! A porta do Castelinho foi desbloqueada.");
    }

    public bool PodeEntrarNoCastelo()
    {
        return fragmentosColetados >= totalFragmentos;
    }

    public void ExibirAvisoBloqueado()
    {
        if (coroutineAviso != null) StopCoroutine(coroutineAviso);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.TocarErroBloqueio();
        }

        coroutineAviso = StartCoroutine(MostrarTextoTemporario("Você não pode entrar aqui ainda!", 2.0f));
    }

    private IEnumerator MostrarTextoTemporario(string mensagem, float duracao)
    {
        if (blockedTextWarning != null)
        {
            blockedTextWarning.text = mensagem;
            blockedTextWarning.gameObject.SetActive(true);
            yield return new WaitForSeconds(duracao);
            blockedTextWarning.gameObject.SetActive(false);
        }
    }

    public void OcultarProgressoEntradaCastelo()
    {
        if (progressUI != null) progressUI.SetActive(false);
    }

    /// <summary>
    /// Limpa os dados de coleta ao iniciar um Novo Jogo no Menu
    /// </summary>
    public static void ResetarProgressoColeta()
    {
        PlayerPrefs.DeleteKey("FragmentosColetados");
        for (int i = 0; i < 10; i++)
        {
            PlayerPrefs.DeleteKey($"Fragmento_{i}");
        }
        PlayerPrefs.Save();
    }
}