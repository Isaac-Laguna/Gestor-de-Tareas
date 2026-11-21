using System;
using System.Collections.Generic;

namespace Gestor_de_Tareas.Modelos
{
    // Connects the English enum values with the Spanish text shown in the UI.
    public static class Labels
    {
        private static readonly string[] PriorityLabels = { "Baja", "Media", "Alta" };
        private static readonly string[] StatusLabels = { "Pendiente", "En curso", "Completada" };

        public static IReadOnlyList<string> Priorities => PriorityLabels;

        public static IReadOnlyList<string> Statuses => StatusLabels;

        public static string Describe(Priority priority)
        {
            return PriorityLabels[(int)priority];
        }

        public static string Describe(Status status)
        {
            return StatusLabels[(int)status];
        }

        public static Priority ParsePriority(string label)
        {
            int index = Array.IndexOf(PriorityLabels, label);
            return index >= 0 ? (Priority)index : Priority.Low;
        }

        public static Status ParseStatus(string label)
        {
            int index = Array.IndexOf(StatusLabels, label);
            return index >= 0 ? (Status)index : Status.Pending;
        }
    }
}
