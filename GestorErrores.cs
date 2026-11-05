using Microsoft.Data.Sqlite;

namespace HabitLoggerWPF
{
    public class GestorErrores
    {
        // Guarda un mensaje de error junto con la fecha y la hora en la tabla Errores.
        public void RegistrarError(string mensaje)
        {
            try
            {
                using (SqliteConnection conexion = ConexionBD.ObtenerConexion())
                {
                    conexion.Open();

                    string sql = @"INSERT INTO Errores (Mensaje, Fecha)
                                   VALUES (@mensaje, @fecha)";

                    using (SqliteCommand comando = new SqliteCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@mensaje", mensaje);
                        comando.Parameters.AddWithValue("@fecha",
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                        comando.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                // No dejamos que falle el registro del error.
            }
        }
    }
}
