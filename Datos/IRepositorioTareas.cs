using System.Collections.Generic;
using Gestor_de_Tareas.Modelos;

namespace Gestor_de_Tareas.Datos
{
    // Abstraction used by the view model, so it does not depend on SQL details.
    public interface ITaskRepository
    {
        IReadOnlyList<TaskItem> GetAll();

        int Create(TaskItem task);

        void Update(TaskItem task);

        void Delete(int id);
    }
}
