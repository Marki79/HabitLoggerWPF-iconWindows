using System.Windows;

namespace HabitLoggerWPF
{
    public partial class App : Application
    {
        public App()
        {
            // Red de seguridad: si ocurre un error no previsto, mostramos un mensaje
            // y la aplicación NO se cierra.
            DispatcherUnhandledException += (sender, e) =>
            {
                new GestorErrores().RegistrarError(e.Exception.ToString());
                MessageBox.Show(e.Exception.Message, "Error inesperado",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                e.Handled = true;
            };
        }

        // Se ejecuta antes de abrir la primera ventana (MainWindow).
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                // Inicializamos el proveedor de SQLite 
                SQLitePCL.Batteries.Init();

                // Creamos la base de datos y las tablas si todavía no existen
                ConexionBD.CrearBaseDatos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo iniciar la base de datos.\n" + ex.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
                return;
            }

            base.OnStartup(e);
        }
    }
}
