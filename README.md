# 🧠 TFG_Agentes_RL

Este proyecto consiste en la implementación de un agente que aprende a escapar de una serie de niveles en un entorno 3D utilizando técnicas de Deep Reinforcement Learning (DRL). El proyecto ha sido desarrollado en Unity y con la ayuda de la herramienta Unity ML-Agents Toolkit. Si se desea conocer más información sobre esta herramienta, se puede consultar su documentación oficial [aquí](https://github.com/Unity-Technologies/ml-agents). El entrenamiento se ha realizado empleando el algoritmo Proximal Policy Optimization (PPO).

## 🎯 Objetivo del proyecto

El objetivo principal ha sido desarrollar un agente capaz de aprender por sí mismo a superar diferentes niveles con dificultad creciente, mediante la interacción con el entorno y el aprendizaje guiado por recompensas.

Además del agente principal, se han creado dos agentes derivados para explorar variantes en el diseño de recompensas y observaciones, permitiendo analizar:

- El impacto de nuevas recompensas en el comportamiento del agente.
- El efecto en el rendimiento al proporcionar más observaciones.

## 📦 Estructura del proyecto

En la carpeta Assets se encuentran, entre otras cosas, los prefabs creados (agentes y niveles) y la carpeta Scripts con el código de cada uno de los agentes. Se proporcionan además las carpetas results, que contiene los modelos entrenados en este proyecto, y la carpeta config que contiene el archivo de configuración utilizado.

## 🛠️ Tecnologías utilizadas

Estas son las tecnologías y la versión utilizadas. Cabe destacar que para mayor aislación del proyecto y las versiones, se ha hecho uso de un virtual environment (venv). Se puede ver información acerca de esto [aquí](https://docs.python.org/3/library/venv.html).
- Unity 2022.3.48f1
- ML-Agents dentro de Unity (última versión)
- Python 3.9.13
- mlagents 0.30.0 con pip en Python (debería servir la última versión)
- torch 2.4.1 con pip en Python (debería servir la última versión)
- tensorboard 2.18 con pip en Python (debería servir la última versión)


Además, para evitar la aparición de ciertos errores fue útil la instalación de estos tres paquetes:
- pip3 install torch torchvision torchaudio
- pip install protobuf==3.20.3
- pip install packaging

## 🚀 Abrir el proyecto

Una vez se tiene la carpeta principal de este repositorio se puede abrir el proyecto desde Unity Hub. Unity creará las carpetas necesarias para regenerar el proyecto entero. 
**Importante**. Si al abrir el proyecto no se muestra automáticamente la escena principal (deberían poder verse los cinco niveles uno al lado del otro), se debe entrar a Assets --> Scenes y abrir la única que hay.
