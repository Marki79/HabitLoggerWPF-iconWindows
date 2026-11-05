using System.Windows;
using System.Windows.Input;

namespace HabitLoggerWPF
{
    // Ventana que muestra la información de UN hábito.
    public partial class DetalleHabitoWindow : Window
    {
        // Recibe el hábito en el que se ha hecho clic
        public DetalleHabitoWindow(Habito habito)
        {
            InitializeComponent();

            // DataContext = de dónde leen los datos los {Binding ...} del XAML
            DataContext = habito;
        }

        // Se ejecuta con cualquier tecla pulsada en esta ventana.
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();   // Cierra solo esta ventana y se vuelve a la principal
            }
        }

        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
