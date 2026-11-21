using System;
using System.Collections.Generic;

namespace Gestor_de_Tareas.Modelos
{
    // Represents one task. Its data can only be changed through its methods.
    public class TaskItem
    {
        public TaskItem(string title, string description, Priority priority, Status status)
            : this(0, title, description, priority, status, DateTime.Now)
        {
        }

        public TaskItem(int id, string title, string description, Priority priority, Status status, DateTime createdAt)
        {
            Id = id;
            Title = title;
            Description = description;
            Priority = priority;
            Status = status;
            CreatedAt = createdAt;
        }

        public int Id { get; private set; }

        public string Title { get; private set; }

        public string Description { get; private set; }

        public Priority Priority { get; private set; }

        public Status Status { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public string PriorityLabel => Labels.Describe(Priority);

        public string StatusLabel => Labels.Describe(Status);

        public IReadOnlyList<string> Validate()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(Title))
            {
                errors.Add("El titulo es obligatorio.");
            }
            else if (Title.Length > 100)
            {
                errors.Add("El titulo no puede superar los 100 caracteres.");
            }

            if (Description.Length > 500)
            {
                errors.Add("La descripcion no puede superar los 500 caracteres.");
            }

            return errors;
        }

        public void Update(string title, string description, Priority priority, Status status)
        {
            Title = title;
            Description = description;
            Priority = priority;
            Status = status;
        }
    }
}
