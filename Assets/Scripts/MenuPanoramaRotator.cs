using UnityEngine;

public class MenuPanoramaRotator : MonoBehaviour
{
    [Header("Configuração do Panorama")]
    [Tooltip("Material Skybox Panoramic atribuído à cena do menu")]
    public Material menuSkyboxMaterial;
    [Tooltip("Textura 2D equirretangular para o fundo do menu")]
    public Texture2D fotoMenuBackground;

    [Header("Velocidade de Rotação")]
    [Tooltip("Velocidade do giro contínuo do fundo (em graus por segundo)")]
    public float velocidadeRotacao = 5.0f;

    private float anguloAtual = 0f;

    void Start()
    {
        // Aplica a foto escolhida no material do Skybox
        if (menuSkyboxMaterial != null && fotoMenuBackground != null)
        {
            menuSkyboxMaterial.SetTexture("_MainTex", fotoMenuBackground);
            RenderSettings.skybox = menuSkyboxMaterial;
            DynamicGI.UpdateEnvironment();
        }
    }

    void Update()
    {
        // Rotaciona o panorama suavemente no eixo Y
        anguloAtual += velocidadeRotacao * Time.deltaTime;

        // Garante que o ângulo fique entre 0 e 360 graus
        if (anguloAtual >= 360f) anguloAtual -= 360f;

        // Atualiza a rotação no Shader Panoramic
        if (menuSkyboxMaterial != null)
        {
            menuSkyboxMaterial.SetFloat("_Rotation", anguloAtual);
        }
    }
}