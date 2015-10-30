#Tutorial

###Definiciones básicas

######Para pasar un Joint a un Punto y correctamete llevarlo al plano de dibujo:

```csharp
Point punto = joint.Scale(mapper);
```

```csharp
public static Point Scale(this Joint joint, CoordinateMapper mapper)
{
    Point point = new Point();

    ColorSpacePoint colorPoint = mapper.MapCameraPointToColorSpace(joint.Position);
    point.X = float.IsInfinity(colorPoint.X) ? 0.0 : colorPoint.X;
    point.Y = float.IsInfinity(colorPoint.Y) ? 0.0 : colorPoint.Y;

    return point;
}
```

######Dibujamos y "escalamos" fácilmente un punto en solo una línea 

```csharp
canvas.DrawPoint(handRight, _sensor.CoordinateMapper);
```

```csharp
public static void DrawPoint(this Canvas canvas, Joint joint, CoordinateMapper mapper)
{
    if (joint.TrackingState == TrackingState.NotTracked) return;

    Point point = joint.Scale(mapper);

    Ellipse ellipse = new Ellipse
    {
        Width = 20,
        Height = 20,
        Fill = new SolidColorBrush(Colors.Coral)
    };

    Canvas.SetLeft(ellipse, point.X - ellipse.Width / 2);
    Canvas.SetTop(ellipse, point.Y - ellipse.Height / 2);

    canvas.Children.Add(ellipse);
}
```

######De la misma forma hacemos con la línea

```csharp
canvas.DrawLine(head,SpineShoulderJoint, _sensor.CoordinateMapper);
```

```csharp
public static void DrawLine(this Canvas canvas, Joint first, Joint second, CoordinateMapper mapper)
{
    if (first.TrackingState == TrackingState.NotTracked || second.TrackingState == TrackingState.NotTracked) return;

    Point firstPoint = first.Scale(mapper);
    Point secondPoint = second.Scale(mapper);

    Line line = new Line
    {
        X1 = firstPoint.X,
        Y1 = firstPoint.Y,
        X2 = secondPoint.X,
        Y2 = secondPoint.Y,
        StrokeThickness = 8,
        Stroke = new SolidColorBrush(Colors.Firebrick)
    };

    canvas.Children.Add(line);
}
```

--

###Game States

######Es mucho más fácil controlar el estado del juego con enums

```csharp
enum GameState { Initial, ShowStart, Running };
```

--

###Estructura de control

######Creamos una estructura para controlar de forma fácil los componentes de nuestro juego

```csharp
struct WaveHand {
    public static Boolean isWaving;
    
    public Joint hand;
    public Joint elbow;
    
    public Boolean waveRight;
    public Boolean waveLeft;
    public int waveCount;
    public Stopwatch waveWatch;
}
```

--

###Controles de condiciones

######Para controlar el tiempo, usamos la clase preestablecida

```csharp
Stopwatch countdown;
```

######Control de las condiciones iniciales del juego

```csharp
void CheckInitialConditions(Joint head){            
    /*
      Controlamos la distancia al dispositivo con poco margen de error.
    */

		if (head.Position.Z > 1.1 && head.Position.Z < 1.31){
		  ...
		
		  /*
		    Controlamos la distancia X,Y respecto al centro de la pantalla.
		  */
		
  		double distance = Math.Sqrt(
  				Math.Pow(
  					headPoint.X > invisibleHeadPoint.X ?
  					invisibleHeadPoint.X - headPoint.X :
  					headPoint.X - invisibleHeadPoint.X
  				, 2)
  				 +
  				Math.Pow(
  					headPoint.Y > invisibleHeadPoint.Y ?
  					invisibleHeadPoint.Y - headPoint.Y :
  					headPoint.Y - invisibleHeadPoint.Y
  				, 2)
  			);
  
  			if (distance < 61){
  			  ...
  			}
  			...
    }
    ...
}
```

######Control durante el juego

```csharp
// Una vez que se ha iniciado el juego, controlamos solo la distancia a la que se encuenta la persona del sensor.

Boolean InGameConditions(Joint head){  //Check correct depth during the game
  	if(head.Position.Z < 1.0)
  		Countdown.Text = "Go Back";
  	else if(head.Position.Z > 1.35)
  		Countdown.Text = "Forward";
  	else
  		return true;	
  
  	return false;
}
```

--

###El juego

```csharp
/*
  Hay que destacar el ref en la definición del parámetro.
  Esto quiere decir que lo estamos pasando por referencia en lugar de por valor, como ocurre por defecto.
  Así podemos modificar el valor de las variables internas del struct.
*/

void WaveWorld(ref WaveHand waveHand){

    /*
      Controlamos que la mano se encuentre por encima del codo.
      También controlamos que el tiempo que haya pasado entre las transiciones sea menor a 1 segundo porque consideramos que es más natural.
    */
    
  	if (waveHand.hand.Position.Y > waveHand.elbow.Position.Y && waveHand.waveWatch.Elapsed.Seconds < 1){

          /*
              Controlamos ahora que la mano pase a derecha o a izquierda respecto al codo.
          */


			    if (waveHand.hand.Position.X > waveHand.elbow.Position.X){

				    if (!waveHand.waveRight){
				        waveHand.waveWatch.Reset();
				        waveHand.waveWatch.Start();

				        waveHand.waveRight = true;
				        waveHand.waveLeft = false;

				        waveHand.waveCount++;      
				    }
			    }
			    else{
				    if (!waveHand.waveLeft){
				        waveHand.waveWatch.Reset();
				        waveHand.waveWatch.Start();

				        waveHand.waveRight = false;
				        waveHand.waveLeft = true;

				        waveHand.waveCount++;
				    
				    }
			    }

			    if (waveHand.waveCount > 4) {
				    Countdown.Text = "Hello World!!";

				    WaveHand.isWaving = true;
			    }
		}
		else{
		    waveHand.waveRight = false;
		    waveHand.waveLeft = false;
		    waveHand.waveCount = 0;

		    if(!WaveHand.isWaving)
			    Countdown.Text = "Wave!!";

		    waveHand.waveWatch.Reset();

		    WaveHand.isWaving = false;
		}	
}
```

--

###La aplicación completa

```csharp
/*
  Según el estado en el que se encuentre el juego, lanzamos una función u otra.
*/

switch (actualState){
			case GameState.Initial:
				CheckInitialConditions(head);
			break;
			case GameState.ShowStart:
				if(countdown.Elapsed.Seconds == 3){
					countdown.Reset();

					actualState = GameState.Running;
				}
			break;
			case GameState.Running:
				if (InGameConditions(head)){
					waveRightHand.hand = handRight;
					waveRightHand.elbow = ElbowRightJoint;

					WaveWorld(ref waveRightHand);


					waveLeftHand.hand = handLeft;
					waveLeftHand.elbow = ElbowLeftJoint;

					WaveWorld(ref waveLeftHand);
				}
			break;	
			default:
      break;								
}
```
