using System;
using System.Drawing;

namespace Proyecto2P
{
    public class Bala
    {
        private PointF _posicion;
        private PointF _direccion;
        private float _rotacion;
        private float _velocidad = 10f;



        public Bala(PointF posicion, PointF direccion)
        {
            _posicion = posicion;
            _direccion = direccion;
            _rotacion = (float)(Math.Atan2(direccion.Y, direccion.X) * 180.0 / Math.PI) + 90;
        }

        public void Actualizar()
        {
            _posicion.X += _direccion.X * _velocidad;
            _posicion.Y += _direccion.Y * _velocidad;
        }

        public void Dibujar(Graphics g, Image imagenBala)
        {

            // Guardar el estado actual de la transformación gráfica
            var estadoOriginal = g.Save();

            // Aplicar la transformación: traslación y rotación
            g.TranslateTransform(_posicion.X, _posicion.Y);
            g.RotateTransform(_rotacion);

            // Dibujar la imagen de la bala centrada en la posición actual
            g.DrawImage(imagenBala, -imagenBala.Width / 2, -imagenBala.Height / 2);

            // Restaurar el estado gráfico original
            g.Restore(estadoOriginal);
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
