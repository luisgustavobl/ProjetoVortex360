using System.Collections;
using UnityEngine;

public class StreetViewManager : MonoBehaviour
{
    [Header("Configuração de Fotos")]
    public Material skyboxMaterial;
    public Cubemap[] listaDeFotos;
    private int indiceAtual = 0;

    [Header("Efeitos de Limite de Mapa")]
    public CanvasGroup flashOverlay;   // Arraste o FlashOverlay aqui
    public CanvasGroup warningCanvasGroup; // Arraste o WarningText aqui (precisa ter CanvasGroup)

    [Header("Tempos de Animação")]
    [Tooltip("Opacidade máxima do flash vermelho (0 a 1)")]
    public float opacidadeFlash = 0.5f;
    [Tooltip("Tempo visível do texto de alerta antes de iniciar o Fade Out")]
    public float tempoExibicaoTexto = 1.5f;
    [Tooltip("Duração do Fade Out do texto e da tela piscando em segundos")]
    public float duracaoFadeOut = 1.0f;

    private Coroutine coroutineAlerta;

    void Start()
    {
        AtualizarFoto();

        // Esconde os elementos de interface ao iniciar
        if (flashOverlay != null) flashOverlay.alpha = 0;
        if (warningCanvasGroup != null) warningCanvasGroup.alpha = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            ProximaFoto();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            FotoAnterior();
        }
    }

    public void ProximaFoto()
    {
        if (listaDeFotos.Length == 0) return;

        if (indiceAtual >= listaDeFotos.Length - 1)
        {
            DispararAlertaLimite();
            return;
        }

        indiceAtual++;
        AtualizarFoto();
    }

    public void FotoAnterior()
    {
        if (listaDeFotos.Length == 0) return;

        if (indiceAtual <= 0)
        {
            DispararAlertaLimite();
            return;
        }

        indiceAtual--;
        AtualizarFoto();
    }

    private void DispararAlertaLimite()
    {
        if (coroutineAlerta != null) StopCoroutine(coroutineAlerta);
        coroutineAlerta = StartCoroutine(EfeitoAlertaBarreira());
    }

    private IEnumerator EfeitoAlertaBarreira()
    {
        // 1. Aparição Instantânea (Flash e Texto)
        if (flashOverlay != null) flashOverlay.alpha = opacidadeFlash;
        if (warningCanvasGroup != null) warningCanvasGroup.alpha = 1.0f;

        // 2. Aguarda o tempo do texto na tela
        yield return new WaitForSeconds(tempoExibicaoTexto);

        // 3. Fade Out Suave do Flash e do Texto ao mesmo tempo
        float tempo = 0;
        float alphaInicialFlash = flashOverlay != null ? flashOverlay.alpha : 0;
        float alphaInicialTexto = warningCanvasGroup != null ? warningCanvasGroup.alpha : 0;

        while (tempo < duracaoFadeOut)
        {
            tempo += Time.deltaTime;
            float fatorLinear = tempo / duracaoFadeOut;

            if (flashOverlay != null)
                flashOverlay.alpha = Mathf.Lerp(alphaInicialFlash, 0, fatorLinear);

            if (warningCanvasGroup != null)
                warningCanvasGroup.alpha = Mathf.Lerp(alphaInicialTexto, 0, fatorLinear);

            yield return null; // Espera o próximo frame
        }

        // Garante que ambos fiquem zerados no final
        if (flashOverlay != null) flashOverlay.alpha = 0;
        if (warningCanvasGroup != null) warningCanvasGroup.alpha = 0;
    }

    private void AtualizarFoto()
    {
        if (skyboxMaterial != null && listaDeFotos.Length > 0 && listaDeFotos[indiceAtual] != null)
        {
            skyboxMaterial.SetTexture("_Tex", listaDeFotos[indiceAtual]);
            DynamicGI.UpdateEnvironment();
        }
    }
}