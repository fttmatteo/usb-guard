# Guía de Contribución para USB Guard

*[Read in English](CONTRIBUTING.en.md)*

¡Gracias por mostrar interés en desarrollar y mejorar **USB Guard**! Como un proyecto de código abierto, dependemos de mentes brillantes y analíticas para robustecer la arquitectura que nos protege a todos.

## Entorno de Desarrollo

Para comenzar a contribuir al código, asegúrate de cumplir con los siguientes requisitos:
1. Microsoft Visual Studio 2022.
2. SDK de **.NET 10.0** (Carga de trabajo para Aplicaciones de Escritorio / WPF).
3. Conocimiento estándar intermedio de la arquitectura **MVVM** (Model-View-ViewModel) y librerías del `CommunityToolkit.Mvvm`.

## Rutas Críticas (Endpoints de Intervención Recomendada)

Si deseas trabajar en vulnerabilidades o ampliar el espectro, recomendamos investigar estas ramas troncales del proyecto para realizar *Pull Requests*:

* **Vulnerabilidades y Hooks Globales:** `USBGuard.Core\Interop`. Actualmente el programa anula entradas estándar a la pantalla de bloqueo (F4, Esc, AltTab). Se buscan implementaciones seguras C++ Unmanaged de P/Invoke `SetWindowsHookEx` para congelar el teclado por completo previniendo inyecciones físicas profundas.
* **Integración al GINA o Credential Provider:** Módulo exploratorio. Escalar USBGuard más allá de correr en capa de usuario para inyectarlo como un Proveedor de Credenciales Nativo debajo del LockScreen de Windows OS, lo que impediría cerrar el proceso por Administrador de Tareas.

## Reglas Básicas

1. **Mantén el Zero-Trust:** Cualquier propuesta que incluya el acceso o armado del sistema operativo debe estar programada considerando que *el usuario final olvidará los pasos o se equivocará*. Provee caminos asíncronos para evitar "Auto-Bloqueos".
2. **Convenciones Visuales:** Cualquier rediseño debe fluir a través de los estilos de `WPF-UI`. Evita sobrecargar colores rígidos y prioriza componentes con opacidades para fusionar visualmente tu componente con fondos de escritorio nativos.
3. No envíes PRs rompiendo la estructura de licenciamiento (Atribución requerida) de Mateo Valencia Ardila.

## Proceso de Pull Request

1. **Crear una rama**: `feat/tu-funcion` o `fix/tu-correccion`.
2. **Commit de cambios**: Asegúrate de que las pruebas pasen localmente.
3. **Crear el PR**: Describe los cambios, el "Por qué" y cualquier cambio disruptivo (breaking change).
4. **Revisión**: Espera el feedback del mantenedor.

## Fork

Para iniciar, haz un Fork, abre tu rama (`git checkout -b feature/MiInventoAsombroso`), inyecta tu código y sube tu historia (`git commit -m 'Hice esta locura'`). ¡Quedamos sumamente atentos!

## Checklist

- [ ] Mi código sigue las guías de estilo de este proyecto.
- [ ] He realizado una auto-revisión de mi propio código.
- [ ] He comentado mi código, especialmente en áreas difíciles de entender.
- [ ] He realizado los cambios correspondientes en la documentación.
- [ ] Mis cambios no generan nuevas advertencias (warnings).
- [ ] He añadido pruebas que demuestran que mi corrección es efectiva o que mi función funciona.

## Licencia
Al contribuir, aceptas que tus contribuciones estarán bajo la **Licencia MIT** del proyecto.