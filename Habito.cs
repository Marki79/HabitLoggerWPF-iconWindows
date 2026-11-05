namespace HabitLoggerWPF
{
    // Modelo: representa una fila de la tabla Habitos.
    // Las propiedades públicas son las que el DataGrid enlaza con {Binding}.
    public class Habito
    {
        // Id único del hábito en la base de datos.
        public int Id { get; set; }

        // Nombre del hábito que se quiere registrar
        public string Nombre { get; set; }

        // Cantidad de veces que se ha realizado el hábito
        public int Cantidad { get; set; }

        // Fecha en la que se ha registrado el hábito (formato dd/MM/yyyy)
        public string Fecha { get; set; }

        // Constructor vacío para crear un hábito sin datos iniciales
        public Habito()
        {
            Nombre = "";
            Fecha = "";
        }

        // Constructor que permite crear un hábito con sus datos
        public Habito(string nombre, int cantidad, string fecha)
        {
            Nombre = nombre;
            Cantidad = cantidad;
            Fecha = fecha;
        }
    }
}
