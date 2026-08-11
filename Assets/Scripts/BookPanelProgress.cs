using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BookPanelProgress : MonoBehaviour
{
    public static BookPanelProgress Instance;

    [Header("Componentes do Painel")]
    public CanvasGroup panelCanvasGroup;
    public RectTransform panelRectTransform;
    public Button closeButton;

    [Header("Painel do Diário Completo (Filho)")]
    public GameObject completedBookPanel;

    [Header("Slots do Diário")]
    [Tooltip("Arraste as imagens do Canvas que representam a vaga/posição de cada um dos 5 fragmentos")]
    public Image[] slotImages; // Array de 5 Imagens na UI (Slots 0 a 4)

    [Header("Texturas/Sprites dos Fragmentos")]
    [Tooltip("Arraste as artes específicas de cada fragmento na ordem dos índices (0 a 4)")]
    public Sprite[] fragmentSprites; // As 5 artes únicas dos fragmentos

    [Header("Configurações da Animação de Queda")]
    public float escalaInicial = 2.8f;
    public float duracaoAnimacao = 0.45f;
    public float anguloRotacaoInicial = -12f;

    public AnimationCurve curvaQuedaImpacto = new AnimationCurve(
        new Keyframe(0f, 0f),
        new Keyframe(0.75f, 1.08f),
        new Keyframe(1f, 1f)
    );

    [Header("Configurações da Fusão Exclusiva (5º Fragmento)")]
    public float tempoEsperaParaFusao = 0.5f;
    public float duracaoCentralizacao = 0.6f;
    public float duracaoBrilhoFusao = 0.8f;
    public Color corBrilhoFusao = new Color(1f, 0.92f, 0.5f, 1f); // Tom dourado/neon brilhante

    private Vector2 posicaoOriginal;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (panelCanvasGroup == null) panelCanvasGroup = GetComponent<CanvasGroup>();
        if (panelRectTransform == null) panelRectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        if (panelRectTransform != null)
        {
            posicaoOriginal = panelRectTransform.anchoredPosition;
        }

        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha = 0f;
            panelCanvasGroup.blocksRaycasts = false;
        }

        if (completedBookPanel != null)
        {
            completedBookPanel.SetActive(false);
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(FecharPainel);
        }

        InicializarSlotsSalvos();
    }

    private void InicializarSlotsSalvos()
    {
        int totalColetados = 0;

        for (int i = 0; i < slotImages.Length; i++)
        {
            if (slotImages[i] != null)
            {
                bool jaColetado = PlayerPrefs.GetInt($"Fragmento_{i}", 0) == 1;

                if (jaColetado)
                {
                    totalColetados++;
                    if (i < fragmentSprites.Length && fragmentSprites[i] != null)
                    {
                        slotImages[i].sprite = fragmentSprites[i];
                        slotImages[i].gameObject.SetActive(true);
                        slotImages[i].rectTransform.localScale = Vector3.one;
                        slotImages[i].rectTransform.localRotation = Quaternion.identity;
                    }
                }
                else
                {
                    slotImages[i].gameObject.SetActive(false);
                }
            }
        }

        // Se já coletou todos previamente, habilita direto o diário completo e oculta o botão da coleção
        if (totalColetados >= slotImages.Length && completedBookPanel != null)
        {
            OcultarSlotsFragmentos();
            if (closeButton != null) closeButton.gameObject.SetActive(false);
            RevelarDiarioCompletoCentralizado();
        }
    }

    public void ExibirEAnimarFragmento(int fragmentIndex, bool ehOUltimoFragmento)
    {
        if (fragmentIndex < 0 || fragmentIndex >= slotImages.Length) return;

        gameObject.SetActive(true);

        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha = 1f;
            panelCanvasGroup.blocksRaycasts = true;
        }

        StartCoroutine(RotinaAnimacaoECompletar(fragmentIndex, ehOUltimoFragmento));
    }

    private IEnumerator RotinaAnimacaoECompletar(int index, bool ehOUltimo)
    {
        // 1. Executa a animação de queda do fragmento no slot
        yield return StartCoroutine(RotinaAnimacaoImpacto(index));

        // 2. Se for o último fragmento (5/5), dispara o efeito mágico de fusão!
        if (ehOUltimo)
        {
            yield return new WaitForSeconds(tempoEsperaParaFusao);
            yield return StartCoroutine(RotinaFusaoETransformacao());
        }
    }

    private IEnumerator RotinaAnimacaoImpacto(int index)
    {
        Image targetSlot = slotImages[index];

        if (index < fragmentSprites.Length && fragmentSprites[index] != null)
        {
            targetSlot.sprite = fragmentSprites[index];
        }

        targetSlot.gameObject.SetActive(true);

        RectTransform rect = targetSlot.rectTransform;
        Vector3 escalaInicialVec = Vector3.one * escalaInicial;
        Vector3 escalaFinalVec = Vector3.one;

        float tempo = 0f;

        while (tempo < duracaoAnimacao)
        {
            tempo += Time.deltaTime;
            float t = tempo / duracaoAnimacao;
            float fatorMola = curvaQuedaImpacto.Evaluate(t);

            rect.localScale = Vector3.LerpUnclamped(escalaInicialVec, escalaFinalVec, fatorMola);

            float anguloAtual = Mathf.Lerp(anguloRotacaoInicial, 0f, t);
            rect.localRotation = Quaternion.Euler(0f, 0f, anguloAtual);

            yield return null;
        }

        rect.localScale = escalaFinalVec;
        rect.localRotation = Quaternion.identity;
    }

    /// <summary>
    /// Move o painel para o centro da tela, OCULTA PERMANENTEMENTE o botão da coleção e brilha apenas as imagens.
    /// </summary>
    private IEnumerator RotinaFusaoETransformacao()
    {
        // Desativa permanentemente o botão de fechar da coleção assim que a fusão inicia
        if (closeButton != null)
        {
            closeButton.gameObject.SetActive(false);
        }

        // A. Centraliza e alinha o painel pai
        panelRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        panelRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        panelRectTransform.pivot = new Vector2(0.5f, 0.5f);

        Vector2 posInicial = panelRectTransform.anchoredPosition;
        Vector2 posCentro = Vector2.zero;
        float tempoMover = 0f;

        while (tempoMover < duracaoCentralizacao)
        {
            tempoMover += Time.deltaTime;
            float t = tempoMover / duracaoCentralizacao;
            panelRectTransform.anchoredPosition = Vector2.Lerp(posInicial, posCentro, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
        panelRectTransform.anchoredPosition = posCentro;

        // B. Efeito de Brilho EXCLUSIVO nas peças de fragmentos (slotImages)
        float tempoBrilho = 0f;
        Color[] coresOriginais = new Color[slotImages.Length];

        for (int i = 0; i < slotImages.Length; i++)
        {
            if (slotImages[i] != null) coresOriginais[i] = slotImages[i].color;
        }

        while (tempoBrilho < duracaoBrilhoFusao)
        {
            tempoBrilho += Time.deltaTime;
            float t = tempoBrilho / duracaoBrilhoFusao;

            float fatorPulso = Mathf.Sin(t * Mathf.PI);

            for (int i = 0; i < slotImages.Length; i++)
            {
                if (slotImages[i] != null)
                {
                    slotImages[i].color = Color.Lerp(coresOriginais[i], corBrilhoFusao, fatorPulso);
                    float escalaSlot = 1f + (fatorPulso * 0.18f);
                    slotImages[i].rectTransform.localScale = Vector3.one * escalaSlot;
                }
            }

            yield return null;
        }

        // Restaura as cores e escalas originais dos slots
        for (int i = 0; i < slotImages.Length; i++)
        {
            if (slotImages[i] != null)
            {
                slotImages[i].color = coresOriginais[i];
                slotImages[i].rectTransform.localScale = Vector3.one;
            }
        }

        panelRectTransform.localScale = Vector3.one;

        // C. Oculta as peças e ativa o CompletedBookPanel no centro exato
        OcultarSlotsFragmentos();
        RevelarDiarioCompletoCentralizado();
    }

    private void RevelarDiarioCompletoCentralizado()
    {
        if (completedBookPanel != null)
        {
            completedBookPanel.SetActive(true);

            RectTransform childRect = completedBookPanel.GetComponent<RectTransform>();
            if (childRect != null)
            {
                childRect.anchorMin = new Vector2(0.5f, 0.5f);
                childRect.anchorMax = new Vector2(0.5f, 0.5f);
                childRect.pivot = new Vector2(0.5f, 0.5f);
                childRect.anchoredPosition = Vector2.zero;
                childRect.localScale = Vector3.one;
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.TocarDiscover();
            }
        }
    }

    private void OcultarSlotsFragmentos()
    {
        for (int i = 0; i < slotImages.Length; i++)
        {
            if (slotImages[i] != null)
            {
                slotImages[i].gameObject.SetActive(false);
            }
        }
    }

    public void FecharPainel()
    {
        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha = 0f;
            panelCanvasGroup.blocksRaycasts = false;
        }

        if (panelRectTransform != null)
        {
            panelRectTransform.anchoredPosition = posicaoOriginal;
        }
    }
}