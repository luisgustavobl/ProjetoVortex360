# Navegador Panorâmico 360° — Protótipo WebGL (Vortex)

>  **Teste a versão online (WebGL):** [Link itch.io](https://luisgustavobl.itch.io/projeto-vortex360)

Projeto desenvolvido como protótipo de navegação panorâmica interativa em 360° utilizando imagens equirretangulares e shaders no ecossistema Unity 6 (URP).

---

## Funcionalidades Implementadas

- [X] **Visualização Panorâmica 360°:** Projeção esférica via Shader `Skybox/Panoramic`.
- [X] **Navegação Multimodal:** Alternância de panoramas via teclado (WASD / Setas) e botões UI na tela.
- [X] **Controle de Câmera:** Rotação livre (Pan via mouse) e aproximação/afastamento (Zoom no Scroll).
- [X] **Limite de Mapa (Feedback Visual):** Alerta de final de percurso com efeito *flash* vermelho e *fade out* suave controlável via Inspector.

---

## AI Logbook

Este projeto utilizou IA Generativa como parceira de desenvolvimento (*Pair Programming*) durante todo o processo de arquitetura, codificação em C# e resolução de gargalos de exportação.

### Histórico de Desenvolvimento e Desafios

#### **Fase 1: Rotação 360° e Input System**

- **Ação:** Implementação da câmera rotacional (`CameraRotator.cs`).
- **Desafio:** Erros de leitura de input do mouse no pipeline URP.
- **Solução com IA:** Ajuste nas preferências de entrada da Unity (*Active Input Handling*) para dar suporte ao código de rotação.

#### **Fase 2: Gerenciamento da Rota e UI de Navegação**

- **Ação:** Criação do script `StreetViewManager.cs` para percorrer a lista de panoramas.
- **Desafio:** Fotos desordenadas/duplicadas ao vincular os botões 2D aos eventos `OnClick`.
- **Solução com IA:** Curadoria da lista de imagens e refatoração dos métodos de avanço e recuo.

#### **Fase 3: Barreira de Limite de Mapa e Efeitos de Canvas**

- **Ação:** Criação de feedback de "barreira de mapa" quando o usuário atinge o início ou fim da rota.
- **Desafio:** A imagem do `FlashOverlay` não aparecia na tela durante o acionamento da Coroutine.
- **Solução com IA:** Ajuste na ordem dos elementos no Canvas, troca do sprite para `UISprite` e correção do Alpha no canal de cor do componente `Image`.

#### **Fase 4: Otimização do Pipeline para WebGL**

- **Ação:** Exportação da build para a plataforma Web.
- **Desafio:** A Build WebGL inicial gerou um arquivo `.data` de quase 300 MB por conta da conversão dos JPGs em Cubemaps (6 faces por imagem).
- **Solução com IA:** Substituição do Shader para `Skybox/Panoramic` (2D Equirretangular), aplicação do *Override* para WebGL e ajuste da compressão, reduzindo o arquivo para ~70 MB e viabilizando a hospedagem no itch.io.

---

## Tecnologias Utilizadas

- **Engine:** Unity 6 (URP - Universal Render Pipeline)
- **Linguagem:** C#
- **Hospedagem WebGL:** itch.io
- **Assistente IA:** Gemini (Pair Programming e Resolução de BUGS)
