# Os Caçadores da Moema Perdida — Protótipo Vortex360

> **Versão online (WebGL):** [Link no itch.io](https://luisgustavobl.itch.io/projeto-vortex360)

Projeto desenvolvido como protótipo de navegação panorâmica interativa em 360° no ecossistema **Unity 6 (URP)** para o Desafio Técnico do Laboratório Vortex (UNIFOR).

---

## Narrativa

Durante uma expedição nas redondezas de uma histórica capela/castelo nas montanhas, a aventureira Moema desapareceu. O jogador assume o papel de um caçador de pistas navegando pela área externa, investigando panoramas para reunir os **5 fragmentos do diário** de Moema. Somente com o diário reconstituído é possível desbloquear a entrada do castelo, encontrar a Moema e resgatá-la.

---

## Funcionalidades Implementadas

- [X] **Visualização Panorâmica 360°:** Projeção esférica equirretangular via Shader URP e Cubemaps.
- [X] **Navegação Dinâmica Invertida por Orientação:**
  - Botões UI permanentes (`StepUpButton` e `StepBackButton`).
  - Mapeamento angular inteligente: quando a câmera aponta para a frente (entre 270° e 90° na bússola), a seta de cima avança e a de baixo recua. Ao girar para trás (entre 90° e 270°), os comandos se invertem para acompanhar o campo de visão do jogador.
  - Suporte secundário a teclas de atalho (WASD / Setas).
- [X] **Controle de Câmera:** Rotação livre (Pan via mouse) com limite vertical e aproximação/afastamento (Zoom no Scroll com FOV `30-70`).
- [X] **Panorama Inicial Customizável:** Configuração de índice padrão (`indiceInicialDefault`) para início em panorama específico sem perder o ponto de retorno ao sair do castelo.
- [X] **Limite de Mapa (Feedback Visual):** Alerta de final de percurso com efeito *flash* vermelho e *fade out* suave controlável via Inspector.
- [X] **Estrutura Múltipla de Cenas:** Transição fluida entre `MenuScene`, `SampleScene` (Área Externa) e `CastleInteriorScene` (Interior do Castelo).
- [X] **Menu Inicial Interativo:** Fundo panorâmico 360° em rotação contínua (`MenuPanoramaRotator.cs`).
- [X] **Gatilhos Condicionais por Ângulo da Câmera:**
  - O botão de entrada no castelo (`EnterCastleButton`) só é exibido no panorama 15 quando o jogador aponta a câmera diretamente na direção da porta.
  - O botão de saída e o botão de ação no interior só aparecem nos ângulos específicos das estruturas/personagem.
- [X] **Mecânica de Gamificação & Diário de Bordo:**
  - Coleta de 5 fragmentos escondidos nos panoramas via clique 3D (`Raycast` e `Colliders`).
  - Contador de progresso condicional na HUD (`3/5`), ativado somente após a primeira coleta.
  - Trava de segurança no castelo: tentar entrar sem o diário completo exibe o aviso temporário *"Você não pode entrar aqui ainda!"*.
  - Revelação do diário restaurado em pergaminho ao coletar o 5º fragmento com botão de fechar (`CloseBookButton`).
- [X] **Sistema de Áudio Centralizado & Dynamic Pitch:**
  - Gerenciamento de áudio via `AudioManager.cs` e `JournalManager.cs`.
  - Som de clique unificado em botões de interface.
  - Som de alerta/erro ao tentar acessar a porta bloqueada.
  - Variação dinâmica de pitch (`AudioSource.pitch`): o tom do som de coleta dos fragmentos se eleva progressivamente a cada item encontrado (ex: 1.0, 1.1, 1.2... até 1.4).
- [X] **Mecânica da Moema (Interior do Castelo):**
  - Troca de sprite/estado com delay customizável (`EmaSleeping` -> `EmaAwake`).
  - Sombra projetada no bloco (`Blob/Drop Shadow`) para garantir ancoragem visual do sprite no cenário 3D.
- [X] **Persistência de Progresso (`PlayerPrefs`):**
  - Salvamento do último panorama visitado (retorna ao panorama 15 ao sair do castelo em vez de reiniciar no ponto zero).
  - Fragmentos coletados salvos permanentemente via chaves únicas (`Fragmento_X`), evitando contagem duplicada e impedindo perda do progresso ao mudar de cena.
  - Botão "JOGAR" no Menu Principal que reseta limpo os dados do `PlayerPrefs` para um novo jogo do zero.
- [X] **Tela de Vitória & Loop de Fim de Jogo (`GameWonPanel`):**
  - Painel de encerramento ativado com delay após a Moema acordar.
  - Botão `ReturnToMenuButton` para voltar ao Menu Principal.
  - Botão `ReturnToGameButton` para explorar livremente a sala, ativando um botão HUD fixo (`InGameReturnToMenuButton`).

---

## Próximas Etapas / Polimento

- [ ] **Efeitos Sonoros Adicionais (SFX):**
  - Efeito sonoro de bocejo/despertar ao acionar o botão de interação com a Moema.
  - Trilha sonora suave/ambiente para o interior do castelo.
- [ ] **Polimento Visual & Arte de UI:**
  - Substituição dos botões padrão por sprites e ícones customizados estilizados.
  - Ajuste de animações suaves de transição (Fade In / Fade Out via CanvasGroup) na troca de cenas e panoramas.

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

- **Gatilho de Visão por Bússola:** Desenvolvimento da normalização angular (`NormalizarAngulo` / `ChecarAnguloNoIntervalo`) para converter rotações negativas do Unity (ex: -90° transformado em 270°) e validar o campo de visão do jogador.
- **Fase de Interior (`CastleInteriorScene`):** Criação da cena dedicada para o interior do castelo com suporte a ângulo de entrada customizado no `CameraRotator`.

#### **Etapa 2: Gamificação, Áudio Dinâmico e Navegação Contextual**

- **Sistema de Coleta e Diário (`JournalManager.cs` / `JournalFragment.cs`):** Centralização do áudio de coleta no manager e implementação de curva de pitch ascendente a cada item coletado.
- **Navegação Invertida Dinâmica:** Refatoração do `StreetViewManager.cs` para manter botões de navegação sempre na tela e inverter o sentido de avanço/recuo caso o jogador esteja olhando para trás no cenário.
- **Persistência de Dados entre Cenas:** Resolução do bug de reset de progresso. Implementação de chaves únicas no `PlayerPrefs` (`Fragmento_X` e `UltimoPanoramaIndex`) permitindo entrar/sair do castelo sem perder os itens coletados.
- **Animação de Despertar e Tela de Vitória:** Implementação das Coroutines no `CastleInteriorManager.cs` com suporte a delays customizáveis, ativação de sprites e fluxo de botões para explorar a sala ou retornar ao menu.

---

## Tecnologias Utilizadas

- **Engine:** Unity 6 (URP - Universal Render Pipeline)
- **Linguagem:** C#
- **Hospedagem WebGL:** itch.io
- **Assistente IA:** Gemini (Pair Programming e Resolução de Bugs)