using UnityEngine;

public class JournalFragment : MonoBehaviour
{
    [Header("Configuração do Fragmento")]
    [Tooltip("Índice do panorama (0 a 19/20) onde este fragmento deve aparecer")]
    public int panoramaIndex = 0;

    [Header("Efeito Sonoro (Opcional)")]
    public AudioSource audioSource;
    public AudioClip collectSound;

    private void OnMouseDown()
    {
        // Executa a coleta via JournalManager
        if (JournalManager.Instance != null)
        {
            JournalManager.Instance.ColetarFragmento();
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