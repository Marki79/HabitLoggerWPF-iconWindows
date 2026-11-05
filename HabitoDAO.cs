using Microsoft.Data.Sqlite;

namespace HabitLoggerWPF
{
    // DAO: la clase que habla con la base de datos. La ventana nunca escribe SQL.
    public class HabitoDAO
    {
        // Guarda un hábito nuevo en la base de datos.
        public void Insertar(Habito habito)
        {
            using (SqliteConnection conexion = ConexionBD.ObtenerConexion())
            {
                conexion.Open();

                // Usamos parámetros (@nombre...) en vez de pegar el texto en el SQL
                string sql = @"INSERT INTO Habitos (Nombre, Cantidad, Fecha)
                               VALUES (@nombre, @cantidad, @fecha)";

                using (SqliteCommand comando = new SqliteCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@nombre", habito.Nombre);
                    comando.Parameters.AddWithValue("@cantidad", habito.Cantidad);
                    comando.Parameters.AddWithValue("@fecha", habito.Fecha);

                    comando.ExecuteNonQuery();
                }
            }
        }

        // Devuelve una lista con todos los hábitos registrados.
        public List<Habito> ObtenerTodos()
        {
            List<Habito> lista = new List<Habito>();

            using (SqliteConnection conexion = ConexionBD.ObtenerConexion())
            {
                conexion.Open();

                // Los más recientes (último Id) salen primero
                string sql = "SELECT * FROM Habitos ORDER BY Id DESC";

                using (SqliteCommand comando = new SqliteCommand(sql, conexion))
                using (SqliteDataReader lector = comando.ExecuteReader())
                {
                    // Leemos los resultados fila a fila
                    while (lector.Read())
                    {
                        Habito habito = new Habito();
                        habito.Id = Convert.ToInt32(lector["Id"]);
                        habito.Nombre = lector["Nombre"].ToString();
                        habito.Cantidad = Convert.ToInt32(lector["Cantidad"]);
                        habito.Fecha = lector["Fecha"].ToString();

                        lista.Add(habito);
                    }
                }
            }

            return lista;
        }

        // Elimina un hábito utilizando su identificador.
        public bool Eliminar(int id)
        {
            using (SqliteConnection conexion = ConexionBD.ObtenerConexion())
            {
                conexion.Open();

                string sql = "DELETE FROM Habitos WHERE Id = @id";

                using (SqliteCommand comando = new SqliteCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);

                    // Devuelve true si se ha eliminado algún registro.
                    return comando.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
