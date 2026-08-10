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
        if (progressUI != null) progressUI.SetActive(false);
        if (painelDiarioCompleto != null) painelDiarioCompleto.SetActive(false);
        if (blockedTextWarning != null) blockedTextWarning.gameObject.SetActive(false);

        AtualizarUI();
    }

    public void ColetarFragmento()
    {
        fragmentosColetados++;

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
}