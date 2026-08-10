using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Componente de Áudio")]
    public AudioSource audioSource;

    [Header("Efeitos Sonoros de UI")]
    public AudioClip somCliqueBotao;
    public AudioClip somErroBloqueio;
    public AudioClip somCliqueLongo;
    public AudioClip somCliqueDiscover;

    [Header("Efeitos Sonoros Ema")]
    public AudioClip somCliqueB;

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
            audioSource.PlayOneShot(somCliqueBotao);
        }
    }

    public void TocarErroBloqueio()
    {
        if (audioSource != null && somErroBloqueio != null)
        {
            audioSource.PlayOneShot(somErroBloqueio);
        }
    }

    public void TocarCliqueBotaoLongo()
    {
        if (audioSource != null && somCliqueLongo != null)
        {
            audioSource.PlayOneShot(somCliqueLongo);
        }
    }
    public void TocarCliqueBotaoDiscover()
    {
        if (audioSource != null && somCliqueDiscover != null)
        {
            audioSource.PlayOneShot(somCliqueDiscover);
        }
    }
}