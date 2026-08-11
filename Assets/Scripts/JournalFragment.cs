using System.Collections;
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
        // Se já foi coletado previamente, esconde imediatamente
        if (JournalManager.Instance != null && JournalManager.Instance.FragmentoJaColetado(fragmentIndex))
        {
            gameObject.SetActive(false);
            return;
        }

        // Aguarda 1 frame para ter certeza de que o StreetViewManager definiu o panorama correto
        StartCoroutine(ValidarVisibilidadeInicial());
    }

    private IEnumerator ValidarVisibilidadeInicial()
    {
        yield return null; // Espera o próximo frame

        if (JournalManager.Instance != null && JournalManager.Instance.FragmentoJaColetado(fragmentIndex))
        {
            gameObject.SetActive(false);
            yield break;
        }

        if (StreetViewManager.Instance != null)
        {
            bool ehOPanoramaCorreto = (panoramaIndex == StreetViewManager.Instance.ObterIndiceAtual());
            gameObject.SetActive(ehOPanoramaCorreto);
        }
        else
        {
            // Caso por algum motivo não haja StreetViewManager, mantém ativo para não sumir
            gameObject.SetActive(true);
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