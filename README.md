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
- [X] **Gerenciador de Transição de Cenas Global (`SceneTransitionManager.cs`):**
  - Objeto persistente via `DontDestroyOnLoad` com um único `CanvasGroup` e renderização em ordem prioritária (`Sort Order 9999`).
  - Carregamento assíncrono de cenas (`LoadSceneAsync`) integrado a efeitos suaves de **Fade Out** e **Fade In** (cortina preta).
  - Áudio sincronizado no início do corte de cena.
- [X] **Panorama Inicial Customizável:** Configuração de índice padrão (`indiceInicialDefault`) para início em panorama específico sem perder o ponto de retorno ao sair do castelo.
- [X] **Limite de Mapa (Feedback Visual e Sonoro):** Alerta de final de percurso com efeito *flash* vermelho, *fade out* suave controlável via Inspector e efeito sonoro de bloqueio.
- [X] **Estrutura Múltipla de Cenas com Transição Suave:** Fluxo contínuo entre `MenuScene`, `SampleScene` (Área Externa) e `CastleInteriorScene` (Interior do Castelo).
- [X] **Menu Inicial Interativo:** Fundo panorâmico 360° em rotação contínua (`MenuPanoramaRotator.cs`).
- [X] **Gatilhos Condicionais por Ângulo da Câmera:**
  - O botão de entrada no castelo (`EnterCastleButton`) só é exibido no panorama 15 quando o jogador aponta a câmera diretamente na direção da porta.
  - O botão de saída e o botão de ação no interior só aparecem nos ângulos específicos das estruturas/personagem.
- [X] **Mecânica de Gamificação & Diário de Bordo:**
  - Coleta de 5 fragmentos escondidos nos panoramas via clique 3D (`Raycast` e `Colliders`).
  - Atualização assíncrona automática da visibilidade dos fragmentos ($1$ frame após o fim do carregamento da cena) para sincronização perfeita com a transição.
  - Contador de progresso condicional na HUD (`3/5`), ativado somente após a primeira coleta.
  - Trava de segurança no castelo: tentar entrar sem o diário completo exibe o aviso temporário *"Você não pode entrar aqui ainda!"*.
  - Revelação do diário restaurado em pergaminho ao coletar o 5º fragmento com botão de fechar (`CloseBookButton`).
- [X] **Sistema de Áudio Centralizado & Escalas de Volume Exclusivas:**
  - Arquitetura de gerenciamento via `AudioManager.cs` e `JournalManager.cs`.
  - **Sons de Passos em Movimentação:** Array de efeitos sonoros sorteados aleatoriamente ao caminhar pelos panoramas, com controle de volume independente (`volumePassos`), acionados apenas em passos válidos (sem ruído extra de clique de botão UI nem som em barreiras).
  - **Controle Fino de Áudio:** Sliders individuais no Inspector para controle de escala de volume de cliques (`volumeCliques`), erros (`volumeErroBloqueio`), passos e efeitos especiais.
  - **Sons de Coleta:** Sorteio aleatório entre 3 arquivos de áudio de páginas/livro ao coletar um fragmento, tocados no tom natural (1.0).
  - Som exclusivo acionado na aparição da tela de vitória (`somVitoria`).
- [X] **Mecânica da Moema (Interior do Castelo):**
  - Troca de sprite/estado com delay customizável (`EmaSleeping` -> `EmaAwake`) e reprodução de som de despertar.
  - Sombra projetada no bloco (`Blob/Drop Shadow`) para garantir ancoragem visual do sprite no cenário 3D.
- [X] **Persistência de Progresso (`PlayerPrefs`):**
  - Salvamento do último panorama visitado (retorna ao panorama 15 ao sair do castelo em vez de reiniciar no ponto zero).
  - Fragmentos coletados salvos permanentemente via chaves únicas (`Fragmento_X`), evitando contagem duplicada e impedindo perda do progresso ao mudar de cena.
  - Botão "JOGAR" no Menu Principal que reseta limpo os dados do `PlayerPrefs` para um novo jogo do zero.
- [X] **Tela de Vitória & Loop de Fim de Jogo (`GameWonPanel`):**
  - Painel de encerramento ativado com delay após a Moema acordar, acompanhado por efeito sonoro de vitória (`somVitoria`).
  - Botão `ReturnToMenuButton` para voltar ao Menu Principal.
  - Botão `ReturnToGameButton` para explorar livremente a sala, ativando um botão HUD fixo (`InGameReturnToMenuButton`).

---

## Polimento Visual & Arquitetura

- [X] **Efeitos Sonoros Dedicados (SFX):**
  - Efeito sonoro de despertar ao acionar a interação com a Moema.
  - Som de jingle/vitória no surgimento do painel de encerramento.
  - Passos táticos nos botões de navegação e efeito sonoro na colisão com as bordas do mapa.
- [X] **Transição Suave (Fade In / Fade Out):**
  - Sistema global desacoplado responsável pelo gerenciamento de telas pretas e trocas de cena assíncronas em toda a aplicação.

---

## AI Logbook

Este projeto utilizou Inteligência Artificial Generativa (**Gemini**) como parceira de desenvolvimento (*Pair Programming*) para arquitetura de código C#, refatoração de matemática vetorial/angular, persistência de dados, design de gerenciadores de áudio e transição de cenas, e otimização de exportação WebGL.

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

- **Sistema de Coleta e Diário (`JournalManager.cs` / `JournalFragment.cs`):** Centralização do áudio de coleta no manager com sorteio aleatório de sons.
- **Navegação Invertida Dinâmica:** Refatoração do `StreetViewManager.cs` para manter botões de navegação sempre na tela e inverter o sentido de avanço/recuo caso o jogador esteja olhando para trás no cenário.
- **Persistência de Dados entre Cenas:** Resolução do bug de reset de progresso. Implementação de chaves únicas no `PlayerPrefs` (`Fragmento_X` e `UltimoPanoramaIndex`) permitindo entrar/sair do castelo sem perder os itens coletados.
- **Animação de Despertar e Tela de Vitória:** Implementação das Coroutines no `CastleInteriorManager.cs` com suporte a delays customizáveis, ativação de sprites e fluxo de botões para explorar a sala ou retornar ao menu.

#### **Etapa 3: Gerenciamento Global, Polimento de Áudio e Transição de Cenas**

- **Sistema de Passos e Escalas de Volume:** Refatoração do `AudioManager.cs` para suportar lista de áudios aleatórios de passos e escalas de volume exclusivas por tipo de som (passos, cliques UI, erros).
- **Remoção de Duplicação de Clique:** Limpeza dos ouvintes de eventos no Inspector para que as setas de movimentação reproduzam unicamente os passos do panorama ao mudar de foto.
- **Gerenciador de Transições Global (`SceneTransitionManager.cs`):** Criação do componente Singleton com `DontDestroyOnLoad` responsável por controlar o `CanvasGroup` de transição, reproduzir o áudio de troca e executar o `LoadSceneAsync`.
- **Sincronização de Sinais e Visibilidade:** Ajuste no `StreetViewManager.cs` e `JournalFragment.cs` utilizando `FindObjectsInactive.Include` e espera de $1$ frame após o carregamento assíncrono para garantir que os fragmentos do diário sejam exibidos instantaneamente no panorama carregado.

---

## Tecnologias Utilizadas

- **Engine:** Unity 6 (URP - Universal Render Pipeline)
- **Linguagem:** C#
- **Hospedagem WebGL:** itch.io
- **Assistente IA:** Gemini (Pair Programming e Resolução de Bugs)