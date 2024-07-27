using Proyecto2P;
using System.Drawing;
using System;
using System.Windows.Forms;

public class Boss
{
    public PointF Posicion { get; private set; }
    private float Velocidad;
    public int Salud { get; private set; }
    private Image[] AnimacionCaminar;
    private Image[] AnimacionAtaque;
    private Image[] AnimacionMuerte;
    private int frameActual;
    private bool estaAtacando;
    private bool estaMuerto;
    private Timer _timerAnimacion;


    private Form1 _form; // Referencia al formulario principal

    public Boss(PointF posicionInicial, float velocidad, int salud, Image[] animacionCaminar, Image[] animacionAtaque, Image[] animacionMuerte, Form1 form)
    {
        Posicion = posicionInicial;
        Velocidad = velocidad;
        Salud = salud;
        AnimacionCaminar = animacionCaminar;
        AnimacionAtaque = animacionAtaque;
        AnimacionMuerte = animacionMuerte;
        frameActual = 0;
        estaAtacando = false;
        estaMuerto = false;
        _form = form;

        _timerAnimacion = new Timer();
        _timerAnimacion.Interval = 100;
        _timerAnimacion.Tick += (sender, e) => AvanzarFrame();
        _timerAnimacion.Start();
    }

    private void AvanzarFrame()
    {
        if (estaMuerto)
        {
            frameActual++;
            if (frameActual >= AnimacionMuerte.Length)
            {
                frameActual = AnimacionMuerte.Length - 1; // Mantener el último frame de la animación de muerte
            }
        }
        else if (estaAtacando)
        {
            frameActual++;
            if (frameActual >= AnimacionAtaque.Length)
            {
                frameActual = 0;
                estaAtacando = false;
            }
        }
        else
        {
            frameActual++;
            if (frameActual >= AnimacionCaminar.Length)
            {
                frameActual = 0;
            }
        }
    }

    public void Actualizar(PointF posicionJugador)
    {
        if (!estaMuerto)
        {
            float dx = posicionJugador.X - Posicion.X;
            float dy = posicionJugador.Y - Posicion.Y;
            float distancia = (float)Math.Sqrt(dx * dx + dy * dy);

            if (!estaAtacando && distancia > 1)
            {
                Posicion = new PointF(Posicion.X + dx / distancia * Velocidad, Posicion.Y + dy / distancia * Velocidad);
            }

            // Iniciar ataque si está suficientemente cerca del jugador
            if (distancia < 30) // Ajustar el rango de ataque
            {
                Atacar(posicionJugador);
            }
        }
    }

    public void Atacar(PointF posicionJugador)
    {
        if (!estaMuerto && !estaAtacando)
        {
            estaAtacando = true;
            frameActual = 0;
            // Lógica de ataque: daño al jugador si está en el radio de ataque
            float dx = posicionJugador.X - Posicion.X;
            float dy = posicionJugador.Y - Posicion.Y;
            float distancia = (float)Math.Sqrt(dx * dx + dy * dy);

            if (distancia < 30) // Ajustar el radio de ataque
            {
                // Hacer daño al jugador
                _form.AplicarDanioJugador(-30);
            }
        }
    }

    public bool EstaAtacando()
    {
        return estaAtacando;
    }

    public void Dibujar(Graphics g)
    {
        Image spriteActual;
        if (estaMuerto)
        {
            spriteActual = AnimacionMuerte[Math.Min(frameActual, AnimacionMuerte.Length - 1)];
        }
        else if (estaAtacando)
        {
            spriteActual = AnimacionAtaque[Math.Min(frameActual, AnimacionAtaque.Length - 1)];
        }
        else
        {
            spriteActual = AnimacionCaminar[Math.Min(frameActual, AnimacionCaminar.Length - 1)];
        }

        g.DrawImage(spriteActual, Posicion.X - spriteActual.Width / 2, Posicion.Y - spriteActual.Height / 2);
    }

    public void RecibirDanio(int danio)
    {
        if (!estaMuerto)
        {
            Salud -= danio;
            if (Salud <= 0)
            {
                estaMuerto = true;
                frameActual = 0;
            }
        }
    }

    public bool EstaMuerto()
    {
        return estaMuerto;
    }

    public bool EstaColisionandoCon(PointF posicion)
    {
        if (estaMuerto) return false;

        float dx = Posicion.X - posicion.X;
        float dy = Posicion.Y - posicion.Y;
        float distancia = (float)Math.Sqrt(dx * dx + dy * dy);
        return distancia < 20; // Ajustar el radio de colisión
    }
}
