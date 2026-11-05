using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HabitLoggerWPF
{
    public partial class MainWindow : Window
    {
        // Objetos que permiten acceder a la base de datos y a los logs (igual que en la UD0)
        private HabitoDAO habitoDAO = new HabitoDAO();
        private GestorErrores gestorErrores = new GestorErrores();
        private Logger logger = new Logger();

        // Aquí guardamos TODOS los hábitos leídos de la base de datos.
        // El buscador filtra esta lista sin volver a consultar la base de datos.
        private List<Habito> todosLosHabitos = new List<Habito>();

        public MainWindow()
        {
            InitializeComponent();
        }

        // ---------- Ciclo de vida de la ventana ----------

        // Se ejecuta cuando la ventana ya está cargada: aquí leemos los datos.
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dpFecha.SelectedDate = DateTime.Today;   // Por defecto, la fecha de hoy
            logger.Registrar("Aplicación iniciada");
            CargarHabitos();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            logger.Registrar("Aplicación cerrada");
        }

        // ---------- Leer y pintar los datos ----------

        // Lee los hábitos de la base de datos y los muestra en la tabla.
        private void CargarHabitos()
        {
            try
            {
                todosLosHabitos = habitoDAO.ObtenerTodos();
                MostrarHabitos();
            }
            catch (Exception ex)
            {
                gestorErrores.RegistrarError(ex.ToString());
                MessageBox.Show("No se pudieron leer los hábitos.\n" + ex.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Aplica el texto del buscador a la lista completa y pinta el resultado en el DataGrid.
        private void MostrarHabitos()
        {
            string textoBuscado = txtBuscar.Text.Trim();
            List<Habito> habitosVisibles;

            if (textoBuscado == "")
            {
                // Sin filtro: mostramos todos
                habitosVisibles = todosLosHabitos;
            }
            else
            {
                // Con filtro: nos quedamos con los que contienen el texto (sin distinguir mayúsculas)
                habitosVisibles = todosLosHabitos
                    .Where(h => h.Nombre.Contains(textoBuscado, StringComparison.CurrentCultureIgnoreCase))
                    .ToList();
            }

            // ItemsSource = la colección que pinta el DataGrid
            dgHabitos.ItemsSource = habitosVisibles;

            // Sumamos la cantidad de todos los hábitos visibles para la barra de estado
            int totalCantidad = habitosVisibles.Sum(h => h.Cantidad);

            txtEstado.Text = "Mostrando " + habitosVisibles.Count + " de " + todosLosHabitos.Count
                + " hábitos  |  Cantidad total: " + totalCantidad;
        }

        // ---------- Eventos de los controles ----------

        // Cada vez que cambia el texto del buscador, volvemos a filtrar.
        private void TxtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            MostrarHabitos();
        }

        // El botón Eliminar solo se activa si hay una fila seleccionada.
        private void DgHabitos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnEliminar.IsEnabled = dgHabitos.SelectedItem != null;
        }

        // Se ejecuta al hacer clic en una fila de la tabla: abre la ventana de detalle.
        private void FilaHabito_Click(object sender, MouseButtonEventArgs e)
        {
            // "sender" es la fila (DataGridRow) en la que se ha hecho clic
            DataGridRow filaClicada = sender as DataGridRow;
            if (filaClicada == null)
            {
                return;
            }

            // Item es el objeto que hay detrás de la fila: el Habito
            Habito habitoClicado = filaClicada.Item as Habito;
            if (habitoClicado == null)
            {
                return;
            }

            // Creamos la ventana de detalle y le pasamos el hábito
            DetalleHabitoWindow ventanaDetalle = new DetalleHabitoWindow(habitoClicado);
            ventanaDetalle.Owner = this;   // Esta ventana es la "madre" (se centra sobre ella)

            // ShowDialog bloquea la ventana principal hasta que se cierre la de detalle
            ventanaDetalle.ShowDialog();
        }

        // Se ejecuta con cualquier tecla pulsada en la ventana principal.
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                // Si el calendario del DatePicker está desplegado, ESC solo debe cerrar el calendario
                if (dpFecha.IsDropDownOpen)
                {
                    return;
                }

                Close();   // Cierra la ventana y, con ella, la aplicación
            }
        }

        private void BtnRecargar_Click(object sender, RoutedEventArgs e)
        {
            CargarHabitos();
        }

        private void MenuSalir_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Valida el formulario y guarda un hábito nuevo.
        private void BtnAnadir_Click(object sender, RoutedEventArgs e)
        {
            // 1) Nombre obligatorio
            string nombre = txtNombre.Text.Trim();
            if (nombre == "")
            {
                MessageBox.Show("Escribe el nombre del hábito.", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtNombre.Focus();
                return;
            }

            // 2) Cantidad: entero mayor o igual que 0 (TryParse no lanza error si no es un número)
            int cantidad;
            if (!int.TryParse(txtCantidad.Text, out cantidad) || cantidad < 0)
            {
                MessageBox.Show("La cantidad debe ser un número entero (0 o más).", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtCantidad.Focus();
                return;
            }

            // 3) Fecha: SelectedDate es DateTime? (puede estar vacía), hay que comprobarlo
            DateTime? fechaElegida = dpFecha.SelectedDate;
            if (fechaElegida == null)
            {
                MessageBox.Show("Elige una fecha.", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Guardamos la fecha como texto dd/MM/yyyy, igual que en la UD0
                Habito nuevoHabito = new Habito(nombre, cantidad, fechaElegida.Value.ToString("dd/MM/yyyy"));
                habitoDAO.Insertar(nuevoHabito);
                logger.Registrar("Hábito insertado: " + nombre + " (" + cantidad + ")");

                // Limpiamos el formulario y recargamos la tabla
                txtNombre.Clear();
                txtCantidad.Clear();
                dpFecha.SelectedDate = DateTime.Today;
                CargarHabitos();
                txtNombre.Focus();
            }
            catch (Exception ex)
            {
                gestorErrores.RegistrarError(ex.ToString());
                MessageBox.Show("No se pudo guardar el hábito.\n" + ex.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Elimina el hábito seleccionado en la tabla (pidiendo confirmación).
        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            // SelectedItem es un object: lo convertimos a Habito con "as" (null si no hay selección)
            Habito habitoSeleccionado = dgHabitos.SelectedItem as Habito;
            if (habitoSeleccionado == null)
            {
                return;
            }

            MessageBoxResult respuesta = MessageBox.Show(
                "¿Eliminar el hábito \"" + habitoSeleccionado.Nombre + "\"?",
                "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (respuesta != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                habitoDAO.Eliminar(habitoSeleccionado.Id);
                logger.Registrar("Hábito eliminado: Id " + habitoSeleccionado.Id);
                CargarHabitos();
            }
            catch (Exception ex)
            {
                gestorErrores.RegistrarError(ex.ToString());
                MessageBox.Show("No se pudo eliminar el hábito.\n" + ex.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Inserta unos cuantos hábitos de prueba para ver la tabla con datos.
        private void MenuEjemplo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string hoy = DateTime.Today.ToString("dd/MM/yyyy");
                string ayer = DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy");

                habitoDAO.Insertar(new Habito("Beber agua", 8, ayer));
                habitoDAO.Insertar(new Habito("Leer", 30, ayer));
                habitoDAO.Insertar(new Habito("Caminar", 5, hoy));
                habitoDAO.Insertar(new Habito("Beber agua", 6, hoy));

                logger.Registrar("Datos de ejemplo insertados");
                CargarHabitos();
            }
            catch (Exception ex)
            {
                gestorErrores.RegistrarError(ex.ToString());
                MessageBox.Show("No se pudieron insertar los datos de ejemplo.\n" + ex.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
