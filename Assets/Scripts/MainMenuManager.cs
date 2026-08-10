using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Configurações de Cenas")]
    [Tooltip("Nome exato da cena do jogo onde estão os panoramas 360")]
    public string nomeCenaJogo = "SampleScene"; // Altere se sua cena tiver outro nome

    [Header("Painéis UI (Opcional)")]
    public GameObject painelInstrucoes;

    /// <summary>
    /// Carrega a cena principal do jogo. Vincular ao OnClick do Botão Jogar.
    /// </summary>
    public void IniciarJogo()
    {
        if (PlayerPrefs.HasKey("UltimoPanoramaIndex"))
        {
            PlayerPrefs.DeleteKey("UltimoPanoramaIndex");
            PlayerPrefs.Save();
        }

        JournalManager.ResetarProgressoColeta();

        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.CarregarCena(nomeCenaJogo);
        }
        else if (!string.IsNullOrEmpty(nomeCenaJogo))
        {
            SceneManager.LoadScene(nomeCenaJogo);
        }
    }

    /// <summary>
    /// Exibe o painel de instruções/história. Vincular ao OnClick do Botão Instruções.
    /// </summary>
    public void AbrirInstrucoes()
    {
        if (painelInstrucoes != null)
            painelInstrucoes.SetActive(true);
    }

    /// <summary>
    /// Fecha o painel de instruções.
    /// </summary>
    public void FecharInstrucoes()
    {
        if (painelInstrucoes != null)
            painelInstrucoes.SetActive(false);
    }

    /// <summary>
    /// Encerra a aplicação (funciona em Builds Standalone e no Editor).
    /// </summary>
    public void SairDoJogo()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}