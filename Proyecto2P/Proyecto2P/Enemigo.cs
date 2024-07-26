using System;
using System.Drawing;

public class Enemigo
{
    public PointF Posicion { get; private set; }
    private int Salud;
    private float Velocidad;

    public Enemigo(PointF posicion, int salud)
    {
        Posicion = posicion;
        Salud = salud;
        Velocidad = 2.0f; // Ajustar la velocidad del enemigo
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

    public void Dibujar(Graphics g, Image imagen)
    {
        g.DrawImage(imagen, Posicion.X - 15, Posicion.Y - 15, 30, 30);
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
