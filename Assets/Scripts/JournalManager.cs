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