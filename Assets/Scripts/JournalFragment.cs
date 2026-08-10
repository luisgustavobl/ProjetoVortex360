using UnityEngine;

public class JournalFragment : MonoBehaviour
{
    [Header("Configuração do Fragmento")]
    [Tooltip("Índice único do fragmento para salvar no PlayerPrefs (0, 1, 2, 3, 4)")]
    public int fragmentIndex = 0;

    [Tooltip("Índice do panorama (0 a 19/20) onde este fragmento deve aparecer")]
    public int panoramaIndex = 0;

    [Header("Efeito Sonoro (Opcional)")]
    public AudioSource audioSource;
    public AudioClip collectSound;

    private void Start()
    {
        // Se o fragmento já foi coletado anteriormente, oculta o objeto
        if (JournalManager.Instance != null && JournalManager.Instance.FragmentoJaColetado(fragmentIndex))
        {
            gameObject.SetActive(false);
        }
    }

    private void OnMouseDown()
    {
        // Executa a coleta via JournalManager passando o índice único
        if (JournalManager.Instance != null)
        {
            JournalManager.Instance.ColetarFragmento(fragmentIndex);
        }

        // Toca som de coleta se configurado
        if (audioSource != null && collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, Camera.main.transform.position);
        }

        // Esconde o fragmento coletado
        gameObject.SetActive(false);
    }
}