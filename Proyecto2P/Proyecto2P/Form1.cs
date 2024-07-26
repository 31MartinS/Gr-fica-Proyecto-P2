using Proyecto2P;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace Proyecto2P
{
    public partial class Form1 : Form
    {
        public static int JugadorSalud { get; set; }
        private Timer _temporizador;
        private List<Bala> _balas;
        private List<Enemigo> _enemigos;
        private Random _aleatorio;
        private int _temporizadorDeAparicionDeEnemigos;
        private int _incrementoDeSaludDeEnemigos;
        private int _saludDelJugador;
        private PointF _posicionDelJugador;
        private float _rotacionDelJugador;
        private Boss _boss;
        private bool _bossAparecido;

        // Agregar variables para las imágenes
        private Image _imagenJugador;
        private Image _imagenEnemigo;
        private Image _imagenBala;
        private Image _imagenFondo;

        private PointF _mousePosition;

        // Agregar instancia de ScoreManager
        private ScoreManager _scoreManager;

        // Variable para controlar el estado de movimiento del jugador
        private bool _puedeMoverse;

        // Variables del mensaje del jefe
        private bool _mostrarMensajeBoss;
        private Timer _temporizadorMensajeBoss;
        private int _duracionMensajeBoss; // Duración en milisegundos
        private int saludJefe;

        public Form1()
        {
            InitializeComponent();

            // Inicializar variables
            _temporizador = new Timer();
            _temporizador.Interval = 16;
            _temporizador.Tick += Actualizar;

            _balas = new List<Bala>();
            _enemigos = new List<Enemigo>();
            _aleatorio = new Random();
            _temporizadorDeAparicionDeEnemigos = 0;
            _incrementoDeSaludDeEnemigos = 0;
            _saludDelJugador = 100;
            JugadorSalud = _saludDelJugador;
            _bossAparecido = false;
            _boss = null;
            saludJefe = 200;

            // Inicializar ScoreManager
            _scoreManager = new ScoreManager();

            // Inicializar variables del mensaje del jefe
            _mostrarMensajeBoss = false;
            _duracionMensajeBoss = 3000; // 3 segundos
            _temporizadorMensajeBoss = new Timer();
            _temporizadorMensajeBoss.Interval = _duracionMensajeBoss;
            _temporizadorMensajeBoss.Tick += (s, e) => _mostrarMensajeBoss = false;

            // Definir el directorio base para las imágenes
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string imagesPath = Path.Combine(basePath, "src");

            // Cargar imágenes usando rutas relativas
            try
            {
                _imagenJugador = Image.FromFile(Path.Combine(imagesPath, "tile_0084.png"));
                _imagenEnemigo = Image.FromFile(Path.Combine(imagesPath, "tile_0121.png"));
                _imagenBala = Image.FromFile(Path.Combine(imagesPath, "tile_0113.png"));
                _imagenFondo = Image.FromFile(Path.Combine(imagesPath, "background.png"));
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show($"Error al cargar la imagen: {ex.Message}");
                return;
            }

            // Ajustar el tamaño del formulario al tamaño del fondo
            this.ClientSize = new Size(_imagenFondo.Width, _imagenFondo.Height);

            // Inicializar la variable de estado de movimiento del jugador
            _puedeMoverse = true;

            // Iniciar el temporizador
            _temporizador.Start();

            // Configurar el formulario
            DoubleBuffered = true;
            Paint += AlDibujar;
            MouseMove += AlMoverElMouse;
            MouseDown += AlPresionarMouse;

            // Posición inicial del jugador
            _posicionDelJugador = new PointF(ClientSize.Width / 2, ClientSize.Height / 2);
        }

        private void AlDibujar(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.CornflowerBlue);

            // Dibujar el fondo
            g.DrawImage(_imagenFondo, 0, 0, _imagenFondo.Width, _imagenFondo.Height);

            if (_saludDelJugador > 0)
            {
                // Dibujar al jugador con imagen
                g.TranslateTransform(_posicionDelJugador.X, _posicionDelJugador.Y);
                g.RotateTransform(_rotacionDelJugador);
                g.DrawImage(_imagenJugador, -15, -15, 30, 30);
                g.ResetTransform();

                // Dibujar las balas con imagen
                foreach (var bala in _balas)
                {
                    bala.Dibujar(g, _imagenBala);
                }

                // Dibujar los enemigos con imagen
                foreach (var enemigo in _enemigos)
                {
                    enemigo.Dibujar(g, _imagenEnemigo);
                }

                // Dibujar HUD
                g.DrawString($"Vida: {_saludDelJugador}", new Font("Arial", 16), Brushes.White, 10, 10);
                g.DrawString($"Puntos: {_scoreManager.GetScore()}", new Font("Arial", 16), Brushes.White, 10, 30);
            }
            else
            {
                // Dibujar mensaje de "You Died" con "Arial Black" o "Impact"
                Font font = new Font("Arial Black", 48, FontStyle.Bold);
                SizeF textSize = g.MeasureString("YOU DIED", font);
                float x = (ClientSize.Width - textSize.Width) / 2;
                float y = (ClientSize.Height - textSize.Height) / 2;
                g.DrawString("YOU DIED", font, Brushes.Red, x, y);
            }

            // Dibujar mensaje del jefe
            if (_mostrarMensajeBoss)
            {
                Font font = new Font("Arial Black", 36, FontStyle.Bold);
                string mensaje = "Un jefe ha aparecido";
                SizeF textSize = g.MeasureString(mensaje, font);
                float x = (ClientSize.Width - textSize.Width) / 2;
                float y = (ClientSize.Height - textSize.Height) / 2;

                // Dibujar borde blanco
                g.DrawString(mensaje, font, Brushes.White, x - 2, y - 2);
                g.DrawString(mensaje, font, Brushes.White, x + 2, y - 2);
                g.DrawString(mensaje, font, Brushes.White, x - 2, y + 2);
                g.DrawString(mensaje, font, Brushes.White, x + 2, y + 2);

                // Dibujar texto negro
                g.DrawString(mensaje, font, Brushes.Black, x, y);
            }

            // Dibujar la barra de vida del jefe
            if (_bossAparecido && _boss != null)
            {
                float healthPercentage = (float)_boss.Salud / 200f; // Suponiendo que 200 es la salud máxima
                int barWidth = (int)(ClientSize.Width * healthPercentage);
                g.FillRectangle(Brushes.Green, 0, ClientSize.Height - 20, barWidth, 20);
                g.DrawRectangle(Pens.Black, 0, ClientSize.Height - 20, ClientSize.Width, 20);
            }

            if (_bossAparecido)
            {
                if (_boss != null)
                {
                    _boss.Dibujar(g);
                }
            }
        }

        private void AlMoverElMouse(object sender, MouseEventArgs e)
        {
            // Actualizar la posición del mouse
            _mousePosition = e.Location;

            // Calcular la rotación del jugador
            float dx = e.X - _posicionDelJugador.X;
            float dy = e.Y - _posicionDelJugador.Y;
            _rotacionDelJugador = (float)(Math.Atan2(dy, dx) * 180.0 / Math.PI);
        }

        private void AlPresionarMouse(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Crear una nueva bala
                float dx = e.X - _posicionDelJugador.X;
                float dy = e.Y - _posicionDelJugador.Y;
                float longitud = (float)Math.Sqrt(dx * dx + dy * dy);
                _balas.Add(new Bala(_posicionDelJugador, new PointF(dx / longitud, dy / longitud)));
            }
            else if (e.Button == MouseButtons.Right)
            {
                // Alternar el estado de movimiento del jugador
                _puedeMoverse = !_puedeMoverse;
            }
        }

        private void Actualizar(object sender, EventArgs e)
        {
            // Actualizar balas
            for (int i = _balas.Count - 1; i >= 0; i--)
            {
                _balas[i].Actualizar();
                if (_balas[i].EstaFueraDePantalla(ClientSize))
                {
                    _balas.RemoveAt(i);
                }
            }

            // Generar enemigos
            _temporizadorDeAparicionDeEnemigos++;
            if (_temporizadorDeAparicionDeEnemigos > 60 && !_bossAparecido)
            {
                GenerarEnemigo();
                _temporizadorDeAparicionDeEnemigos = 0;
            }

            // Actualizar enemigos
            for (int i = _enemigos.Count - 1; i >= 0; i--)
            {
                _enemigos[i].Actualizar(_posicionDelJugador);
                if (_enemigos[i].EstaMuerto())
                {
                    _enemigos.RemoveAt(i);
                    _scoreManager.AddPoints(5); // Añadir puntos al eliminar un enemigo
                }
                else if (_enemigos[i].EstaColisionandoCon(_posicionDelJugador))
                {
                    _saludDelJugador -= 10;
                    JugadorSalud = _saludDelJugador;
                    _enemigos.RemoveAt(i);
                }
            }

            // Comprobar colisiones entre balas y enemigos
            for (int i = _balas.Count - 1; i >= 0; i--)
            {
                for (int j = _enemigos.Count - 1; j >= 0; j--)
                {
                    if (_balas[i].EstaColisionandoCon(_enemigos[j].Posicion))
                    {
                        _enemigos[j].RecibirDanio(10);
                        _balas.RemoveAt(i);
                        break;
                    }
                }
            }

            // Comprobar colisiones entre balas y el jefe
            if (_bossAparecido && _boss != null)
            {
                if (_boss.EstaMuerto())
                {
                    // Eliminar el jefe si está muerto
                    _boss = null;
                    _bossAparecido = false;
                    _enemigos.Clear(); // Desaparecer enemigos
                }
                else
                {
                    for (int i = _balas.Count - 1; i >= 0; i--)
                    {
                        if (_balas[i].EstaColisionandoCon(_boss.Posicion))
                        {
                            _boss.RecibirDanio(10);
                            _balas.RemoveAt(i);
                            break;
                        }
                    }
                }
            }

            // Dibujar la barra de vida del jefe
            if (_bossAparecido && _boss != null)
            {
                saludJefe = _boss.Salud; // Actualizar la salud del jefe
            }

            // Detener el juego si la salud del jugador es 0
            if (_saludDelJugador <= 0)
            {
                _temporizador.Stop();
            }

            // Mover el jugador si está permitido
            if (_puedeMoverse)
            {
                MoverJugador();
            }

            if (_scoreManager.GetScore() >= 100 && !_bossAparecido)
            {
                _bossAparecido = true;
                _enemigos.Clear(); // Desaparecer enemigos

                // Mostrar mensaje del jefe
                _mostrarMensajeBoss = true;
                _temporizadorMensajeBoss.Start();

                // Cargar animaciones del jefe
                string bossPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ORC");
                Image[] animacionCaminar = {
            Image.FromFile(Path.Combine(bossPath, "Idel", "01.png")),
            Image.FromFile(Path.Combine(bossPath, "Idel", "02.png")),
            Image.FromFile(Path.Combine(bossPath, "Idel", "03.png")),
            Image.FromFile(Path.Combine(bossPath, "Idel", "04.png"))
        };
                Image[] animacionAtaque = {
            Image.FromFile(Path.Combine(bossPath, "ATTACK", "01.png")),
            Image.FromFile(Path.Combine(bossPath, "ATTACK", "02.png")),
            Image.FromFile(Path.Combine(bossPath, "ATTACK", "03.png")),
            Image.FromFile(Path.Combine(bossPath, "ATTACK", "04.png")),
            Image.FromFile(Path.Combine(bossPath, "ATTACK", "05.png")),
            Image.FromFile(Path.Combine(bossPath, "ATTACK", "06.png")),
            Image.FromFile(Path.Combine(bossPath, "ATTACK", "07.png")),
            Image.FromFile(Path.Combine(bossPath, "ATTACK", "08.png")),
            Image.FromFile(Path.Combine(bossPath, "ATTACK", "09.png"))
        };
                Image[] animacionMuerte = {
            Image.FromFile(Path.Combine(bossPath, "dead", "01.png")),
            Image.FromFile(Path.Combine(bossPath, "dead", "02.png")),
            Image.FromFile(Path.Combine(bossPath, "dead", "03.png")),
            Image.FromFile(Path.Combine(bossPath, "dead", "04.png")),
            Image.FromFile(Path.Combine(bossPath, "dead", "05.png")),
            Image.FromFile(Path.Combine(bossPath, "dead", "06.png")),
            Image.FromFile(Path.Combine(bossPath, "dead", "07.png")),
            Image.FromFile(Path.Combine(bossPath, "dead", "08.png")),
            Image.FromFile(Path.Combine(bossPath, "dead", "09.png"))
        };

                _boss = new Boss(new PointF(ClientSize.Width / 2, ClientSize.Height / 2), 4.0f, 200, animacionCaminar, animacionAtaque, animacionMuerte);
            }

            if (_bossAparecido)
            {
                if (_boss != null)
                {
                    _boss.Actualizar(_posicionDelJugador, _enemigos);
                    if (_boss.EstaColisionandoCon(_posicionDelJugador))
                    {
                        _saludDelJugador -= 40;
                        JugadorSalud = _saludDelJugador;
                    }
                }
            }

            Invalidate();
        }


        private void MoverJugador()
        {
            float dx = _mousePosition.X - _posicionDelJugador.X;
            float dy = _mousePosition.Y - _posicionDelJugador.Y;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);
            float speed = Math.Min(distance / 10.0f, 5.0f);

            if (distance > 1)
            {
                _posicionDelJugador.X += dx / distance * speed;
                _posicionDelJugador.Y += dy / distance * speed;

                // Limitar la posición del jugador dentro de los límites del fondo
                _posicionDelJugador.X = Math.Max(0, Math.Min(ClientSize.Width, _posicionDelJugador.X));
                _posicionDelJugador.Y = Math.Max(0, Math.Min(ClientSize.Height, _posicionDelJugador.Y));
            }
        }

        private void GenerarEnemigo()
        {
            PointF posicion = new PointF(_aleatorio.Next(ClientSize.Width), _aleatorio.Next(ClientSize.Height));
            _incrementoDeSaludDeEnemigos++;
            _enemigos.Add(new Enemigo(posicion, _incrementoDeSaludDeEnemigos));
        }

        public static void AjustarSaludJugador(int cantidad)
        {
            JugadorSalud += cantidad;
            if (JugadorSalud < 0) JugadorSalud = 0;
        }
    }
}
