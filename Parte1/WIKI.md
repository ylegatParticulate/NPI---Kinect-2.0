### 1 ¿CÓMO FUNCIONA KINECT?
Kinect es un sensor de captación que reconoce entornos mediantes diversos sensores. Entre ellos tenemos detectores de sonido y de movimiento, además de infrarrojos y una cámara de vídeo.
Para trabajar con Kinect usaremos Visual Studio, lo que quiere decir que trabajaremos por un lado con detectores y por otro con el resultado de las operaciones realizadas con lo detectado. 


#### 1.1 ¿Qué lenguaje usamos?
Utilizamos C# porque es un punto intermedio entre los lenguajes que ya conocíamos, otro motivo por el cual lo seleccionamos fue porque el lenguaje es orientado a objetos, esto quiere decir que la repartición de tareas nos iba suponer un coste menor. Esto se debe a que cada uno de los miembros puede hacerse cargo de un objeto distinto. 

--
### 2 ¿CÓMO KINECT DETECTA PERSONAS?
Kinect usa a nivel de código lo que se llama “streams”, que son como capas de abstracción que se encargan de distintos tipos de detecciones. Nosotros vamos a usar los depth stream, body stream y color stream. El depth stream lo usamos para medir la distancia a la que el usuario se encuentra del Kinect, el body stream lo usamos para detectar las partes del cuerpo de una o varias personas y el color stream lo usamos para las funciones de cámara. 
Nosotros vamos a detectar la parte de los brazos y la cabeza, y los usaremos para comprobar si interactúan con objetos virtuales. 

--
### 3 ¿CÓMO DIBUJAMOS UNA PERSONA?
De las personas, Kinect detecta las articulaciones, lo que hace que para poder mostrar la persona capturada, es necesario "pintarla" por pantalla, y para pintarla tenemos que transformarla en elementos que puedan ser pintados, dichos elementos son los puntos y las líneas, esto quiere decir que hay que realizar la conversión. Para ello, haciendo uso de las funciones preestablecidas, definiremos elementos punto y línea basándose en las coordenadas de las articulaciones detectadas y para correctamente escalarlos a la pantalla usaremos el Coordinate Mapper, o Mapeador de Coordenadas. El resultado final es una especie de esqueleto de puntos y líneas.

--
### 4 POSICIÓN FIJA
Usaremos una posición fija para controlar donde el usuario va a encontrarse inicialmente para que correctamente pueda interactuar con Kinect. En nuestro caso nos hemos guiado por el depth stream para controlar la distancia a la que se encuentra del dispositivo, a lo que hay que añadirle que el usuario se encuentre en el centro de la pantalla, para ello medimos la distancia que hay desde su cabeza hasta un punto invisible que se encuentra en el centro de ésta. Para controlar que el usuario llegue hasta dicha posición, aparecerá un texto indicativo en la pantalla informándole de hacia donde debe moverse para alcanzar la posición, además de una silueta que cambiará de color una vez alcanzado ésta. Cuando ya se encuentra en la posición inicial usamos una cuenta atrás para que no se desplace del sitio hasta que empiece el juego.

--
### 5 MOVIMIENTO
El movimiento se detecta comparando la variación de posición de elementos detectados.
En nuestro proyecto hemos hecho uso de dicha variación de posición para comprobar si se ha realizado un gesto.

--
### 6 RECONOCIMIENTO DE GESTOS
Hasta ahora, el único gesto que hemos reconocido es el de un simple saludo con la mano. Para ello hay que comprobar que se cumplen varios pasos:
1. La mano tiene que estar por encima del codo.
2. La mano se mueve hacia la derecha o izquierda.
3. La mano se mueve hacia el sentido contrario.
4. Dicho movimiento debe de durar menos de un segundo en ir de un lado al otro.
5. Consideramos el gesto exitoso al realizarse 4 veces seguidas.

--
### 7 CÓDIGO
El código es una inspiración directa de [este tutorial](http://pterneas.com/2014/03/21/kinect-for-windows-version-2-hand-tracking/) de reconocimiento de manos. Quiere esto decir que hemos mantenido intacta algunas funciones para configurar y poner en marcha el Kinect y sus detectores, pero hemos modificado altamente, hasta el punto de reescribir el resto de código pese a que hemos mantenido los nombres o el código original en forma de comentarios. Por otro lado, el código puede ser altamente optimizado ya que se pueden reutilizar objetos tras cada iteración en lugar de volver a crearlos.
Para controlar el estado de la aplicación, hemos definido Enumeraciones (enum) con los distintos estados del juego: llegar a la posición inicial, transición entre la posición inicial y la ejecución, y la ejecución.
La **posición inicial** es un control más o menos estricto de la distancia al dispositivo y la situación respecto a la pantalla.
La **transición** simplemente da a entender al usuario que el juego va a comenzar.
Y la **ejecución**, es en sí el juego, aunque también llevará un control no estricto de la situación del jugador, deteniendo el juego si llega a distancias no deseadas.

--
### 8 INTERACCIÓN CON EL USUARIO
En nuestra aplicación hacemos uso de una silueta transparente y de una caja de texto para informar al usuario de la posición en la que debe estar, al principio del juego o si llega a distancias indeseadas en mitad de este, además de avisos en sí para que explícitamente sepa lo que tiene que hacer y el resultado de su acción.

--
### 9 PROBLEMAS Y SOLUCIONES
 - El primer problema que tuvimos fue el idioma, Alemán vs Español, la **solución**: hablar inglés. En ocasiones puntuales nos encontramos con cosas en alemán que requieren un intérprete.
 - Otro de los primeros problemas que a todos nos ha sido común tiene nombre y apellidos: Microsoft Windows (8.1 o 10). **Solución**: dado que no se consiguió usar Kinect con los drivers libres que hay para sistemas GNU/Linux, no quedaba otra que aguantarse en Windows.
 - A continuación nos han perseguido problemas técnicos con los ordenadores por desconocimiento de los requisitos para poder usar Kinect, desconocimiento de los requisitos necesarios para poder instalar los requisitos para poder usar Kinect, etc. Por otro lado hay que sumarle mal-instalaciones o instalaciones en directorios no comunes (no instalar en Archivos de Programa del disco C). La **solución** fue paciencia, instalar y desinstalar varias veces las cosas y finalmente indicar a Visual Studio la ruta de donde se encontraba su querido Microsoft.Kinect.dll.
 - Obligatoriamente hay que mencionar la falta de ejemplos de uso básico (fuera de los aparentemente básicos que se encuentran junto con el SDK) en internet, así como una simple interacción entre objetos virtuales y reales, si existen funciones para las colisiones, si existen funciones para el cálculo de distancias, si existe una forma entendible de crear objetos en el lienzo no a través del Windows Form Designer, etc. La **solución** a esto ha sido la implementación de todo aquello que no se encontraba (bien porque no era intuitivo el lugar al que pertenecía dicha función o bien por su inexistencia), o el cambiar de idea y realizar las cosas de forma aún más básica de lo que se había pensado. Ejemplo de esto:
 - Para la primera práctica pasar de una idea original de ejemplo básico: interaccionar con unos cubos a un simple gesto de saludo con la mano. No ha sido obvio que las articulaciones (Joint) tenían unas coordenadas completamente distintas (desde el centro de la pantalla) que las que tienen los puntos (Point).
 - No es obvio (y no lo hemos conseguido), el cambiar de color el bloque de texto una vez haya ocurrido algún suceso.
 - No era obvio que se usase el objeto Stopwatch para medir el tiempo ya que en la mayoría de lenguajes de programación se usa la clase de fecha (también existente en C# pero sin este uso o no se ha conseguido de esta forma).
 - No es obvio en absoluto como clonar objetos sin hacer uso de funciones.
 - Por defecto, las funciones pasan todos los objetos por valor (hacen una copia), en lugar de pasarlos por referencia, lo que hace que el código sea generalmente ineficiente si esto se desconoce, aunque se puede **solucionar** añadiendo **ref** tanto en la definición del parámetro como en el parámetro en sí enviado.

--
### 10 ENLACES USADOS
 - [Game Over](http://www.wired.com/images_blogs/gamelife/2010/07/249-artwork-focus.jpg)
 - [Círculo 1](http://www.polyvore.com/cgi/img-thing?.out=jpg&size=l&tid=23643390)
 - [Círculo 2](http://dmad.com/wp-content/uploads/2012/09/isolated-circle.png)
 - [Flecha 1](http://static1.squarespace.com/static/52306c87e4b048babe0d2faf/t/54ac12dbe4b0f3612b25ea39/1420563208228/curved_arrow-black.png)
 - [Flecha 2](http://richmonkey.co.uk/ebay/images/curved_arrow.png)