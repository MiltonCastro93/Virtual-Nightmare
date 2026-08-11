<h1 align="center">🎮 Virtual Nightmare — Character Controller & Core Systems</h1>

<p align="center">
  <i>Prototipo de horror en primera persona — Desarrollo activo del personaje, cámara e interacción</i>
</p>

---

## 🛠 Estado Actual del Desarrollo

El prototipo está enfocado actualmente en construir una **base sólida para el personaje y sus sistemas principales**, tomando como referencia experiencias de horror en primera persona como **Outlast**.

El objetivo de esta etapa es desarrollar un Character Controller modular y escalable antes de comenzar con sistemas de gameplay más complejos.

Actualmente estoy trabajando en:

### 🔹 Sistema de Personaje

- Implementación del **Character Controller** como base del movimiento.
- Sistema de movimiento modular mediante `CharacterMotor`.
- Máquina de estados basada en clases mediante `ICharacterState`.
- Estados independientes para:
  - Idle.
  - Walking.
  - Running.
  - Crouching.
  - Jumping.
  - Falling.
  - Sliding.
  - Vaulting / Mantling.
- Sistema preparado para futuras mecánicas de movimiento.

### 🔹 Sistema de Cámara

- Cámara en primera persona programada desde cero.
- Rotación horizontal y vertical.
- Control de sensibilidad.
- Límites de rotación vertical.
- Lean izquierdo y derecho.
- Mirar hacia atrás.
- Cancelación automática de acciones incompatibles durante el Sprint.
- Preparación para efectos adicionales de cámara.

### 🔹 Sistema de Interacción

- Detección mediante Raycast.
- Distancia máxima de interacción.
- Sistema basado en `IInteractable`.
- Separación entre detección y comportamiento del objeto.

---

## 🎯 Mecánica Núcleo (Core)

El objetivo principal del prototipo es crear una experiencia de **horror y supervivencia en primera persona**, donde el movimiento y la sensación de control del personaje sean una parte fundamental de la experiencia.

### 👤 Personaje

El personaje está siendo diseñado alrededor de un sistema modular que permita incorporar diferentes comportamientos sin convertir el `PlayerController` en una clase monolítica.

Entre las mecánicas previstas:

- Caminar.
- Correr.
- Agacharse.
- Saltar.
- Caer.
- Deslizarse al agacharse mientras se corre.
- Superar obstáculos.
- Vault / Mantle.
- Trepar.
- Lean.
- Mirar hacia atrás.
- Interactuar con el entorno.

---

## 🧠 Arquitectura

El proyecto utiliza una arquitectura orientada a responsabilidades.

El flujo principal del personaje es:

```text
InputSystem_Actions
        │
        ▼
PlayerInputHandler
        │
        ▼
PlayerController
        │
        ├───────────────┬────────────────┐
        ▼               ▼                ▼
CharacterStateMachine  CameraController  CharacterInteractor
        │
        ▼
   CharacterState
        │
        ▼
   CharacterMotor
