# 🛡️ USB Guard

*[Read in English](README.en.md)*

Un poderoso sistema de seguridad Zero-Trust para entornos Windows que transforma cualquier memoria USB común en una Llave Maestra criptográfica para bloquear y desbloquear tu ordenador.

<p>
  <img src="https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet" alt=".NET 10" />
  <img src="https://img.shields.io/badge/WPF-UI-0078D4?style=for-the-badge&logo=windows" alt="WPF UI" />
  <img src="https://img.shields.io/badge/License-MIT-green?style=for-the-badge" alt="MIT License" />
</p>

[![Download usb-guard](https://a.fsdn.com/con/app/sf-download-button)](https://sourceforge.net/projects/usb-guard/files/latest/download)

[![Download usb-guard](https://img.shields.io/sourceforge/dt/usb-guard.svg)](https://sourceforge.net/projects/usb-guard/files/latest/download)

## Características Principales

* **Seguridad Cero-Confianza (Zero-Trust):** No basta con clonar archivos. El sistema lee el número de serie ensamblado en el hardware de tu USB para validar la propiedad.
* **Interfaz Glassmorphism de Álta Fidelidad:** Una pantalla de bloqueo inmersiva que se fusiona nativamente con tu fondo de pantalla (mediante llamadas directas Win32) logrando estéticas comparables a macOS y Windows 11.
* **Protección Criptográfica contra Fuerza Bruta:** Implementación rigurosa de retraso exponencial (Rate-Limiting). Tres intentos fallidos congelan la interfaz mediante cálculos logarítmicos que previenen ataques automatizados (*Rubber Ducky*).
* **Integración al Núcleo Local:** Funciones interconstruidas para silenciar la máquina al estar bloqueada, arrancar armados silenciosamente desde el Registro de Windows (Run), y persistir en la memoria de sistema.

## Instalación y Uso Rápido

> [!IMPORTANT]
> Para que el ejecutable (`.exe`) funcione correctamente, debes asegurarte de que el **Control Inteligente de Aplicaciones** (Smart App Control) de Windows esté **Desactivado** en la configuración de seguridad del sistema operativo.

1. **Compilación:** Abre la solución `USBGuard.sln` en tu Visual Studio. Asegúrate de tener instalado el SDK de **.NET 10.0 para Windows (WPF)**.
2. **Release:** Cambia el perfil de trabajo de "Debug" a "Release" para maximizar la ofuscación y el rendimiento, y presiona F5.
3. **Tu Primera Llave:**
   * Inserta un Pendrive (Memoria USB) normal en tu equipo.
   * Ve al Gestor de Dispositivos dentro de USB Guard.
   * Escribe una contraseña de recuperación sólida y presiona "Encriptar".
4. **Armado:** Haz clic en el botón gigante del escudo en el Panel Principal para armar el sistema. Al sacar la memoria, ¡la computadora quedará instantáneamente bloqueada!

## ¿Por qué USB Guard es distinto?

La mayoría de los programas comerciales "baratos" solo verifican si un archivo de texto existe dentro de la USB. **USB Guard** toma ese concepto y lo fortifica:
* Encripta la llave interna y valida el hardware matriz mediante llamadas nativas directas al Kernel de Windows (P/Invoke a `kernel32.dll`), sin dependencias pesadas como WMI.
* No se deja burlar por desconexiones agresivas simuladas; espera 0.5 segundos asíncronos para permitirle a Windows asimilar las interrupciones IRQ de hardware del USB.
* Posee un Desarme de Emergencia; Si accidentalmente borras todas tus bases de llaves, abortará el disparo para evitar bloqueos perpetuos (Auto-secuestro).

## Contribuciones

¿Quieres mejorar USB Guard? ¡Eres bienvenido! Por favor revisa nuestra guía de [CONTRIBUTING.md](CONTRIBUTING.md).

## Seguridad

La seguridad es mi prioridad. Si encuentras una vulnerabilidad, por favor consulta nuestra política en [SECURITY.md](SECURITY.md).

## Créditos y Licencia

Creado y mantenido por **[Mateo Valencia Ardila](https://github.com/fttmatteo)**.

Este software operada bajo la **Licencia MIT**. Eres libre de distribuir, editar o utilizar esta base arquitectónica incluso para desarrollo comercial bajo la principal condición obligatoria de preservar explícitamente los párrafos de derechos de autor y reconocimiento a Mateo Valencia como creador matriz. (Lee el documento `LICENSE` para más instrucciones legales).

