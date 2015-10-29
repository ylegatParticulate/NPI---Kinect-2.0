hola hacemos una wiki :) en español.
1. How does kinect work --> understand
2. kinect detects person
3. draw person to screen
4. fixed position (interaction person <--> object)
5. movement
6. gesture recognition
7. Problems we had

### 1 COMO FUNCIONA KINECT
En Kinect hay un sensor que detecta personas por sus partes del cuerpo. En Kinect hay una parte que trabaja detectando cuerpos y otra que muestra en la pantalla el resultado de las operaciones que se realiza con el cuerpo detectado. 

--
#### 1.1
Utilizamos C# porque es un punto intermedio entre los lenguajes que ya conocíamos, otro motivo por el cual lo seleccionamos fue porque el lenguaje es orientado a objetos, esto quiere decir que la repartición de tareas nos iba suponer un coste menor. Ya que cada uno de los miembros podrìa hacerse cargo de un objeto distinto. 

--
### 2 COMO KINECT DETECTA PERSONAS 
Kinect usa a nivel de código lo que se llama “streams”, que son como capas que se encargan de distintos tipos de detecciones. Nosotros vamos a usar los depth stream, body stream y color stream, el depth stream lo usamos para medir la distancia a la que el usuario se encuentra del Kinect, el body stream lo usamos para detectar las partes del cuerpo de una o varias personas, el color stream lo usamos para mostrar lo capturado por la cámara. 
Nosotros vamos a detectar la parte de los brazos y la cabeza y los usaremos para comprobar si interactúan con objetos. 

--
### 3 COMO PINTAMOS A UNA PERSONA
La cámara de Kinect detecta las articulaciones y para pintarlas tenemos que transformarlas en elementos que puedan ser pintados, dichos elementos son los puntos y las líneas, así que tenemos que realizar la conversión. Para ello, haciendo uso de las funciones por defecto, definiremos elementos punto y línea basandose en las coordenadas de las articulaciones detectadas y para correctamente escalarlos a la pantalla usaremos el Coordinate Mapper, o Mapeador de Coordenadas. 

--
### 4 
#### 4.1 POSICIÓN FIJA
Usaremos una posición fija para controlar donde el usuario va a encontrarse para que correctamente pueda interactuar con Kinect. En nuestro caso nos hemos guiado por el depth stream para controlar la distancia a la que se encuentra del dispositivo y controlamos también que el usuario se encuentra el centro de la pantalla midiendo la distancia que hay desde su cabeza hasta un punto invisible que se encuentra en el centro de la pantalla. Para controlar que el usuario llegue hasta esa posición aparecera un texto indicativo en la pantalla informandole de hacia donde debe moverse para alcanzar la posición además de una silueta que cambiará de color cuando lo haga. Una vez se encuentra en la posición inicial usamos una cuenta atrás para que se mantenga la posición correcta hasta que empieze el juego. 

