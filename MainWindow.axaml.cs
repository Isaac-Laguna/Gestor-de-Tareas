using Avalonia.Controls;
using Avalonia.Interactivity;
using Gestor_de_Tareas.Datos;
using Gestor_de_Tareas.ModelosDeVista;

namespace Gestor_de_Tareas
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainViewModel(new SqliteTaskRepository());
            DataContext = _viewModel;
        }

        private void OnNewClick(object? sender, RoutedEventArgs e)
        {
            _viewModel.ClearForm();
        }

        private void OnCreateClick(object? sender, RoutedEventArgs e)
        {
            _viewModel.CreateTask();
        }

        private void OnUpdateClick(object? sender, RoutedEventArgs e)
        {
            _viewModel.UpdateTask();
        }

        private void OnDeleteClick(object? sender, RoutedEventArgs e)
        {
            _viewModel.DeleteTask();
        }

        private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            _viewModel.LoadSelectedTaskIntoForm();
        }
    }
}
