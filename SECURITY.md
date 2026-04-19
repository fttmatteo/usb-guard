# Política de Seguridad / Security Policy

*[Read in English](SECURITY.en.md)*

## Versiones Soportadas (Supported Versions)

A continuación, se indican las ramas oficiales del proyecto de código abierto que en este momento reciben parches biológicos y correcciones arquitectónicas de seguridad de la red.

| Versión | ¿Soportada Activamente? |
| ------- | ------------------ |
| v1.1.x   | :white_check_mark: |
| Antiguas | :x:                |

## Alcance y Limitaciones Actuales (v1.0.x)

**USB Guard** es un software de grado de consumidor/pro-sumidor. Actualmente intercepta con éxito las ventanas en la Capa del Usuario mediante ganchos asíncronos y propiedades *TopMost* / *Taskbar Hiding*.

Ten en cuenta que *USB Guard no es un Antivirus, ni encripta el flujo de hardware físico del puerto C*. Además, se recomienda enfundar el programa con políticas globales de bloqueo desde el Administrador de Directivas de Grupo (GPO), ya que, teóricamente, usuarios avanzados con privilegios de `Administrador NT` podrían finalizar el proceso nativo forzadamente a través del *Administrador de Tareas* interrumpiendo P/Invoke C++. Por eso sugerimos elegir el "Bloqueo nativo de Windows" si tu ambiente de oficina es intrínsecamente hostil.

## Detectar una Vulnerabilidad (Reporting a Vulnerability)

La estructura de autenticación de **Fuerza Bruta** impide inyecciones automatizadas (Rate Limits). 
Aun así, si consideras haber encontrado la forma matemática de romper el Criptoalgoritmo local `System.Security.Cryptography`, por favor **NO CREES UN ISSUE PÚBLICO** que comprometa las instalaciones funcionales actuales. 

1. Envía un correo electrónico privado o mensaje directo al empaquetador del proyecto (**Mateo Valencia Ardila**) con el asunto `USB GUARD VULNERABILITY`.
2. Provee un PoC (Proof of Concept) o un código de inyección reproducible.
3. Trataremos de mitigar la brecha publicando un "Hotfix" de seguridad dentro del ciclo semanal posterior a la lectura formal del informe. Agradecemos inmensamente a quien detecte errores letales con ánimos de ayudar.

