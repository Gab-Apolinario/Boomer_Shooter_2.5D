# ⚡ Projeto Ragnarok — Boomer Shooter 2.5D

> FPS de ritmo acelerado inspirado nos clássicos dos anos 90, com estética
> cyberpunk fundida à mitologia nórdica. Você é um robô descontrolado
> enfrentando IAs corrompidas por pedaços de deuses nórdicos, capturando
> pontos abençoados por runas antigas para conseguir poderes temporários.

🕹️ **[Jogar no itch.io](https://gabriel-apolinario.itch.io/projeto-ragnarok)** | 📂 **[Código-fonte](https://github.com/Gab-Apolinario/Boomer_Shooter_2.5D)**

Projeto desenvolvido para a disciplina de Experiência Criativa: Prototipação
de Jogos (PUCPR) e exibido no **PUC Game Show 2026**.

---

## 📖 Sobre o Projeto

Boomer Shooter 2.5D em primeira pessoa, com sprites billboard animados sobre
cenários 3D rodando em Unity. O jogador enfrenta waves
de inimigos cada vez mais variadas enquanto captura pontos de controle
espalhados pelo mapa — cada captura concede um power-up temporário e mantém o
jogo em ritmo constante, sem pausas.

Equipe: **Gabriel Apolinário** (programação e liderança técnica), **Lucas
Raphael** (game design e direção de arte / mapas), **Leonardo Oliveri**
(arte 2D) e **Uirá Lima** (efeitos visuais e, posteriormente, efeitos
sonoros).

## 👨‍💻 Meu Papel

Atuei como **desenvolvedor líder** do projeto, responsável por praticamente
toda a implementação de código, pela integração dos assets produzidos pela
equipe (arte, VFX, mapas) e pela documentação técnica do processo via
changelogs semanais ao longo de 5 sprints.

## 🏗️ Arquitetura

### Estrutura de Pastas

```
Assets/Game/
├── Actors/
│   ├── Player/       — Player.cs, PlayerController.cs, PlayerCamera.cs, WeaponBob.cs
│   └── Enemy/         — BaseEnemy.cs + MeleeEnemy/RangedEnemy, DataSOs, Billboard, AnimationEvents
├── Combat/            — WeaponSystem.cs, MeleeSystem.cs, PowerUpsSO.cs, DamageZone.cs, DamageIndicatorUI.cs
├── Core/               — Acoes.cs (barramento de eventos), ControlPoint.cs, SceneLoader.cs
├── Managers/          — GameManager, UIManager, AudioManager, BuffManager, WaveManager,
│                         SettingsManager, LeaderboardManager, RuneUIManager, PowerUpSelectionUI
├── Input/             — InputHandler.cs (New Input System)
├── Particles/         — VFX de dash, cura, shield, buff, impacto, muzzle flash
└── Data/              — HealthPickup.cs
```

O projeto é estruturado em torno de três pilares:

```
Acoes.cs (barramento de eventos estático)
        │
   ┌────┼────────────────┬─────────────────┐
   ▼    ▼                ▼                 ▼
Player  Enemies      ControlPoints      UI / Managers
(dash,  (SO-driven,  (estados +          (HUD, RuneUI,
 arma,   billboard,   beam visual +       PowerUp modal,
 melee)  Animator)    condição vitória)   Settings)
```

**Sistema de eventos (`Acoes.cs`)** — todos os sistemas se comunicam via
`Action` delegates estáticos, sem referências diretas entre si (mesmo padrão
já usado no Clone SeaQuest, evoluído aqui para um projeto bem maior).

**Inimigos guiados por ScriptableObject** — `EnemyDataSO`, com
`MeleeEnemyDataSO` e `RangedEnemyDataSO` herdando dela, separam dados (HP,
dano, velocidade, FOV) de comportamento (`BaseEnemy.cs` → `MeleeEnemy.cs` /
`RangedEnemy.cs`). Isso permite criar variações inteiras de inimigo trocando
apenas o `.asset`, sem duplicar código.

**Pontos de Controle (`ControlPoint.cs`)** — a mecânica central do jogo.
Três estados (`Neutro → Capturando → Capturado`), beam visual via
`LineRenderer` com gradiente de cor dinâmico, `ControlPointPointer.cs`
indicando o próximo ponto ativo, e uma sequência circular embaralhada no
`GameManager` (Fisher-Yates + fila circular) que define a condição de
vitória.

## ⚙️ Sistemas Principais

| Sistema | Descrição |
| --- | --- |
| **Movimento** | Dash direcional com i-frames de invencibilidade como único mecanismo de reposicionamento (pulo/sprint/stamina foram removidos e depois o pulo foi reimplementado a partir do histórico do Git quando analisamos os feedbacks de playtests) |
| **Arma** | Sistema de superaquecimento (substitui munição tradicional) configurado via `WeaponConfigSO`, com suporte a múltiplas armas e troca entre modo à distância e corpo a corpo (`WeaponSystem` como orquestrador, `MeleeSystem` como executor) |
| **Inimigos** | `BaseEnemy` → `MeleeEnemy` / `RangedEnemy`, dados via ScriptableObject, sprites billboard (`BillboardBehaviour.cs`) sempre voltados à câmera, Animator Controller por tipo com ponte de eventos (`EnemyAnimationEvents.cs`) |
| **Pontos de Controle & Power-ups** | `ControlPoint.cs`, `PowerUpsSO`, `BuffManager` (aplica e expira buffs) e modal de seleção com 3 opções aleatórias sem repetição |
| **UI/HUD** | HUD, `RuneUIManager` (indicadores sequenciais de captura), `DamageIndicatorUI` (flash direcional de dano), menu de pause com sensibilidade ajustável, tela de controles, `SettingsManager` com persistência via `PlayerPrefs` |
| **Leaderboard Online** | Ranking via LootLocker (`LeaderboardManager.cs`), com UI dentro do jogo e uma página HTML standalone hospedada no GitHub Pages, embutida na página do itch.io. Pontuação parseada em C# e JavaScript a partir de uma mesma string delimitada (`"tempo;waves"`) |
| **Ondas e Input** | `WaveManager.cs` controla o spawn escalonado de inimigos; `InputHandler.cs` centraliza o New Input System; `MainMenuManager.cs` e `SceneLoader.cs` cuidam de navegação entre cenas; `CheatCodes.cs` mantém os atalhos de debug usados em teste (ex: troca de arma via F9) |

## 🔄 Processo de Desenvolvimento

O projeto foi construído ao longo de **5 sprints semanais**, cada uma
documentada em changelog próprio:

- **Sprint 1** — Refatoração do dash (direcional por input), sensibilidade
  ajustável, sistema de superaquecimento substituindo munição,
  `WeaponConfigSO`, suporte a múltiplas armas, `AudioManager`
- **Sprint 2** — Sistema de stamina, ataque corpo a corpo, VFX de
  corrida/dash/cura, indicadores de tela (`OnScreenPointer`)
- **Sprint 3** — Redesign de movimento (remoção de pulo/sprint/stamina em
  favor do dash com i-frames), `PowerUpsSO`, `BuffManager`, troca de arma
  melee/ranged, `ControlPoint`, `DamageZone`, refatoração de inimigos para
  ScriptableObjects, ambiente sandbox para colaboração da equipe
- **Sprint 4** — Sistema completo de `ControlPoints` com beam visual e
  condição de vitória, sequência circular shuffleada, cena principal
  montada com NavMesh e hazards
- **Sprint 5** — Migração visual para sprites billboard 2.5D, UI de pontos
  de controle com runas, indicador direcional de dano, shield cumulativo
  com feedback visual, correções de sincronização entre Animator e lógica

Uma decisão marcante veio de uma reunião de sprint com os professores: o
professor Artur sugeriu uma lista de mudanças que levaram a equipe a
adaptar o jogo quase por completo — entre elas, a mecânica de Pontos de
Controle na forma final e o sistema procedural de power-ups.

## 📚 O Que Aprendi

**Técnico**
- Arquitetura orientada a eventos (`Action` delegates) escalando de um
  projeto pequeno (Clone SeaQuest) para um projeto de equipe bem maior
- ScriptableObjects como fonte de dados, separando configuração de
  comportamento e permitindo iteração rápida sem tocar em código
- Geração procedural controlada (Fisher-Yates + fila circular) aplicada a
  design de jogo, não só a estruturas de dados
- Recuperar uma feature do histórico do Git (o pulo) para reaproveitar em
  um contexto de design diferente do original
- Integração entre sistemas de terceiros (LootLocker) e front-end web
  (HTML/JS) consumindo os mesmos dados que o jogo em C#

**Liderança e processo**
- Documentar decisões e progresso semanalmente (changelogs) mesmo sob
  pressão de prazo
- Lidar com níveis desiguais de engajamento em um time de faculdade, e a
  importância de comunicar isso a tempo com professores e colegas
- Adaptar escopo com clareza quando o prazo aperta (de várias fases
  planejadas para um único mapa em formato de waves) sem perder a
  identidade do projeto
- Aceitar feedback externo (professores e playtesters) como oportunidade de
  melhoria, mesmo exigindo reescrever partes já prontas

## 🛠️ Ferramentas & Tecnologias

| Ferramenta | Uso |
| --- | --- |
| **Unity 6 (URP)** | Engine e pipeline de renderização |
| **C#** | Lógica de jogo |
| **NavMesh** | Navegação de inimigos |
| **ScriptableObjects** | Configuração de inimigos, armas e power-ups |
| **LootLocker** | Backend de leaderboard online |
| **HTML/CSS/JS** | Página de leaderboard standalone (GitHub Pages) |
| **Git/GitHub** | Versionamento (usado como backup do Unity Version Control da equipe) |
| **itch.io** | Publicação da build WebGL |

## 🎓 Contexto Acadêmico

Projeto desenvolvido para a disciplina de Experiência Criativa:
Prototipação de Jogos, curso de Tecnologia em Jogos Digitais — PUCPR, e
exibido no PUC Game Show 2026. Projeto não-comercial, com fins
educacionais.

## 👥 Créditos

- **Gabriel Apolinário** — Programação, arquitetura, liderança técnica, documentação
- **Lucas Raphael** — Game design, direção de arte, mapas
- **Leonardo Oliveri** — Arte 2D (inimigo melee, cartas de power-up)
- **Uirá Lima** — Efeitos visuais e sonoros

---

*Feito com dedicação por um estudante de Jogos Digitais liderando seu
primeiro projeto de equipe em escala real.*
**Gabriel Apolinário** · Jogos Digitais · PUCPR
