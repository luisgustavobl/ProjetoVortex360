# Os Caçadores da Moema Perdida — Protótipo Vortex360

> **Versão online (WebGL):** [Link no itch.io](https://luisgustavobl.itch.io/projeto-vortex360)

Projeto desenvolvido como protótipo de navegação panorâmica interativa em 360° no ecossistema **Unity 6 (URP)** para o Desafio Técnico do Laboratório Vortex (UNIFOR).

---

## Narrativa

Durante uma expedição nas redondezas de uma histórica capela/castelo nas montanhas, a aventureira Moema desapareceu. O jogador assume o papel de um caçador de pistas navegando pela área externa, investigando panoramas para reunir os **5 fragmentos do diário** de Moema. Somente com o diário reconstituído é possível desbloquear a entrada do castelo, encontrar a Moema e resgatá-la.

---

## Funcionalidades Implementadas

- [X] **Visualização Panorâmica 360°:** Projeção esférica equirretangular via Shader URP e Cubemaps.
- [X] **Navegação Multimodal:** Alternância de panoramas via teclado (WASD / Setas) e botões UI na tela (`StepUpButton` e `StepBackButton`).
- [X] **Controle de Câmera:** Rotação livre (Pan via mouse) com limite vertical e aproximação/afastamento (Zoom no Scroll com FOV `30-70`).
- [X] **Limite de Mapa (Feedback Visual):** Alerta de final de percurso com efeito *flash* vermelho e *fade out* suave controlável via Inspector.
- [X] **Estrutura Múltipla de Cenas:** Transição fluida entre `MenuScene`, `SampleScene` (Área Externa) e `CastleInteriorScene` (Interior do Castelo).
- [X] **Menu Inicial Interativo:** Fundo panorâmico 360° em rotação contínua no estilo *Minecraft* (`MenuPanoramaRotator.cs`).
- [X] **Gatilhos Condicionais por Ângulo da Câmera:**
  - O botão de entrada no castelo (`EnterCastleButton`) só é exibido no panorama 15 quando o jogador aponta a câmera diretamente na direção da porta.
  - O botão de saída (`ExitCastleButton`) e o botão de acorda (`WakeUpButton`) no interior só aparecem nos ângulos específicos das estruturas/personagem.
- [X] **Mecânica de Gamificação & Diário de Bordo:**
  - Coleta de 5 fragmentos escondidos nos panoramas via clique 3D (`Raycast` e `Colliders`).
  - Contador de progresso condicional na HUD (`3/5`), ativado somente após a primeira coleta.
  - Trava de segurança no castelo: tentar entrar sem o diário completo exibe o aviso temporário *"Você não pode entrar aqui ainda!"*.
  - Revelação do diário restaurado em pergaminho ao coletar o 5º fragmento com botão de fechar (`CloseBookButton`).
- [X] **Mecânica da Moema (Interior do Castelo):**
  - Troca de sprite/estado com delay customizável (`EmaSleeping` $\rightarrow$ `EmaAwake`).
  - Sombra projetada no bloco (`Blob/Drop Shadow`) para garantir ancoragem visual do sprite no cenário 3D.
- [X] **Persistência de Progresso (`PlayerPrefs`):**
  - Salvamento do último panorama visitado (retorna ao panorama 15 ao sair do castelo em vez de reiniciar no 0).
  - Fragmentos coletados salvos permanentemente (evita contagem duplicada e impede perda do progresso ao mudar de cena).
  - Botão "JOGAR" no Menu Principal que reseta limpo os dados do `PlayerPrefs` para um novo jogo do zero.
- [X] **Tela de Vitória & Loop de Fim de Jogo (`GameWonPanel`):**
  - Painel de encerramento ativado com delay após a Moema acordar.
  - Botão `ReturnToMenuButton` para voltar ao Menu Principal.
  - Botão `ReturnToGameButton` para explorar livremente a sala, ativando um botão HUD fixo (`InGameReturnToMenuButton`).

---

## Próximas Etapas / Polimento

- [ ] **Integração dos Áudios e Efeitos Sonoros (SFX):**
  - Som de clique genérico para botões da UI (`OnClick`).
  - Som de papel/página ao coletar fragmento do diário.
  - Som de alerta/erro ao tentar entrar no castelo bloqueado.
  - Efeito sonoro de bocejo/despertar ao acionar o `WakeUpButton` para a Moema (`AudioSource.PlayOneShot`).
  - Trilha sonora suave/ambiente para o interior do castelo.
- [ ] **Polimento Visual & Arte de UI:**
  - Substituição dos botões padrão da Unity por sprites/ícones customizados estilizados.
  - Ajuste de animações suaves de transição (Fade In / Fade Out via CanvasGroup) na troca de cenas.

---

## AI Logbook

Este projeto utilizou Inteligência Artificial Generativa (**Gemini**) como parceira de desenvolvimento (*Pair Programming*) para arquitetura de código C#, refatoração de matemática vetorial/angular, persistência de dados e otimização de exportação WebGL.

### Histórico de Desenvolvimento e Desafios

#### **Etapa Initial: Prototipação e Pipeline**

- **Fase 1: Rotação 360° e Input System:** Implementação da câmera rotacional (`CameraRotator.cs`). Ajuste nas preferências de entrada para URP e limites de inclinação vertical.
- **Fase 2: Gerenciamento da Rota:** Script `StreetViewManager.cs` para percorrer a lista de Cubemaps/Panoramas via teclado e UI.
- **Fase 3: Feedback de Limite de Mapa:** Implementação da Coroutine de *flash* vermelho e *fade out* suave para limites da rota.
- **Fase 4: Otimização WebGL:** Redução da build de ~300 MB para ~70 MB substituindo Cubemaps pesados por marcação panorâmica equirretangular `Skybox/Panoramic` com compressão para Web.

#### **Etapa 1: Arquitetura Multicenas & Posição Angular**

- **Gatilho de Visão por Bússola:** Desenvolvimento da normalização angular (`NormalizarAngulo` / `ChecarAnguloNoIntervalo`) para converter rotações negativas do Unity (ex: $-70^\circ$) e exibir botões condicionais apenas quando o jogador encara o objetivo.
- **Fase de Interior (`CastleInteriorScene`):** Criação da cena dedicada para o interior do castelo com suporte a ângulo de entrada customizado no `CameraRotator`.

#### **Etapa 2: Gamificação, Resgate da Moema e Persistência**

- **Sistema de Coleta e Diário (`JournalManager.cs` / `JournalFragment.cs`):** Criação do fluxo de coleta dos 5 fragmentos, contador na HUD e trava no castelo.
- **Persistência de Dados entre Cenas:** Resolução do bug de reset de progresso. Implementação de chaves únicas no `PlayerPrefs` (`Fragmento_X` e `UltimoPanoramaIndex`) permitindo entrar/sair do castelo sem perder os itens coletados.
- **Animação de Despertar e Tela de Vitória:** Implementação das Coroutines no `CastleInteriorManager.cs` com suporte a delays customizáveis, ativação de sprites e fluxo de botões para explorar a sala ou retornar ao menu.

---

## Tecnologias Utilizadas

- **Engine:** Unity 6 (URP - Universal Render Pipeline)
- **Linguagem:** C#
- **Hospedagem WebGL:** itch.io
- **Assistente IA:** Gemini (Pair Programming e Resolução de Bugs)
