# 🚁 Helicopter Attack 3D

[![Unity](https://img.shields.io/badge/Unity-2021%2F2022%20LTS-blue.svg?logo=unity)](https://unity.com/)
[![Language](https://img.shields.io/badge/Language-C%23-green.svg?logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Platform](https://img.shields.io/badge/Platform-Windows%20PC-red.svg?logo=windows)](https://www.microsoft.com/windows)
[![License](https://img.shields.io/badge/License-MIT-amber.svg)](LICENSE)

Videojuego interactivo de combate táctico aéreo-terrestre en 3D desarrollado en **Unity** y **C#**. El jugador pilotea un helicóptero militar fuertemente armado para defender una zona estratégica contra oleadas progresivas de tanques enemigos equipados con inteligencia artificial.

---

## 🎯 Características Principales

* 🎮 **Controles PC Tácticos (Teclado y Ratón):** Desplazamiento fluido en 6 direcciones, elevación/descenso dinámico y apuntado por ratón.
* 🌙 **Ciclo Ambiental Día y Noche Dinámico:** Transición continua de sol, iluminación ambiental y niebla exponencial.
* 📹 **Visión Nocturna Táctica (Cámara CCTV IR):** Procesamiento de imagen mediante Shader HLSL en escala de grises con estampa de fecha/hora en vivo, conmutable al presionar **`N`** o **`B`**.
* 🛡️ **Campaña de Supervivencia de 5 Minutos (5 Oleadas):** Progresión de cuota de bajas de tanques enemigos (Oleadas 1 a 5) con anuncios en pantalla.
* 📷 **Modos de Cámara Dinámicos:** Alterna en cualquier momento entre vista en **3ª Persona** (exterior) y **1ª Persona** (cabina de piloto) con las teclas **`V`**, **`C`** o **`Tab`**.
* 📋 **Menús de Inicio y Pausa:** Menú principal con selección de 3 campañas, control de volumen persistente y menú de pausa congelando la partida con **`Escape`** o **`P`**.

---

## ⌨️ Esquema de Controles

| Acción en el Juego | Tecla / Control | Descripción |
| :--- | :---: | :--- |
| **Movimiento Horizontal** | **`W` / `A` / `S` / `D`** | Mover adelante, atrás y lados (Strafe). |
| **Elevar (Subir Altura)** | **`Espacio` / `E`** | Aplica empuje vertical ascendente. |
| **Descender (Bajar Altura)** | **`Shift Izq` / `Ctrl Izq` / `Q`** | Reduce empuje vertical para descender. |
| **Apuntado y Giro de Vista** | **Movimiento del Ratón** | Apuntado dinámico de cámara y mira. |
| **Ametralladora Frontal** | **Clic Izquierdo del Ratón** *(o `J`)* | Fuego continuo de ráfaga rápida. |
| **Lanzamiento de Cohetes** | **Clic Derecho del Ratón** *(o `K`)* | Disparo de misiles explosivos alternados. |
| **Alternar Cámara (1ª/3ª P)** | **`V` / `C` / `Tab`** | Alterna entre cabina e inspección exterior. |
| **Visión Nocturna CCTV (On/Off)**| **`N` / `B`** | Activa/Desactiva el modo escala de grises CCTV. |
| **Menú de Pausa** | **`Escape` / `P`** | Congela el tiempo y abre el menú de pausa. |

---

## 🏗️ Arquitectura del Proyecto

```
HelicopterAttack/
├── Assets/
│   └── HelicopterAttack/
│       ├── Prefabs/          # Helicóptero, Tanques Enemigos, Proyectiles, Partículas, UI
│       ├── Scenes/           # Scene_MainMenu (Menú de Inicio), Scene_1 (Misión Principal)
│       ├── Scripts/
│       │   ├── Gameplay/     # PlayerHeli, EnemyTank, WaveManager, DayNightCycle, NightCameraEffect
│       │   ├── Input/        # InputControl (Captura de entradas PC)
│       │   └── UI/           # MainMenuUI, PauseUI, GameUI, LoseUI, WinUI
│       └── Shaders/          # NightVisionShader.shader (Efecto CCTV en Escala de Grises HLSL)
└── ProjectSettings/          # Configuración de compilación y entradas de Unity
```

---

## 🛠️ Requisitos e Instalación

### Requisitos del Sistema
* **SO:** Windows 10 / 11 (64-bit).
* **GPU:** Compatible con DirectX 11.
* **Entorno de Desarrollo:** Unity 2021 LTS o superior.

### Ejecución en Unity Editor
1. Clona este repositorio:
   ```bash
   git clone https://github.com/KevinCallo/TrabajoFinalCGVCYM.git
   ```
2. Abre la carpeta del proyecto en **Unity Hub**.
3. Navega a `Assets/HelicopterAttack/Scenes/` y abre **`Scene_MainMenu`** o **`Scene_1`**.
4. Presiona el botón **Play** ▶️ en Unity.

---

## 👥 Créditos y Repositorio

* **Desarrollado por:** Kevin Callo & Equipo de Desarrollo
* **Repositorio Oficial:** [https://github.com/KevinCallo/TrabajoFinalCGVCYM](https://github.com/KevinCallo/TrabajoFinalCGVCYM)
