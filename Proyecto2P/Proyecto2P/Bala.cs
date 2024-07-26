using System;
using System.Drawing;

namespace Proyecto2P
{
    public class Bala
    {
        private PointF _posicion;
        private PointF _direccion;
        private float _velocidad = 10f;

        public Bala(PointF posicion, PointF direccion)
        {
            _posicion = posicion;
            _direccion = direccion;
        }

        public void Actualizar()
        {
            _posicion.X += _direccion.X * _velocidad;
            _posicion.Y += _direccion.Y * _velocidad;
        }

        // Modificar para usar imagen en lugar de dibujar una elipse
        public void Dibujar(Graphics g, Image imagenBala)
        {
            g.DrawImage(imagenBala, _posicion.X - 5, _posicion.Y - 5, 10, 10);
        }

        public bool EstaFueraDePantalla(Size tamanoPantalla)
        {
            return _posicion.X < 0 || _posicion.Y < 0 || _posicion.X > tamanoPantalla.Width || _posicion.Y > tamanoPantalla.Height;
        }

        public bool EstaColisionandoCon(PointF posicion)
        {
            return Math.Sqrt(Math.Pow(_posicion.X - posicion.X, 2) + Math.Pow(_posicion.Y - posicion.Y, 2)) < 10;
        }
    }
}
