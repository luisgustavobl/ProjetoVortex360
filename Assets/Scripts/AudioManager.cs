using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Componente de Áudio")]
    public AudioSource audioSource;

    [Header("Efeitos Sonoros de UI")]
    public AudioClip somCliqueBotao;

    [Range(0f, 1f)]
    [Tooltip("Volume exclusivo para cliques.")]
    public float volumeCliques = 0.5f;

    [Range(0f, 2f)]
    public AudioClip somErroBloqueio;
    [Tooltip("Volume exclusivo para erros.")]
    public float volumeErroBloqueio = 0.5f;


    [Header("Efeitos Sonoros de Movimentação (Passos/Seta)")]
    [Tooltip("Lista de áudios sorteados aleatoriamente ao clicar nas setas de avançar/voltar")]
    public AudioClip[] sonsPassoPanorama;

    [Range(0f, 1f)]
    [Tooltip("Volume exclusivo dos passos (0 = mudo, 1 = volume máximo)")]
    public float volumePassos = 0.5f; // Valor padrão em 50% do volume


    [Header("Outros efeitos:")]
    [Tooltip("Placeholder")]
    public AudioClip somCliquePH;
    [Range(0f, 1f)]
    [Tooltip("Volume exclusivo para placeholder.")]
    public float volumeCliquePH = 0.5f;

    public AudioClip somCliqueSpecial;
    [Range(0f, 2f)]
    [Tooltip("Volume exclusivo para cliques especiais.")]
    public float volumeCliqueSpecial = 0.5f;

    public AudioClip somDiscover;
    [Range(0f, 2f)]
    [Tooltip("Volume exclusivo para descobertas.")]
    public float volumeDiscover = 0.5f;

    void Awake()
    {
        // Define esta instância como a ativa da cena
        Instance = this;

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void TocarCliqueBotao()
    {
        if (audioSource != null && somCliqueBotao != null)
        {
            audioSource.PlayOneShot(somCliqueBotao, volumeCliques);
        }
    }

    public void TocarErroBloqueio()
    {
        if (audioSource != null && somErroBloqueio != null)
        {
            audioSource.PlayOneShot(somErroBloqueio, volumeErroBloqueio);

        }
    }

    /// <summary>
    /// Sorteia e toca um áudio aleatório da lista de movimentação aplicando a escala de volume customizada
    /// </summary>
    public void TocarSomPassoPanorama()
    {
        if (audioSource != null && sonsPassoPanorama != null && sonsPassoPanorama.Length > 0)
        {
            int indiceSorteado = Random.Range(0, sonsPassoPanorama.Length);
            AudioClip somSorteado = sonsPassoPanorama[indiceSorteado];

            if (somSorteado != null)
            {
                // O segundo parâmetro do PlayOneShot define o volume relativo (de 0.0 a 1.0)
                audioSource.PlayOneShot(somSorteado, volumePassos);
            }
        }
        else
        {
            // Fallback
            //TocarCliqueBotao();
        }
    }

    public void TocarCliqueB()
    {
        if (audioSource != null && somCliquePH!= null)
        {
            audioSource.PlayOneShot(somCliquePH, volumeCliquePH);
        }
    }
    public void TocarCliqueBotaoSpecial()
    {
        if (audioSource != null && somCliqueSpecial != null)
        {
            audioSource.PlayOneShot(somCliqueSpecial, volumeCliqueSpecial);
        }
    }
    public void TocarDiscover()
    {
        if (audioSource != null && somDiscover != null)
        {
            audioSource.PlayOneShot(somDiscover, volumeDiscover);
        }
    }
}