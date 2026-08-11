<h1 align="center">🎮 Virtual Nightmare</h1>

<p align="center">
  <i>Prototipo de horror en primera persona — Character Controller, cámara e interacción</i>
</p>

<p align="center">
  🚧 Desarrollo activo
</p>

---

# 🛠 Estado Actual del Desarrollo

El proyecto se encuentra actualmente en una etapa de desarrollo centrada en construir una **base sólida y escalable para el personaje**, tomando como referencia experiencias de horror en primera persona como **Outlast**.

El objetivo de esta etapa es desarrollar primero los sistemas fundamentales de:

- Movimiento.
- Cámara.
- Input.
- Interacción.
- Máquina de estados.
- Física del personaje.
- Acciones especiales de movimiento.

La arquitectura está siendo diseñada para permitir agregar nuevas mecánicas sin convertir el `PlayerController` en una clase excesivamente grande y difícil de mantener.

---

# 🎯 Objetivo del Prototipo

El objetivo es desarrollar un juego de horror en primera persona donde el **movimiento, la cámara y la interacción con el entorno** tengan un papel importante en la experiencia.

La intención es conseguir una sensación de control similar a juegos como:

- Outlast.
- Amnesia.
- Alien: Isolation.

El personaje deberá poder realizar diferentes acciones dependiendo de su estado actual y de las condiciones del entorno.

---

# 🧍 Sistema de Personaje

El personaje está construido mediante diferentes sistemas independientes.

La estructura principal actualmente es:

```text
InputSystem_Actions
        │
        ▼
PlayerInputHandler
        │
        ▼
PlayerController
        │
        ├───────────────┬─────────────────┐
        ▼               ▼                 ▼
CharacterStateMachine  CameraController  CharacterInteractor
        │
        ▼
ICharacterState
        │
        ├── IdleState
        ├── WalkingState
        └── RunningState
        │
        ▼
CharacterMotor
```

Cada sistema tiene una responsabilidad específica.

---

# 🎮 Input System

El proyecto utiliza el **Unity Input System**.

Se creó un `Input Actions Asset` y se generó su correspondiente clase C#.

El código generado permite acceder a las acciones directamente desde C# sin depender del componente `Player Input`.

## Acciones actuales

```text
Move
Look
Interact
Sprint
LeanLeft
LeanRight
LookBack
Jump
Crouch
```

Algunas acciones ya están conectadas al gameplay, mientras que otras están preparadas para futuras mecánicas.

---

# 🔌 PlayerInputHandler

`PlayerInputHandler` es responsable de detectar y traducir los inputs del jugador.

No contiene la lógica específica de gameplay.

Su función es transformar:

```text
Input System
      ↓
PlayerInputHandler
      ↓
Datos / Eventos
```

## Input continuo

Actualmente maneja información como:

```csharp
public Vector2 MoveInput { get; private set; }
public Vector2 LookInput { get; private set; }
```

## Eventos

También utiliza eventos `Action` para acciones puntuales:

```text
OnInteract
OnRunStarted
OnRunCanceled
OnLeanLeft
OnLeanRight
OnLeanReset
OnLookBack
OnLookBackCanceled
```

Esto permite que otros sistemas reaccionen al input sin acoplarse directamente al Input System.

---

# 🎯 PlayerController

`PlayerController` funciona como el **orquestador del personaje**.

Su responsabilidad es recibir las acciones del jugador y decidir qué sistema debe encargarse de ellas.

Por ejemplo:

```text
Jugador presiona Shift
        ↓
PlayerInputHandler
        ↓
PlayerController
        ↓
CharacterStateMachine
        ↓
RunningState
        ↓
CharacterMotor
```

Otro ejemplo:

```text
Jugador presiona Interact
        ↓
PlayerInputHandler
        ↓
PlayerController
        ↓
CharacterInteractor
```

El `PlayerController` no debería encargarse directamente de implementar la física, la cámara o la interacción.

---

# 🤖 CharacterHuman

`CharacterHuman` funciona como una clase base para personajes humanos.

Actualmente proporciona la base común para la máquina de estados:

```text
CharacterHuman
      │
      ▼
CharacterStateMachine
```

También permite que futuros personajes humanos puedan compartir sistemas y comportamientos comunes.

---

# 🔄 Character State Machine

El proyecto utiliza una **máquina de estados basada en clases e interfaces**.

La interfaz principal es:

```csharp
public interface ICharacterState
{
    bool CanMove { get; }

    void EnterState();
    void UpdateState();
    void ExitState();
}
```

Cada estado representa un comportamiento concreto del personaje.

La máquina de estados es responsable de realizar las transiciones:

```text
Estado actual
      ↓
ExitState()
      ↓
ChangeState()
      ↓
Nuevo estado
      ↓
EnterState()
```

## Estados implementados

```text
IdleState
WalkingState
RunningState
```

## Estados planificados

```text
CrouchingState
JumpingState
FallingState
SlidingState
VaultingState
MantlingState
ClimbingState
```

---

# 💤 IdleState

Representa al personaje cuando no está realizando movimiento.

Estado inicial previsto para el personaje.

```text
Idle
 ↓
Movimiento detectado
 ↓
Walking
```

---

# 🚶 WalkingState

Representa el movimiento normal del personaje.

Transiciones previstas:

```text
Idle
 ↓
Walking
 ↓
Idle
```

y:

```text
Walking
 ↓
Sprint
 ↓
Running
```

---

# 🏃 RunningState

Representa el estado de Sprint.

El estado puede comunicarse con `CharacterMotor` para modificar parámetros relacionados con el movimiento.

Ejemplo conceptual:

```text
RunningState
      ↓
CharacterMotor
      ↓
Run Speed
```

Al abandonar el estado:

```text
RunningState
      ↓
ExitState()
      ↓
Movimiento normal
```

El objetivo es que el estado sea responsable de las características propias de correr, mientras que `CharacterMotor` continúa siendo responsable de cómo se aplica físicamente el movimiento.

---

# 🏃‍♂️ Reglas actuales del Sprint

El Sprint tiene prioridad sobre ciertas acciones de cámara.

Cuando el jugador comienza a correr:

### Lean

Si el personaje estaba haciendo Lean:

```text
Lean
 ↓
Sprint
 ↓
Cancelar Lean
```

### Look Back

Si el personaje estaba mirando hacia atrás:

```text
Look Back
 ↓
Sprint
 ↓
Cancelar Look Back
```

También se bloquean nuevas acciones de Lean y Look Back mientras el personaje está corriendo.

---

# 📷 CameraController

`CameraController` controla exclusivamente el comportamiento de la cámara.

Actualmente contiene:

- Sensibilidad.
- Rotación horizontal.
- Rotación vertical.
- Límite vertical.
- Lean izquierdo.
- Lean derecho.
- Reset del Lean.
- Look Back.
- Reset del Look Back.

La rotación se mantiene mediante valores internos:

```text
rotationX
rotationY
rotationZ
```

La cámara aplica finalmente:

```csharp
transform.localRotation = Quaternion.Euler(
    rotationX,
    rotationY,
    rotationZ
);
```

Esto permite combinar diferentes comportamientos de cámara sin sobrescribir directamente la rotación completa desde múltiples métodos.

---

# ↔️ Lean System

El personaje puede inclinarse hacia ambos lados.

```text
Q → Lean Left

E → Lean Right
```

Al liberar el botón:

```text
Q/E
 ↓
Reset Lean
```

El Lean se bloquea mientras el personaje está corriendo.

Si el jugador comienza a correr mientras está inclinado:

```text
Lean
 ↓
Sprint
 ↓
Reset Lean
 ↓
Running
```

---

# 🔄 Look Back

El personaje puede mirar hacia atrás mientras no está corriendo.

```text
Look Back
 ↓
CameraController
 ↓
rotationY = 180°
```

Al cancelar:

```text
Look Back
 ↓
Reset Look Back
 ↓
rotationY = 0°
```

El sistema también impide utilizar Look Back mientras el personaje está corriendo.

Si comienza el Sprint mientras mira hacia atrás:

```text
Look Back
 ↓
Sprint
 ↓
Reset Look Back
 ↓
Running
```

---

# 🏋️ CharacterMotor

`CharacterMotor` es responsable del **movimiento físico del personaje**.

Su objetivo es encapsular la lógica relacionada con:

- Movimiento.
- Velocidad.
- Sprint.
- Gravedad.
- Movimiento vertical.
- `CharacterController`.
- Detección del suelo.
- Caídas.
- Futuras físicas del personaje.

La separación principal es:

```text
CharacterStateMachine
        ↓
¿Qué estado tiene el personaje?

CharacterMotor
        ↓
¿Cómo se mueve físicamente?
```

Esto permite que los estados puedan modificar determinadas características del movimiento sin implementar ellos mismos la física.

---

# 🎯 CharacterInteractor

Sistema encargado de detectar objetos interactuables.

Actualmente utiliza un Raycast desde la cámara:

```text
Camera
  │
  ▼
Raycast
  │
  ▼
Collider
  │
  ▼
IInteractable
  │
  ▼
Interact()
```

La intención es evitar que el `PlayerController` tenga conocimiento de cada objeto interactuable.

---

# 🔌 IInteractable

Los objetos que puedan ser utilizados por el jugador implementarán:

```csharp
public interface IInteractable
{
    void Interact();
}
```

Esto permitirá crear diferentes tipos de objetos:

```text
Puertas
Botones
Palancas
Notas
Objetos recogibles
Cajones
Computadoras
Interruptores
Etc.
```

Todos pueden ser detectados por el mismo `CharacterInteractor`.

---

# 🏗️ Arquitectura General

La arquitectura actual sigue el siguiente flujo:

```text
                 INPUT
                   │
                   ▼
        ┌────────────────────┐
        │ InputSystem_Actions│
        └─────────┬──────────┘
                  │
                  ▼
        ┌────────────────────┐
        │ PlayerInputHandler │
        └─────────┬──────────┘
                  │
                  ▼
        ┌────────────────────┐
        │   PlayerController │
        └─────────┬──────────┘
                  │
        ┌─────────┼───────────┐
        │         │           │
        ▼         ▼           ▼
     Camera    Interactor   StateMachine
     System        │           │
                   │           ▼
                   │       CharacterState
                   │           │
                   │           ▼
                   │      CharacterMotor
                   │
                   ▼
             Interactable
```

La idea principal es:

> **InputSystem detecta → PlayerInputHandler traduce → PlayerController coordina → cada sistema ejecuta su responsabilidad.**

---

# 📐 Diagramas

Los diagramas de arquitectura se almacenarán dentro del repositorio.

## Arquitectura General

<p align="center">
  <img src="Documentation/Architecture/architecture.png" width="800" alt="Arquitectura general"/>
</p>

## Diagrama de Clases

<p align="center">
  <img src="Documentation/Architecture/class-diagram.png" width="800" alt="Diagrama de clases"/>
</p>

## Flujo del Personaje

<p align="center">
  <img src="Documentation/Architecture/player-flow.png" width="800" alt="Flujo del personaje"/>
</p>

> Los diagramas serán actualizados a medida que evolucione la arquitectura.

---

# 📸 Capturas del Prototipo

Las capturas del desarrollo se almacenarán en:

```text
Documentation/
└── Screenshots/
```

Ejemplo:

<p align="center">
  <img src="Documentation/Screenshots/gameplay.png" width="700" alt="Gameplay"/>
</p>

---

# 🚧 Roadmap

## ✔ Sistemas implementados

- [x] Unity Input System.
- [x] Input Actions generado en C#.
- [x] PlayerInputHandler.
- [x] Sistema de eventos `Action`.
- [x] PlayerController.
- [x] CharacterHuman.
- [x] CharacterMotor.
- [x] CharacterStateMachine.
- [x] ICharacterState.
- [x] IdleState.
- [x] WalkingState.
- [x] RunningState.
- [x] Cámara FPS personalizada.
- [x] Rotación horizontal.
- [x] Rotación vertical.
- [x] Clamp vertical.
- [x] Sistema de sensibilidad.
- [x] Lean Left.
- [x] Lean Right.
- [x] Reset Lean.
- [x] Look Back.
- [x] Reset Look Back.
- [x] Cancelación de Lean al correr.
- [x] Cancelación de Look Back al correr.
- [x] Bloqueo de Lean durante Sprint.
- [x] Bloqueo de Look Back durante Sprint.
- [x] CharacterInteractor.
- [x] Raycast de interacción.
- [x] IInteractable.

---

# 🕒 Próximos sistemas

## 🧍 Movimiento

- [ ] Crouching.
- [ ] Jumping.
- [ ] Falling.
- [ ] Control de gravedad.
- [ ] Diferenciación entre suelo y aire.
- [ ] Sliding.
- [ ] Vaulting.
- [ ] Mantling.
- [ ] Climbing.
- [ ] Superar obstáculos automáticamente.
- [ ] Transiciones entre diferentes alturas.

---

## 🏃 Movimiento avanzado

### Slide

Implementar:

```text
Running
   │
   ▼
Crouch
   │
   ▼
Sliding
```

El objetivo es que si el jugador está corriendo y presiona el botón de agacharse:

```text
Run + Crouch
      ↓
    Slide
```

En lugar de pasar directamente a Crouching.

---

## 🧎 Crouching

Implementar:

```text
Walking → Crouching
Crouching → Walking
```

Y posteriormente:

```text
Running → Crouching
         ↓
       Sliding
```

También será necesario comprobar si existe espacio suficiente sobre la cabeza antes de volver a ponerse de pie.

---

## 🦘 Jump / Fall

Separar el salto de la caída:

```text
Walking
   ↓
Jumping
   ↓
Falling
   ↓
Walking
```

La detección del suelo y la gravedad serán responsabilidad de `CharacterMotor`.

---

## 🧗 Vault / Mantle

Agregar detección de obstáculos delante del jugador.

Conceptualmente:

```text
Player
  │
  ▼
Obstacle Detection
  │
  ├── Obstáculo bajo
  │       ↓
  │     Vault
  │
  └── Obstáculo alto
          ↓
        Mantle
```

El sistema deberá comprobar:

- Altura del obstáculo.
- Distancia.
- Espacio disponible.
- Posición de aterrizaje.
- Estado actual del personaje.

---

# 🎥 Próximas mejoras de Cámara

- [ ] Head Bob.
- [ ] FOV durante Sprint.
- [ ] FOV durante Slide.
- [ ] Camera Shake.
- [ ] Landing Camera Effect.
- [ ] Efectos de respiración.
- [ ] Movimiento de cámara durante Vault.
- [ ] Movimiento de cámara durante Mantle.
- [ ] Transiciones de cámara.
- [ ] Animaciones de cámara.

---

# 🎬 Animaciones

Una vez terminada la base de movimiento:

- [ ] Animator Controller.
- [ ] Idle Animation.
- [ ] Walk Animation.
- [ ] Run Animation.
- [ ] Crouch Animation.
- [ ] Jump Animation.
- [ ] Fall Animation.
- [ ] Slide Animation.
- [ ] Vault Animation.
- [ ] Mantle Animation.
- [ ] Climb Animation.
- [ ] Lean Animation.
- [ ] Animaciones de interacción.

Los estados podrán utilizarse posteriormente para enviar parámetros al Animator.

---

# 🎮 Gameplay Futuro

Una vez terminada la base del Character Controller:

- [ ] Sistema de interacción avanzado.
- [ ] Objetos recogibles.
- [ ] Puertas.
- [ ] Armarios.
- [ ] Escondites.
- [ ] Sistema de inventario.
- [ ] Objetos utilizables.
- [ ] Sistema de stamina.
- [ ] Sistema de daño.
- [ ] Sistema de muerte.
- [ ] Sistema de checkpoints.
- [ ] Sistema de persecución.
- [ ] IA enemiga.
- [ ] Detección visual.
- [ ] Detección auditiva.
- [ ] Sistema de ruido.
- [ ] Eventos de gameplay.

---

# 🧠 Filosofía de Arquitectura

El proyecto busca mantener una separación clara entre responsabilidades.

### Input

```text
InputSystem_Actions
        ↓
PlayerInputHandler
```

### Coordinación

```text
PlayerController
```

### Estado

```text
CharacterStateMachine
        ↓
ICharacterState
```

### Movimiento

```text
CharacterMotor
```

### Cámara

```text
CameraController
```

### Interacción

```text
CharacterInteractor
        ↓
IInteractable
```

La intención es evitar que una única clase controle todos los sistemas del personaje.

---

# 📁 Estructura del Proyecto

```text
Assets/
│
├── Scripts/
│   │
│   ├── Character/
│   │   ├── CharacterHuman.cs
│   │   ├── CharacterMotor.cs
│   │   ├── CharacterStateMachine.cs
│   │   │
│   │   └── States/
│   │       ├── ICharacterState.cs
│   │       ├── IdleState.cs
│   │       ├── WalkingState.cs
│   │       └── RunningState.cs
│   │
│   ├── Player/
│   │   ├── PlayerController.cs
│   │   └── PlayerInputHandler.cs
│   │
│   ├── Camera/
│   │   └── CameraController.cs
│   │
│   └── Interaction/
│       ├── CharacterInteractor.cs
│       └── IInteractable.cs
│
├── Input/
│   ├── InputSystem_Actions.inputactions
│   └── InputSystem_Actions.cs
│
└── Documentation/
    │
    ├── Architecture/
    │   ├── architecture.png
    │   ├── class-diagram.png
    │   └── player-flow.png
    │
    └── Screenshots/
        └── gameplay.png
```

---

# 🛠 Tecnologías

- **Unity 6**
- **C#**
- **Unity Input System**
- **Character Controller**
- **Physics / Raycast**
- **Programación Orientada a Objetos**
- **Interfaces**
- **Eventos `Action`**
- **Máquina de Estados**
- **Git**
- **GitHub**
- **ProBuilder** para prototipado

---

# 📌 Estado del Proyecto

🚧 **EN DESARROLLO**

Actualmente la prioridad es terminar el **Character Controller** y establecer una arquitectura sólida antes de comenzar a desarrollar los sistemas principales de gameplay.

El siguiente objetivo es ampliar la máquina de estados con:

```text
Crouching
    ↓
Jumping
    ↓
Falling
    ↓
Sliding
    ↓
Vaulting
    ↓
Mantling
    ↓
Climbing
```

y conectar progresivamente estos estados con:

- CharacterMotor.
- CameraController.
- Animator.
- Física.
- Detección de obstáculos.

---

<p align="center">
  <b>🎮 Proyecto desarrollado en Unity + C#</b>
</p>

<p align="center">
  <i>Prototipo en desarrollo</i>
</p>
