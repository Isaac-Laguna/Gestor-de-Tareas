using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Gestor_de_Tareas.Datos;
using Gestor_de_Tareas.Modelos;

namespace Gestor_de_Tareas.ModelosDeVista
{
    // Holds the data shown by the window and the actions triggered by its events.
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ITaskRepository _repository;
        private readonly ObservableCollection<TaskItem> _tasks;
        private TaskItem? _selectedTask;
        private string _title = string.Empty;
        private string _description = string.Empty;
        private string _selectedPriority = Labels.Priorities[0];
        private string _selectedStatus = Labels.Statuses[0];
        private string _message = "Listo para comenzar.";
        private bool _hasTasks;

        public MainViewModel(ITaskRepository repository)
        {
            _repository = repository;
            _tasks = new ObservableCollection<TaskItem>();
            Reload();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<TaskItem> Tasks => _tasks;

        public IReadOnlyList<string> Priorities => Labels.Priorities;

        public IReadOnlyList<string> Statuses => Labels.Statuses;

        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        public string SelectedPriority
        {
            get => _selectedPriority;
            set { _selectedPriority = value; OnPropertyChanged(); }
        }

        public string SelectedStatus
        {
            get => _selectedStatus;
            set { _selectedStatus = value; OnPropertyChanged(); }
        }

        public string Message
        {
            get => _message;
            set { _message = value; OnPropertyChanged(); }
        }

        public bool HasTasks
        {
            get => _hasTasks;
            private set { _hasTasks = value; OnPropertyChanged(); }
        }

        public bool IsEmpty => !_hasTasks;

        public TaskItem? SelectedTask
        {
            get => _selectedTask;
            set { _selectedTask = value; OnPropertyChanged(); }
        }

        public void CreateTask()
        {
            TaskItem task = BuildTaskFromForm();
            IReadOnlyList<string> errors = task.Validate();
            if (errors.Count > 0)
            {
                Message = string.Join(" ", errors);
                return;
            }

            _repository.Create(task);
            Reload();
            ClearForm();
            Message = "Tarea creada correctamente.";
        }

        public void UpdateTask()
        {
            if (SelectedTask is null)
            {
                Message = "Selecciona una tarea de la lista para actualizarla.";
                return;
            }

            TaskItem task = BuildTaskFromForm();
            IReadOnlyList<string> errors = task.Validate();
            if (errors.Count > 0)
            {
                Message = string.Join(" ", errors);
                return;
            }

            SelectedTask.Update(task.Title, task.Description, task.Priority, task.Status);
            _repository.Update(SelectedTask);
            Reload();
            ClearForm();
            Message = "Tarea actualizada correctamente.";
        }

        public void DeleteTask()
        {
            if (SelectedTask is null)
            {
                Message = "Selecciona una tarea de la lista para eliminarla.";
                return;
            }

            _repository.Delete(SelectedTask.Id);
            Reload();
            ClearForm();
            Message = "Tarea eliminada correctamente.";
        }

        public void LoadSelectedTaskIntoForm()
        {
            if (SelectedTask is null)
            {
                return;
            }

            Title = SelectedTask.Title;
            Description = SelectedTask.Description;
            SelectedPriority = SelectedTask.PriorityLabel;
            SelectedStatus = SelectedTask.StatusLabel;
            Message = "Tarea cargada en el formulario. Puedes actualizarla o eliminarla.";
        }

        public void ClearForm()
        {
            Title = string.Empty;
            Description = string.Empty;
            SelectedPriority = Labels.Priorities[0];
            SelectedStatus = Labels.Statuses[0];
            SelectedTask = null;
            Message = "Formulario limpio. Escribe una tarea nueva.";
        }

        private TaskItem BuildTaskFromForm()
        {
            return new TaskItem(
                Title,
                Description,
                Labels.ParsePriority(SelectedPriority),
                Labels.ParseStatus(SelectedStatus));
        }

        private void Reload()
        {
            _tasks.Clear();
            foreach (TaskItem task in _repository.GetAll())
            {
                _tasks.Add(task);
            }

            SelectedTask = null;
            HasTasks = _tasks.Count > 0;
            OnPropertyChanged(nameof(IsEmpty));
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
