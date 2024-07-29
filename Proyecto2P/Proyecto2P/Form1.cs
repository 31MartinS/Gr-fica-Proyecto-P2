using Proyecto2P;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Media;
//using System.Runtime.InteropServices;


namespace Proyecto2P
{
    public partial class Form1 : Form
    {
        public int _saludDelJugador;
        public int JugadorDanio;
        public int JugadorSaludMax;
        private Timer _temporizador;
        private List<Bala> _balas;
        private List<Enemigo> _enemigos;
        private Random _aleatorio;
        private int _temporizadorDeAparicionDeEnemigos;
        private int _incrementoDeSaludDeEnemigos;
        private PointF _posicionDelJugador;
        private float _rotacionDelJugador;
        private Boss _boss;
        private bool _bossAparecido;

        public int _vidaMago;
        public int _vidaCaballero;
        public int _vidaEnano;
        public int _vidaAldeano;
        public int _danoMago;
        public int _danoCaballero;
        public int _danoEnano;
        public int _danoAldeano;

        public int reciveDano;


        // Agregar variables para las imágenes
        private Image _imagenJugador;
        private Image _imagenEnemigo;
        private Image _imagenBala;
        private Image _imagenFondo;
        private Image _imagenMira;
        private Image _corazon;
        private Image _medioCorazon;
        private Image _corazonVacio;
        private Image _espada;
        private Image _mago;
        private Image _caballero;
        private Image _enano;
        private Image _aldeano;
        private Image _original;



        private PointF _mousePosition;

        // Agregar instancia de ScoreManager
        private ScoreManager _scoreManager;

        // Variable para controlar el estado de movimiento del jugador
        private bool _puedeMoverse;

        // Variables del mensaje del jefe
        private bool _mostrarMensajeBoss;
        private Timer _temporizadorMensajeBoss;
        private int _duracionMensajeBoss; // Duración en milisegundos
        private int _derrotados;


        // Variables para los paneles de menus
        private Panel panelInicio;
        private Panel panelPausa;
        private Panel panelSeleccionPersonaje;
        private Panel panelGameOver;
        private Panel panelMejora;
        private bool inicio;
        private bool pausa;


        //Personaje
        private string personajeSeleccionado = "Mago";

        //Path relativo
        string basePath = AppDomain.CurrentDomain.BaseDirectory;


        //Musica
        private SoundPlayer _player;



        //Cinematica
        private bool _animacionIniciada;
        private PointF _posicionInicialJugador;
        private PointF _posicionFinalJugador;
        private bool _rotacionBloqueada;

        private Image _cofreCerrado;
        private Image _cofreMedioAbierto;
        private Image _cofreAbierto;
        private int _estadoCofre;
        private bool _animacionCofreIniciada;
        private Timer _temporizadorCofre;

        private Image _imagenExclamacion;
        private Timer _temporizadorExclamacion;
        private int _exclamacionTitileoContador;
        private bool _mostrarExclamacion;

        private int _puntosParaSiguienteBoss;
        private int _incrementoPuntosBoss;

        //Mover
        private bool _arribaPresionado;
        private bool _abajoPresionado;
        private bool _izquierdaPresionado;
        private bool _derechaPresionado;


        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(AlPresionarTecla);
            this.KeyUp += new KeyEventHandler(AlSoltarTecla);
            this.Load += (s, e) => BloquearCursor();
            this.FormClosing += (s, e) => LiberarCursor();


            // Inicializar variables
            _temporizador = new Timer();
            _temporizador.Interval = 16;
            _temporizador.Tick += Actualizar;

            _balas = new List<Bala>();
            _enemigos = new List<Enemigo>();
            _aleatorio = new Random();
            _temporizadorDeAparicionDeEnemigos = 0;
            _incrementoDeSaludDeEnemigos = 0;
            _bossAparecido = false;
            _boss = null;
            _derrotados = 0;

            _vidaMago = 100;
            _vidaCaballero = 150;
            _vidaEnano = 120;
            _vidaAldeano = 80;

            _danoMago = 15;
            _danoCaballero = 10;
            _danoEnano = 20;
            _danoAldeano = 5;

            JugadorSaludMax = _vidaMago;
            JugadorDanio = _danoMago;
            _saludDelJugador = JugadorSaludMax;
            reciveDano = 0; ;

            inicio = true;
            pausa = false;
            // Inicializar ScoreManager
            _scoreManager = new ScoreManager();

            // Inicializar variables del mensaje del jefe
            _mostrarMensajeBoss = false;
            _duracionMensajeBoss = 1000; // 2 segundos
            _temporizadorMensajeBoss = new Timer();
            _temporizadorMensajeBoss.Interval = _duracionMensajeBoss;
            _temporizadorMensajeBoss.Tick += (s, e) => _mostrarMensajeBoss = false;

            // Inicializar puntos para la aparición del jefe
            _puntosParaSiguienteBoss = 100;
            _incrementoPuntosBoss = 200;

            // Definir el directorio base para las imágenes
            string imagesPath = Path.Combine(basePath, "src");

            // Cargar imágenes usando rutas relativas
            try
            {
                _imagenEnemigo = Image.FromFile(Path.Combine(imagesPath, "tile_0121.png"));
                _imagenBala = Image.FromFile(Path.Combine(imagesPath, "tile_0113.png"));
                _imagenFondo = Image.FromFile(Path.Combine(imagesPath, "background.png"));
                _imagenMira = Image.FromFile(Path.Combine(imagesPath, "tile_0060.png"));
                _cofreCerrado = Image.FromFile(Path.Combine(imagesPath, "tile_0089.png"));
                _cofreMedioAbierto = Image.FromFile(Path.Combine(imagesPath, "tile_0090.png"));
                _cofreAbierto = Image.FromFile(Path.Combine(imagesPath, "tile_0091.png"));
                _imagenExclamacion = Image.FromFile(Path.Combine(imagesPath, "signo.png"));
                _corazon = Image.FromFile(Path.Combine(imagesPath, "Corazon.png"));
                _medioCorazon = Image.FromFile(Path.Combine(imagesPath, "MedioCorazon.png"));
                _corazonVacio = Image.FromFile(Path.Combine(imagesPath, "CorazonVacio.png"));
                _espada = Image.FromFile(Path.Combine(imagesPath, "espada.png"));

                _mago = Image.FromFile(Path.Combine(imagesPath, "tile_0084.png"));
                _caballero = Image.FromFile(Path.Combine(imagesPath, "tile_0097.png"));
                _enano = Image.FromFile(Path.Combine(imagesPath, "tile_0087.png"));
                _aldeano = Image.FromFile(Path.Combine(imagesPath, "tile_0088.png"));


            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show($"Error al cargar la imagen: {ex.Message}");
                return;
            }

            _imagenJugador = _mago;
            _original = _imagenJugador;


            string audioPath = Path.Combine(basePath, "musica", "AOG.wav");
            _player = new SoundPlayer(audioPath);

            // Reproducir la música en bucle
            _player.PlayLooping();

            // Ajustar el tamaño del formulario al tamaño del fondo
            this.ClientSize = new Size(_imagenFondo.Width, _imagenFondo.Height);

            // Bloqueo de tamaño de pantalla
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Configurar el formulario
            DoubleBuffered = true;
            Paint += AlDibujar;
            MouseMove += AlMoverElMouse;
            MouseDown += AlPresionarMouse;

            // Inicializar variables de animación
            _animacionIniciada = false;
            _posicionInicialJugador = new PointF(ClientSize.Width / 2, ClientSize.Height); // Desde abajo
            _posicionFinalJugador = new PointF(ClientSize.Width / 2, ClientSize.Height / 2); // Hasta el centro
            _rotacionBloqueada = true;
            _estadoCofre = 0;
            _animacionCofreIniciada = false;
            _temporizadorCofre = new Timer();
            _temporizadorCofre.Interval = 500;
            _temporizadorCofre.Tick += (s, e) => CambiarEstadoCofre();

            _temporizadorExclamacion = new Timer();
            _temporizadorExclamacion.Interval = 300;
            _temporizadorExclamacion.Tick += (s, e) => TitilarExclamacion();
            _exclamacionTitileoContador = 0;
            _mostrarExclamacion = false;

            // Inicializar paneles de menú
            InicializarPanelesDeMenu();
        }


        private void cambiarPersonaje()
        {
            // Definir el directorio base para las imágenes
            string imagesPath = Path.Combine(basePath, "src");

            try
            {
                switch (personajeSeleccionado)
                {
                    case "Mago":
                        _imagenJugador = _mago;
                        _imagenBala = Image.FromFile(Path.Combine(imagesPath, "tile_0113.png"));
                        JugadorSaludMax = _vidaMago;
                        _saludDelJugador = JugadorSaludMax;
                        JugadorDanio = _danoMago;
                        _original = _imagenJugador;

                        break;
                    case "Caballero":
                        _imagenJugador = _caballero;
                        _imagenBala = Image.FromFile(Path.Combine(imagesPath, "tile_0106.png"));
                        JugadorSaludMax = _vidaCaballero;
                        _saludDelJugador = JugadorSaludMax;
                        JugadorDanio = _danoCaballero;
                        _original = _imagenJugador;

                        break;
                    case "Enano":
                        _imagenJugador = _enano;
                        _imagenBala = Image.FromFile(Path.Combine(imagesPath, "tile_0118.png"));
                        JugadorSaludMax = _vidaEnano;
                        _saludDelJugador = JugadorSaludMax;
                        JugadorDanio = _danoEnano;
                        _original = _imagenJugador;

                        break;
                    case "Aldeano":
                        _imagenJugador = _aldeano;
                        _imagenBala = Image.FromFile(Path.Combine(imagesPath, "tile_0103.png"));
                        JugadorSaludMax = _vidaAldeano;
                        _saludDelJugador = JugadorSaludMax;
                        JugadorDanio = _danoAldeano;
                        _original = _imagenJugador;

                        break;
                    default:
                        throw new Exception("Personaje no reconocido");
                }

            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show($"Error al cargar la imagen: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }

        
        }
        
        //Cursos invisible
        private Cursor OcultarCursor()
        {
            Bitmap bitmap = new Bitmap(1, 1);
            IntPtr ptr = bitmap.GetHicon();
            Icon icon = Icon.FromHandle(ptr);
            return new Cursor(icon.Handle);
        }


        private void ReiniciarJuego()
        {
            // Detener el temporizador y ocultar los paneles
            _temporizador.Stop();
            panelGameOver.Visible = false;
            panelPausa.Visible = false;

            // Restablecer la salud del jugador
            _saludDelJugador = JugadorSaludMax;

            // Limpiar listas de balas y enemigos
            _balas.Clear();
            _enemigos.Clear();

            // Restablecer otras variables de estado
            _bossAparecido = false;
            _boss = null;
            _puntosParaSiguienteBoss = 100;
            _rotacionDelJugador = 0;
            _posicionDelJugador = _posicionInicialJugador;
            _scoreManager.Reset();
            _incrementoDeSaludDeEnemigos = 0;
            _temporizadorDeAparicionDeEnemigos = 0;
            _derrotados = 0;

            // Mostrar el panel de inicio
            panelInicio.Visible = true;
            this.Cursor = Cursors.Default;

            // Musica
            _player.PlayLooping();

        }



        private void InicializarPanelesDeMenu()
        {
            // Panel de inicio
            panelInicio = new Panel
            {
                Size = this.ClientSize,
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(100, 0, 0, 0),
                Visible = true
            };

            Label titulo = new Label
            {
                Text = "Bochi the rock",
                ForeColor = Color.White,
                Font = new Font("Arial Black", 24, FontStyle.Bold),
                AutoSize = true,
                Location = new Point((panelInicio.Width / 2) - 140, (panelInicio.Height / 2) - 100),
                BackColor = Color.FromArgb(0, 0, 0, 0),
            };

            Button btnIniciar = CreateStyledButton("Iniciar Juego", (panelInicio.Width / 2) - 75, (panelInicio.Height / 2) - 25);
            btnIniciar.Click += (s, e) => {
                panelInicio.Visible = false;
                cambiarPersonaje();
                this.Cursor = OcultarCursor();
                _puedeMoverse = false;
                _posicionDelJugador = _posicionInicialJugador;
                _animacionIniciada = true;
                _rotacionBloqueada = true;
                _temporizador.Start();
                BloquearCursor();
                inicio = false;

            };

            Button btnSalir = CreateStyledButton("Salir", (panelInicio.Width / 2) - 75, (panelInicio.Height / 2) + 95);
            btnSalir.Click += (s, e) => Application.Exit();

            Button btnSeleccionPersonaje = CreateStyledButton("Selección de Personaje", (panelInicio.Width / 2) - 75, (panelInicio.Height / 2) + 35);
            btnSeleccionPersonaje.Click += (s, e) => {
                panelSeleccionPersonaje.Visible = true;
                panelInicio.Visible = false;
            };

            //// TrackBar para el control del volumen
            //TrackBar volumenTrackBar = new TrackBar
            //{
            //    Location = new Point((panelInicio.Width / 2) - 75, (panelInicio.Height / 2) + 160),
            //    Size = new Size(150, 45),
            //    Minimum = 10000,
            //    Maximum = 1000000000,
            //    Value = 100000000 // Valor inicial
            //};
            //volumenTrackBar.Scroll += (s, e) =>
            //{
            //    VolumeControl.SetVolume(volumenTrackBar.Value);
            //};

            panelInicio.Controls.Add(titulo);
            panelInicio.Controls.Add(btnIniciar);
            panelInicio.Controls.Add(btnSeleccionPersonaje);
            panelInicio.Controls.Add(btnSalir);
            //panelInicio.Controls.Add(volumenTrackBar);
            this.Controls.Add(panelInicio);












            // Panel de selección de personaje
            panelSeleccionPersonaje = new Panel
            {
                Size = this.ClientSize,
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(200, 0, 0, 0),
                Visible = false
            };

            PictureBox Mago = new PictureBox
            {
                Image = _mago,
                Size = new Size(40, 40),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Location = new Point((panelSeleccionPersonaje.Width / 2) - 125, (panelSeleccionPersonaje.Height / 2) - 100)
            };
            Button btnMago = CreateStyledButton("Mago", (panelSeleccionPersonaje.Width / 2) - 75, (panelSeleccionPersonaje.Height / 2) - 100);
            btnMago.Click += (s, e) => {
                personajeSeleccionado = "Mago";
                panelSeleccionPersonaje.Visible = false;
                panelInicio.Visible = true;
            };
            Label atributosMago = new Label
            {
                Text = $"Vida: {_vidaMago} \nAtaque: {_danoMago}",
                ForeColor = Color.White,
                Font = new Font("Arial Black", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point((panelSeleccionPersonaje.Width / 2) + 80, (panelSeleccionPersonaje.Height / 2) - 100),
                BackColor = Color.FromArgb(0, 0, 0, 0),
            };


            PictureBox Caballero = new PictureBox
            {
                Image = _caballero,
                Size = new Size(40, 40),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Location = new Point((panelSeleccionPersonaje.Width / 2) - 125, (panelSeleccionPersonaje.Height / 2) - 30)
            };
            Button btnCaballero = CreateStyledButton("Caballero", (panelSeleccionPersonaje.Width / 2) - 75, (panelSeleccionPersonaje.Height / 2) - 30);
            btnCaballero.Click += (s, e) =>
            {
                personajeSeleccionado = "Caballero";
                panelSeleccionPersonaje.Visible = false;
                panelInicio.Visible = true;
            };
            Label atributosCaballero = new Label
            {
                Text = $"Vida: {_vidaCaballero} \nAtaque: {_danoCaballero}",
                ForeColor = Color.White,
                Font = new Font("Arial Black", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point((panelSeleccionPersonaje.Width / 2) + 80, (panelSeleccionPersonaje.Height / 2) - 30),
                BackColor = Color.FromArgb(0, 0, 0, 0),
            };

            PictureBox Enano = new PictureBox
            {
                Image = _enano,
                Size = new Size(40, 40),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Location = new Point((panelSeleccionPersonaje.Width / 2) - 125, (panelSeleccionPersonaje.Height / 2) + 40)
            };
            Button btnEnano = CreateStyledButton("Enano", (panelSeleccionPersonaje.Width / 2) - 75, (panelSeleccionPersonaje.Height / 2) + 40);
            btnEnano.Click += (s, e) => {
                personajeSeleccionado = "Enano";
                panelSeleccionPersonaje.Visible = false;
                panelInicio.Visible = true;
            };
            Label atributosEnano = new Label
            {
                Text = $"Vida: {_vidaEnano} \nAtaque: {_danoEnano}",
                ForeColor = Color.White,
                Font = new Font("Arial Black", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point((panelSeleccionPersonaje.Width / 2) + 80, (panelSeleccionPersonaje.Height / 2) + 40),
                BackColor = Color.FromArgb(0, 0, 0, 0),
            };

            PictureBox Aldeano = new PictureBox
            {
                Image = _aldeano,
                Size = new Size(40, 40),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Location = new Point((panelSeleccionPersonaje.Width / 2) - 125, (panelSeleccionPersonaje.Height / 2) + 110)
            };
            Button btnAldeano = CreateStyledButton("Aldeano", (panelSeleccionPersonaje.Width / 2) - 75, (panelSeleccionPersonaje.Height / 2) + 110);
            btnAldeano.Click += (s, e) => {
                personajeSeleccionado = "Aldeano";
                panelSeleccionPersonaje.Visible = false;
                panelInicio.Visible = true;
            };
            Label atributosAldeano = new Label
            {
                Text = $"Vida: {_vidaAldeano} \nAtaque: {_danoAldeano}",
                ForeColor = Color.White,
                Font = new Font("Arial Black", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point((panelSeleccionPersonaje.Width / 2) + 80, (panelSeleccionPersonaje.Height / 2) + 110),
                BackColor = Color.FromArgb(0, 0, 0, 0),
            };

            Button btnVolver = CreateStyledButton("Volver", (panelSeleccionPersonaje.Width / 2) - 75, (panelSeleccionPersonaje.Height / 2) + 180);
            btnVolver.Click += (s, e) => {
                panelSeleccionPersonaje.Visible = false;
                panelInicio.Visible = true;
            };

            panelSeleccionPersonaje.Controls.Add(Mago);
            panelSeleccionPersonaje.Controls.Add(btnMago);
            panelSeleccionPersonaje.Controls.Add(atributosMago);
            panelSeleccionPersonaje.Controls.Add(Caballero);
            panelSeleccionPersonaje.Controls.Add(btnCaballero);
            panelSeleccionPersonaje.Controls.Add(atributosCaballero);
            panelSeleccionPersonaje.Controls.Add(Enano);
            panelSeleccionPersonaje.Controls.Add(btnEnano);
            panelSeleccionPersonaje.Controls.Add(atributosEnano);
            panelSeleccionPersonaje.Controls.Add(Aldeano);
            panelSeleccionPersonaje.Controls.Add(btnAldeano);
            panelSeleccionPersonaje.Controls.Add(atributosAldeano);
            panelSeleccionPersonaje.Controls.Add(btnVolver);

            this.Controls.Add(panelSeleccionPersonaje);







            // Panel de pausa
            panelPausa = new Panel
            {
                Size = this.ClientSize,
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(200, 0, 0, 0),
                Visible = false
            };

            Label pausaTitulo = new Label
            {
                Text = "Pausa",
                ForeColor = Color.White,
                Font = new Font("Arial Black", 24, FontStyle.Bold),
                AutoSize = true,
                Location = new Point((panelPausa.Width / 2) - 60, (panelPausa.Height / 2) - 100),
                BackColor = Color.FromArgb(0, 0, 0, 0),
            };

            Button btnReanudar = CreateStyledButton("Reanudar", (panelPausa.Width / 2) - 75, (panelPausa.Height / 2) - 25);
            btnReanudar.Click += (s, e) => {
                pausa = false;
                _temporizador.Start();
                panelPausa.Visible = false;
                this.Cursor = OcultarCursor();
                BloquearCursor();
            };

            Button btnReiniciarPausa = CreateStyledButton("Reiniciar", (panelPausa.Width / 2) - 75, (panelPausa.Height / 2) + 35);
            btnReiniciarPausa.Click += (s, e) => {
                ReiniciarJuego();
                inicio = true;
            };

            Button btnSalirPausa = CreateStyledButton("Salir", (panelPausa.Width / 2) - 75, (panelPausa.Height / 2) + 95);
            btnSalirPausa.Click += (s, e) => Application.Exit();

            panelPausa.Controls.Add(pausaTitulo);
            panelPausa.Controls.Add(btnReanudar);
            panelPausa.Controls.Add(btnReiniciarPausa);
            panelPausa.Controls.Add(btnSalirPausa);
            this.Controls.Add(panelPausa);









            // Panel de Game Over
            panelGameOver = new Panel
            {
                Size = this.ClientSize,
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(200, 0, 0, 0),
                Visible = false
            };

            Label gameOverTitulo = new Label
            {
                Text = "YOU DIED",
                ForeColor = Color.Red,
                Font = new Font("Arial Black", 48, FontStyle.Bold),
                AutoSize = true,
                Location = new Point((panelGameOver.Width / 2) - 160, (panelGameOver.Height / 2) - 100),
                BackColor = Color.FromArgb(0, 0, 0, 0),
            };

            Button btnReiniciar = CreateStyledButton("Reiniciar", (panelGameOver.Width / 2) - 75, (panelGameOver.Height / 2) + 20);
            btnReiniciar.Click += (s, e) => {
                ReiniciarJuego();
                inicio = true;
            };
            Button btnSalirGameOver = CreateStyledButton("Salir", (panelGameOver.Width / 2) - 75, (panelGameOver.Height / 2) + 90);
            btnSalirGameOver.Click += (s, e) => Application.Exit();

            panelGameOver.Controls.Add(gameOverTitulo);
            panelGameOver.Controls.Add(btnReiniciar);
            panelGameOver.Controls.Add(btnSalirGameOver);
            this.Controls.Add(panelGameOver);

            // Panel de Mejora
            panelMejora = new Panel
            {
                Size = this.ClientSize,
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(200, 0, 0, 0),
                Visible = false
            };

            Label mejoraTitulo = new Label
            {
                Text = "Elige una mejora",
                ForeColor = Color.White,
                Font = new Font("Arial Black", 24, FontStyle.Bold),
                AutoSize = true,
                Location = new Point((panelMejora.Width / 2) - 140, (panelMejora.Height / 2) - 100),
                BackColor = Color.FromArgb(0, 0, 0, 0),
            };

            Button btnAumentarAtaque = CreateStyledButton("Aumentar Ataque", (panelMejora.Width / 2) - 75, (panelMejora.Height / 2) + 20);
            btnAumentarAtaque.Click += (s, e) =>
            {
                inicio = false;
                JugadorDanio += 5;
                panelMejora.Visible = false;
                this.Cursor = OcultarCursor();
                BloquearCursor();
                _temporizador.Start();
            };

            Button btnCurarse = CreateStyledButton("Curarse", (panelMejora.Width / 2) - 75, (panelMejora.Height / 2) + 90);
            btnCurarse.Click += (s, e) =>
            {
                inicio = false;
                _saludDelJugador = Math.Min(_saludDelJugador + 30, JugadorSaludMax);
                panelMejora.Visible = false;
                this.Cursor = OcultarCursor();
                BloquearCursor();
                _temporizador.Start();
            };

            panelMejora.Controls.Add(mejoraTitulo);
            panelMejora.Controls.Add(btnAumentarAtaque);
            panelMejora.Controls.Add(btnCurarse);
            this.Controls.Add(panelMejora);
        }

        private Button CreateStyledButton(string text, int x, int y)
        {
            Button button = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(150, 50),
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = Color.White;
            return button;
        }




        private void AlDibujar(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.CornflowerBlue);

            // Dibujar el fondo
            g.DrawImage(_imagenFondo, 0, 0, _imagenFondo.Width, _imagenFondo.Height);

            // Dibujar el cofre en el centro
            Image imagenCofre;
            switch (_estadoCofre)
            {
                case 0:
                    imagenCofre = _cofreCerrado;
                    break;
                case 1:
                    imagenCofre = _cofreMedioAbierto;
                    break;
                case 2:
                    imagenCofre = _cofreAbierto;
                    break;
                default:
                    imagenCofre = _cofreCerrado;
                    break;
            }
            g.DrawImage(imagenCofre, (ClientSize.Width - imagenCofre.Width) / 2, (ClientSize.Height - imagenCofre.Height) / 2 - 20);

            if (_mostrarExclamacion)
            {
                g.DrawImage(_imagenExclamacion, _posicionDelJugador.X - _imagenExclamacion.Width / 2 + 200, _posicionDelJugador.Y - _imagenJugador.Height - _imagenExclamacion.Height + 400, _imagenExclamacion.Width - 400, _imagenExclamacion.Height - 400);
            }


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
                    enemigo.Dibujar(g);
                }

                // Dibujar la mira en la posición del mouse
                g.DrawImage(_imagenMira, _mousePosition.X - _imagenMira.Width / 2, _mousePosition.Y - _imagenMira.Height / 2, _imagenMira.Width, _imagenMira.Height);


                // Dibujar HUD
                DibujarVida(g);
                DibujarAtaque(g);
                g.DrawString($"Puntos: {_scoreManager.GetScore()}", new Font("Arial", 16), Brushes.White, 10, 115);
            }
            else if (_saludDelJugador <= 0)
            {
                // Detener el temporizador y mostrar el panel de Game Over

                inicio = true;
                _temporizador.Stop();
                panelGameOver.Visible = true;

                // Mostrar el cursor del mouse y detener la música
                this.Cursor = Cursors.Default;
                _player.Stop();
                LiberarCursor();
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
                float healthPercentage = (float)_boss.Salud / (200f + 200 * _derrotados);
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
            if (!_rotacionBloqueada)
            {
                // Actualizar la posición del mouse
                _mousePosition = e.Location;

                // Calcular la rotación del jugador
                float dx = e.X - _posicionDelJugador.X;
                float dy = e.Y - _posicionDelJugador.Y;
                _rotacionDelJugador = (float)(Math.Atan2(dy, dx) * 180.0 / Math.PI);
            }
        }

        private void DibujarVida(Graphics g)
        {
            int corazonAncho = _corazon.Width / 2;
            int corazonAltura = _corazon.Height / 2;
            int xInicial = 10;
            int yInicial = 10;

            for (int i = 0; i < JugadorSaludMax / 20; i++)
            {
                if (_saludDelJugador >= (i + 1) * 20)
                {
                    g.DrawImage(_corazon, xInicial + i * corazonAncho, yInicial, corazonAncho, corazonAltura);
                }
                else if (_saludDelJugador >= i * 20 + 10)
                {
                    g.DrawImage(_medioCorazon, xInicial + i * corazonAncho, yInicial, corazonAncho, corazonAltura);
                }
                else
                {
                    g.DrawImage(_corazonVacio, xInicial + i * corazonAncho, yInicial, corazonAncho, corazonAltura);
                }
            }
        }

        private void DibujarAtaque(Graphics g)
        {
            int espadaAncho = _espada.Width / 9;
            int espadaAltura = _espada.Height / 9;
            int xInicial = 10;
            int yInicial = 50;

            for (int i = 0; i < JugadorDanio / 5; i++)
            {
                g.DrawImage(_espada, xInicial + i * espadaAncho, yInicial, espadaAncho, espadaAltura);
            }
        }



        private void AlPresionarMouse(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _puedeMoverse)
            {
                // Crear una nueva bala
                float dx = e.X - _posicionDelJugador.X;
                float dy = e.Y - _posicionDelJugador.Y;
                float longitud = (float)Math.Sqrt(dx * dx + dy * dy);

                _balas.Add(new Bala(_posicionDelJugador, new PointF(dx / longitud, dy / longitud)));
            }
        }

        private void Actualizar(object sender, EventArgs e)
        {
            // Actualizar animación de entrada del jugador
            if (_animacionIniciada)
            {
                float dx = _posicionFinalJugador.X - _posicionDelJugador.X;
                float dy = _posicionFinalJugador.Y - _posicionDelJugador.Y;
                float distance = (float)Math.Sqrt(dx * dx + dy * dy);
                float speed = 3.0f;

                if (distance > 1)
                {
                    _posicionDelJugador.X += dx / distance * speed;
                    _posicionDelJugador.Y += dy / distance * speed;
                }
                else
                {
                    _posicionDelJugador = _posicionFinalJugador;
                    _animacionIniciada = false;

                    // Iniciar animación del cofre
                    _animacionCofreIniciada = true;
                    _temporizadorCofre.Start();
                }

                Invalidate();
                return;
            }

            if (_animacionCofreIniciada && _estadoCofre == 2)
            {
                _animacionCofreIniciada = false;
                _temporizadorCofre.Stop();
                _temporizadorExclamacion.Start();
            }

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
            if (_exclamacionTitileoContador == 0)
            {
                _temporizadorDeAparicionDeEnemigos++;
                if (_temporizadorDeAparicionDeEnemigos > 60 && !_bossAparecido)
                {
                    GenerarEnemigo();
                    _temporizadorDeAparicionDeEnemigos = 0;
                }
            }


            //Quitar filtro rojo de danio
            if(reciveDano >= 1)
            {
                reciveDano++;
                if(reciveDano == 10)
                {
                    _imagenJugador = _original;
                    reciveDano = 0;
                }
            }

            // Actualizar enemigos
            for (int i = _enemigos.Count - 1; i >= 0; i--)
            {
                _enemigos[i].Actualizar(_posicionDelJugador);
                if (_enemigos[i].EstaMuerto())
                {
                    _enemigos.RemoveAt(i);
                    _scoreManager.AddPoints(5);
                }
                else if (_enemigos[i].EstaColisionandoCon(_posicionDelJugador))
                {

                    // Aplicar el filtro rojo de danio
                    if(reciveDano == 0)
                    {
                        reciveDano += 1;
                        aplicarDanio(ref _imagenJugador);
                    }

                    _saludDelJugador -= 10;
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
                        _enemigos[j].RecibirDanio(JugadorDanio);
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

                    // Mostrar el panel de mejora
                    panelMejora.Visible = true;
                    inicio = true;
                    this.Cursor = Cursors.Default;
                    _temporizador.Stop();
                    LiberarCursor();

                    // Incrementar el umbral para la siguiente aparición del jefe
                    _puntosParaSiguienteBoss += _incrementoPuntosBoss;
                    _derrotados++;
                }
                else
                {
                    for (int i = _balas.Count - 1; i >= 0; i--)
                    {
                        if (_balas[i].EstaColisionandoCon(_boss.Posicion))
                        {
                            _boss.RecibirDanio(JugadorDanio);
                            _balas.RemoveAt(i);
                            break;
                        }
                    }

                    // Hacer que el jefe continúe moviéndose hacia el jugador
                    _boss.Actualizar(_posicionDelJugador);

                    // Verificar si el jefe está atacando y aplicar daño al jugador
                    if (_boss.EstaAtacando())
                    {
                        _boss.Atacar(_posicionDelJugador);
                    }

                    // Verificar colisión simple y aplicar daño al jugador
                    if (_boss.EstaColisionandoCon(_posicionDelJugador))
                    {
                        // Aplicar el filtro rojo de danio
                        if (reciveDano == 0)
                        {
                            reciveDano += 1;
                            aplicarDanio(ref _imagenJugador);
                        }
                        _saludDelJugador -= 10;
                    }
                }
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

            // Verificar si es necesario generar un nuevo jefe basado en los puntos del jugador
            if (_scoreManager.GetScore() >= _puntosParaSiguienteBoss && !_bossAparecido)
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
                _boss = new Boss(new PointF(ClientSize.Width / 2, ClientSize.Height / 2), 5.5f, 200 + 200 * _derrotados, animacionCaminar, animacionAtaque, animacionMuerte, this);
            }

            Invalidate();
        }




        private void MoverJugador()
        {
            float speed = 5.0f;

            if (_arribaPresionado)
                _posicionDelJugador.Y -= speed;
            if (_abajoPresionado)
                _posicionDelJugador.Y += speed;
            if (_izquierdaPresionado)
                _posicionDelJugador.X -= speed;
            if (_derechaPresionado)
                _posicionDelJugador.X += speed;

            // Limitar la posición del jugador dentro de los límites del fondo
            _posicionDelJugador.X = Math.Max(0, Math.Min(ClientSize.Width, _posicionDelJugador.X));
            _posicionDelJugador.Y = Math.Max(0, Math.Min(ClientSize.Height, _posicionDelJugador.Y));

            Invalidate(); // Redibujar el formulario para actualizar la posición del jugador
        }


        private void AlPresionarTecla(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    _arribaPresionado = true;
                    break;
                case Keys.S:
                    _abajoPresionado = true;
                    break;
                case Keys.A:
                    _izquierdaPresionado = true;
                    break;
                case Keys.D:
                    _derechaPresionado = true;
                    break;
                case Keys.Escape:

                    if (pausa == true)
                    {
                        pausa = false;
                        _temporizador.Start();
                        panelPausa.Visible = false;
                        this.Cursor = OcultarCursor();
                        BloquearCursor();
                        panelPausa.Visible = false;
                    }
                    else if (inicio == false)
                    {
                        PausarJuego();
                    }
                    
                    break;
                case Keys.Space:
                    e.SuppressKeyPress = true;
                    break;
            }
        }

        private void AlSoltarTecla(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    _arribaPresionado = false;
                    break;
                case Keys.S:
                    _abajoPresionado = false;
                    break;
                case Keys.A:
                    _izquierdaPresionado = false;
                    break;
                case Keys.D:
                    _derechaPresionado = false;
                    break;
                case Keys.Space:
                    e.SuppressKeyPress = true;
                    break;
            }
        }

        private void PausarJuego()
        {
            pausa = true;
            _temporizador.Stop();
            panelPausa.Visible = true;
            this.Cursor = Cursors.Default;
            LiberarCursor();
        }





        private void GenerarEnemigo()
        {
            string imagesPath = Path.Combine(basePath, "src");

            switch (_incrementoDeSaludDeEnemigos)
            {
                case int salud when salud < 20 && _derrotados == 0:
                    _imagenEnemigo = Image.FromFile(Path.Combine(imagesPath, "tile_0121.png"));
                    _incrementoDeSaludDeEnemigos++;

                    break;
                case int salud when salud < 30 && _derrotados == 0:
                    _imagenEnemigo = Image.FromFile(Path.Combine(imagesPath, "tile_0122.png"));
                    _incrementoDeSaludDeEnemigos++;

                    break;
                case int salud when salud < 40 && _derrotados == 1:
                    _imagenEnemigo = Image.FromFile(Path.Combine(imagesPath, "tile_0123.png"));
                    _incrementoDeSaludDeEnemigos++;

                    break;
                case int salud when salud < 50 && _derrotados == 1:
                    _imagenEnemigo = Image.FromFile(Path.Combine(imagesPath, "tile_0108.png"));
                    _incrementoDeSaludDeEnemigos++;

                    break;
                case int salud when salud < 60 && _derrotados == 2:
                    _imagenEnemigo = Image.FromFile(Path.Combine(imagesPath, "tile_0109.png"));
                    _incrementoDeSaludDeEnemigos++;

                    break;
                case int salud when salud < 70 && _derrotados == 2:
                    _imagenEnemigo = Image.FromFile(Path.Combine(imagesPath, "tile_0111.png"));
                    _incrementoDeSaludDeEnemigos++;

                    break;
                default:
                    if(_derrotados > 2)
                    {
                        _incrementoDeSaludDeEnemigos++;
                    }
                    break;
            }

            PointF posicion = new PointF(_aleatorio.Next(ClientSize.Width), _aleatorio.Next(ClientSize.Height));
            _enemigos.Add(new Enemigo(posicion, _incrementoDeSaludDeEnemigos, _imagenEnemigo, _incrementoDeSaludDeEnemigos));
        }


        private void CambiarEstadoCofre()
        {
            _estadoCofre++;
            if (_estadoCofre > 2)
            {
                _estadoCofre = 2;
                _temporizadorCofre.Stop();
            }

            Invalidate();
        }

        private void TitilarExclamacion()
        {
            _mostrarExclamacion = !_mostrarExclamacion;
            _exclamacionTitileoContador++;

            if (_exclamacionTitileoContador >= 6)
            {
                _temporizadorExclamacion.Stop();
                _exclamacionTitileoContador = 0;
                _mostrarExclamacion = false;
                _puedeMoverse = true;
                _rotacionBloqueada = false;
            }

            Invalidate();
        }

        public void AplicarDanioJugador(int cantidad)
        {
            // Aplicar el filtro rojo de danio
            if (reciveDano == 0)
            {
                reciveDano += 1;
                aplicarDanio(ref _imagenJugador);
            }

            _saludDelJugador += cantidad;
            if (_saludDelJugador < 0)
            {
                _saludDelJugador = 0;
            }
        }

        private void BloquearCursor()
        {
            Rectangle rect = this.RectangleToScreen(this.ClientRectangle);
            Cursor.Clip = rect;
        }

        private void LiberarCursor()
        {
            Cursor.Clip = Rectangle.Empty;
        }


        private void aplicarDanio(ref Image image)
        {
            Bitmap bmp = new Bitmap(image);
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color originalColor = bmp.GetPixel(x, y);
                    Color redColor = Color.FromArgb(originalColor.A, originalColor.R, 0, 0);
                    bmp.SetPixel(x, y, redColor);
                }
            }
            image = bmp;
        }

        


    }
}