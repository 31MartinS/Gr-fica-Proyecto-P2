using System;
using System.Drawing;

public class Enemigo
{
    public PointF Posicion { get; private set; }
    private int Salud;
    private int SaludMax;
    private float Velocidad;
    private Image _imagen;



    public int getSalud()
    {
        return Salud;
    }

    public Enemigo(PointF posicion, int salud , Image imagen, int saludMax)
    {
        Posicion = posicion;
        Salud = salud;
        Velocidad = 2.0f;
        _imagen = imagen;
        SaludMax = saludMax;
    }

    public void Actualizar(PointF posicionDelJugador)
    {
        // Perseguir al jugador
        float dx = posicionDelJugador.X - Posicion.X;
        float dy = posicionDelJugador.Y - Posicion.Y;
        float distancia = (float)Math.Sqrt(dx * dx + dy * dy);

        if (distancia > 1)
        {
            Mover(dx / distancia * Velocidad, dy / distancia * Velocidad);
        }
    }

    public void Mover(float offsetX, float offsetY)
    {
        Posicion = new PointF(Posicion.X + offsetX, Posicion.Y + offsetY);
    }

    public void Dibujar(Graphics g)
    {
        // Dibujar la imagen del enemigo
        g.DrawImage(_imagen, Posicion.X - 15, Posicion.Y - 15, 30, 30);

        // Calcular el porcentaje de salud restante
        float porcentajeSalud = (float)Salud / SaludMax;

        // Definir el tamaño y la posición de la barra de vida
        int barraAncho = 30;
        int barraAlto = 5;
        int barraX = (int)Posicion.X - barraAncho / 2;
        int barraY = (int)Posicion.Y - 20;

        // Dibujar el fondo de la barra de vida (rojo)
        g.FillRectangle(Brushes.Red, barraX, barraY, barraAncho, barraAlto);

        // Dibujar la parte de la barra de vida que representa la salud restante (verde)
        g.FillRectangle(Brushes.Green, barraX, barraY, (int)(barraAncho * porcentajeSalud), barraAlto);

        // Dibujar el borde de la barra de vida (negro)
        g.DrawRectangle(Pens.Black, barraX, barraY, barraAncho, barraAlto);
    }

    public void RecibirDanio(int danio)
    {
        Salud -= danio;
    }

    public bool EstaMuerto()
    {
        return Salud <= 0;
    }

    public bool EstaColisionandoCon(PointF posicion)
    {
        float dx = Posicion.X - posicion.X;
        float dy = Posicion.Y - posicion.Y;
        float distancia = (float)Math.Sqrt(dx * dx + dy * dy);
        return distancia < 15; // Ajustar el radio de colisión
    }
}
