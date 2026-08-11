using UnityEngine;
using UnityEngine.UI;

public class MinimapManager : MonoBehaviour
{
    public static MinimapManager Instance;

    [Header("Componentes do Minimapa")]
    public CanvasGroup minimapCanvasGroup;
    public RectTransform playerPin;       // Ícone do pino do jogador
    public RectTransform visionCone;      // Cone de visão/orientação

    [Header("Caminho/Pontos no Minimapa")]
    [Tooltip("Arraste os waypoints correspondentes na ordem dos panoramas (0 a N)")]
    public RectTransform[] mapWaypoints;

    [Header("Configuração de Movimento Suave")]
    public float velocidadeDeslizamento = 8.0f;
    private Vector2 posicaoAlvo;

    [Header("Ajuste/Balanceamento da Câmera & Visão")]
    public Transform cameraTransform;

    [Tooltip("Ajuste em graus para alinhar o cone com o mapa (Ex: 90, -90, 180). Use para corrigir o desvio de orientação!")]
    public float offsetAnguloCone = 0f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (minimapCanvasGroup == null) minimapCanvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        // Posiciona o pino no primeiro waypoint ao iniciar
        if (playerPin != null && mapWaypoints != null && mapWaypoints.Length > 0 && mapWaypoints[0] != null)
        {
            posicaoAlvo = mapWaypoints[0].anchoredPosition;
            playerPin.anchoredPosition = posicaoAlvo;
        }
    }

    void Update()
    {
        // 1. Desliza suavemente o pino até o waypoint atual
        if (playerPin != null)
        {
            playerPin.anchoredPosition = Vector2.Lerp(
                playerPin.anchoredPosition,
                posicaoAlvo,
                Time.deltaTime * velocidadeDeslizamento
            );
        }

        // 2. Atualiza a rotação do cone de visão aplicando o Offset de Correção
        if (visionCone != null && cameraTransform != null)
        {
            float anguloY = cameraTransform.eulerAngles.y;

            // Aplica a inversão para UI (-anguloY) somada ao Offset de ajuste
            float anguloCorrigido = -anguloY + offsetAnguloCone;

            visionCone.localRotation = Quaternion.Euler(0f, 0f, anguloCorrigido);
        }
    }

    /// <summary>
    /// Atualiza a posição de destino do pino do minimapa
    /// </summary>
    public void AtualizarPosicaoMinimapa(int indicePanorama)
    {
        if (mapWaypoints == null || mapWaypoints.Length == 0) return;

        if (indicePanorama >= 0 && indicePanorama < mapWaypoints.Length)
        {
            RectTransform targetWaypoint = mapWaypoints[indicePanorama];

            if (targetWaypoint != null)
            {
                posicaoAlvo = targetWaypoint.anchoredPosition;
            }
        }
    }

    public void AlternarVisibilidade(bool visivel)
    {
        if (minimapCanvasGroup != null)
        {
            minimapCanvasGroup.alpha = visivel ? 1f : 0f;
            minimapCanvasGroup.blocksRaycasts = visivel;
        }
    }
}