using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace meteoalarm_tlg
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer;
        private AvisosTlg _avisosTlg;
        private bool _isProcessing;

        public MainWindow()
        {
            InitializeComponent();
            // Inicializar el objeto AvisosTlg y el DispatcherTimer
            _avisosTlg = new AvisosTlg();
            // Suscribir los eventos ProcesarIniciado y ProcesarTerminado
            _avisosTlg.ProcesarIniciado += OnProcesarIniciado;
            _avisosTlg.ProcesarTerminado += OnProcesarTerminado;
            // Suscribir los eventos
            _avisosTlg.ProcesarLabelZonasAct += OnProcesarLabelZonasAct;
            _avisosTlg.ProcesarLabelNumEjec += OnProcesarLabelNumEjec;
            _avisosTlg.ProcesarLabelFecPrimeraEjec += OnProcesarLabelFecPrimeraEjec;
            _avisosTlg.ProcesarLabelNumAvisosAct += OnProcesarLabelNumAvisosAct;
            _avisosTlg.ProcesarLabelFecUltEjec += OnProcesarLabelFecUltEjec;
            _avisosTlg.ProcesarLabelAvisosSent += OnProcesarLabelAvisosSent;

            _avisosTlg.ProcesarError += OnProcesarError;

            // Crear un nuevo DispatcherTimer
            _timer = new DispatcherTimer();
            // Establecer el intervalo del DispatcherTimer
            _timer.Interval = TimeSpan.FromSeconds(AppSettings.IntervaloTiempo);
            // Asignar el evento Timer_Tick al DispatcherTimer
            _timer.Tick += Timer_Tick;
            // Valor por defecto para _isProcessing, con el proceso detenido
            _isProcessing = false;
        }


        /// <summary>
        /// Metodo que se ejecuta cada vez que el DispatcherTimer alcanza el intervalo establecido
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Comprobar si los datos de configuración están completos
            if (AppSettings.BotToken == null || AppSettings.ChannelId == null || AppSettings.ChannelPrioId == null || AppSettings.UrlMeteoAlarm == null)
            {
                MessageBox.Show("Faltan datos de configuración", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Procesar los avisos, llamando al método Procesar de la clase AvisosTlg
            //_avisosTlg.Procesar();
            // Ejecutar el método Procesar de manera asíncrona
            Task.Run(() => _avisosTlg.Procesar());
        }

        
        private void BtnConfig_Click(object sender, RoutedEventArgs e)
        {
            Config ventanaConfig = new Config();
            ventanaConfig.Show();
        }

        private void BtnProceso_Click(object sender, RoutedEventArgs e)
        {
            if (_isProcessing)
            {
                string fechaHoraActual = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                lblFecPrimeraEjec.Content = "Primera ejecución: " + fechaHoraActual;

                lblZonasAct.Content = AppSettings.EmmaId;

                // Detener el proceso
                _timer.Stop();
                // Cambiar la imagen del botón
                BtnProceso.Content = new Image
                {
                    Source = new BitmapImage(new Uri("pack://application:,,,/images/play_32x32.png")),
                    Height = 32,
                    Width = 32
                };
            }
            else
            {
                // Iniciar el proceso
                _timer.Start();
                // Cambiar la imagen del botón
                BtnProceso.Content = new Image
                {
                    Source = new BitmapImage(new Uri("pack://application:,,,/images/stop_32x32.png")),
                    Height = 32,
                    Width = 32
                };
            }

            _isProcessing = !_isProcessing;
        }






        // Botón para ejecutar el proceso de manera manual
        private void BtnEjecManual_Click(object sender, RoutedEventArgs e)
        {
            string fechaHoraActual = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            lblFecPrimeraEjec.Content = "Primera ejecución: " + fechaHoraActual;

            lblZonasAct.Content = AppSettings.EmmaId;

            // Ejecutar de forma manual, pero asíncrona
            Task.Run(() => _avisosTlg.Procesar());
        }







        private void OnProcesarIniciado(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                StatusImage.Source = new BitmapImage(new Uri("pack://application:,,,/images/verde_32x32.png"));
            });
        }

        private void OnProcesarTerminado(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                StatusImage.Source = new BitmapImage(new Uri("pack://application:,,,/images/rojo_32x32.png"));
            });
        }


        // Método para manejar el evento y actualizar el label
        private void OnProcesarLabelZonasAct(object sender, string texto)
        {
            // Asegúrate de actualizar el label en el hilo de la interfaz de usuario
            Dispatcher.Invoke(() => 
            {
                lblZonasAct.Content = texto;
            });
        }

        private void OnProcesarLabelNumEjec(object sender, string texto)
        {
            // Asegúrate de actualizar el label en el hilo de la interfaz de usuario
            Dispatcher.Invoke(() =>
            {
                lblNumEjec.Content = texto;
            });
        }

        private void OnProcesarLabelFecPrimeraEjec(object sender, string texto)
        {
            // Asegúrate de actualizar el label en el hilo de la interfaz de usuario
            Dispatcher.Invoke(() =>
            {
                lblFecPrimeraEjec.Content = texto;
            });
        }

        private void OnProcesarLabelNumAvisosAct(object sender, string texto)
        {
            // Asegúrate de actualizar el label en el hilo de la interfaz de usuario
            Dispatcher.Invoke(() =>
            {
                lblNumAvisosAct.Content = texto;
            });
        }

        private void OnProcesarLabelFecUltEjec(object sender, string texto)
        {
            // Asegúrate de actualizar el label en el hilo de la interfaz de usuario
            Dispatcher.Invoke(() =>
            {
                lblFecUltEjec.Content = texto;
            });
        }

        private void OnProcesarLabelAvisosSent(object sender, string texto)
        {
            // Asegúrate de actualizar el label en el hilo de la interfaz de usuario
            Dispatcher.Invoke(() =>
            {
                lblNumAvisosSent.Content = texto;
            });
        }


        private void OnProcesarError(object sender, string mensaje)
        {
            MessageBox.Show(mensaje, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

    }
}
