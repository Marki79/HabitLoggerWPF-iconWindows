using System.IO;
namespace HabitLoggerWPF
{
    public class Logger
    {
        // Registra una operación en el archivo de log del día (carpeta logs, junto al .exe).
        public void Registrar(string mensaje)
        {
            try
            {
                string carpeta = Path.Combine(AppContext.BaseDirectory, "logs");

                // Creamos la carpeta si todavía no existe
                if (!Directory.Exists(carpeta))
                {
                    Directory.CreateDirectory(carpeta);
                }

                // Un archivo por día: 2026-11-02.log
                string archivo = DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                string ruta = Path.Combine(carpeta, archivo);

                string texto = DateTime.Now.ToString("HH:mm:ss") + " - " + mensaje;

                // Añadimos la línea sin borrar las anteriores.
                File.AppendAllText(ruta, texto + Environment.NewLine);
            }
            catch (IOException)
            {
                // Un fallo en el log no debe cerrar la aplicación.
            }
            catch (UnauthorizedAccessException)
            {
                // Puede no haber permisos para escribir.
            }
        }
    }
}
