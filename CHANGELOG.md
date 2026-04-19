# Registro de Cambios

Todos los cambios notables en este proyecto serán documentados en este archivo.

## [1.2.2] - 2026-04-19

### Added
- Implementación de **Rate-Limiting exponencial** para proteger contra ataques de fuerza bruta en la pantalla de bloqueo.
- Estética **Glassmorphism** mejorada mediante llamadas directas a la API de Windows (Win32).
- Soporte para **múltiples idiomas** en la documentación inicial.
- Sistema de **automatización de versiones** con scripts de PowerShell.
- Documentación profesional bilingüe (README, Contributing, Security).

### Changed
- Migración de dependencias WMI a llamadas nativas **P/Invoke** de `kernel32.dll` para mejorar el rendimiento y reducir el consumo de RAM.
- Optimización del manejo de interrupciones de hardware (USB IRQ) para mayor estabilidad al desconectar dispositivos.

### Security
- Mejorada la validación de hardware mediante lectura directa de descriptores de dispositivo.
