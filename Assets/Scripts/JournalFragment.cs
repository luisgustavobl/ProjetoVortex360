using UnityEngine;

public class JournalFragment : MonoBehaviour
{
    [Header("Configuração do Fragmento")]
    [Tooltip("Índice único do fragmento para salvar no PlayerPrefs (0, 1, 2, 3, 4)")]
    public int fragmentIndex = 0;

    [Tooltip("Índice do panorama (0 a 19/20) onde este fragmento deve aparecer")]
    public int panoramaIndex = 0;

    private void Start()
    {
        // Se já foi coletado previamente, desativa o objeto imediatamente
        if (JournalManager.Instance != null && JournalManager.Instance.FragmentoJaColetado(fragmentIndex))
        {
            gameObject.SetActive(false);
            return;
        }

        // Caso o StreetViewManager já esteja pronto, valida se deve aparecer no panorama atual
        if (StreetViewManager.Instance != null)
        {
            bool ehOPanoramaCorreto = (panoramaIndex == StreetViewManager.Instance.ObterIndiceAtual());
            gameObject.SetActive(ehOPanoramaCorreto);
        }
    }

    private void OnMouseDown()
    {
        if (JournalManager.Instance != null)
        {
            JournalManager.Instance.ColetarFragmento(fragmentIndex);
        }

        gameObject.SetActive(false);
    }
}