# Gestor de tareas

## Informacion General

- **Nombre del proyecto:** Gestor de tareas (`Gestor-de-Tareas`).
- **Nombre del estudiante:** Isaac Laguna.
- **Descripcion del software:** aplicacion de escritorio que permite administrar tareas, asignarles una prioridad y un estado, y conservarlas en una base de datos local.
- **Problema o necesidad que resuelve:** llevar un registro sencillo de actividades sin depender de hojas de calculo ni de servicios en la nube. Evita perder tareas al cerrar el programa y facilita ver cual esta pendiente, en curso o completada.
- **Objetivo principal:** practicar Programacion Orientada a Objetos, la construccion de interfaces con Avalonia UI y la persistencia de datos con ADO.NET y SQLite en una sola aplicacion funcional.

## Funcionalidades

La interfaz muestra un formulario a la izquierda y la lista de tareas a la derecha. El usuario puede:

- **Crear (Create):** escribir titulo y descripcion, elegir prioridad (Baja, Media, Alta) y estado (Pendiente, En curso, Completada), y presionar **Guardar**.
- **Consultar (Read):** ver todas las tareas guardadas en la lista, ordenadas por identificador.
- **Actualizar (Update):** seleccionar una tarea de la lista; el formulario se carga con sus datos, se editan los campos y se presiona **Actualizar**.
- **Eliminar (Delete):** seleccionar una tarea y presionar **Eliminar**.
- **Limpiar el formulario:** el boton **Nuevo** deja los campos listos para una tarea nueva.
- **Validar entradas:** el titulo es obligatorio y no supera 100 caracteres; la descripcion no supera 500. Los errores se muestran con un mensaje comprensible en la parte inferior.
- **Recibir notificaciones:** cada operacion muestra un mensaje de confirmacion o de error en la barra inferior de la ventana.
- **Conservar datos:** los registros sobreviven al cierre y a una nueva ejecucion del programa.

## Tecnologias Utilizadas

- **.NET:** SDK 10.0 (el proyecto tiene `TargetFramework net10.0`).
- **Lenguaje:** C#.
- **Interfaz grafica:** Avalonia UI 11.3.12 (`Avalonia`, `Avalonia.Desktop`, `Avalonia.Themes.Fluent`, `Avalonia.Fonts.Inter` y `Avalonia.Diagnostics`).
- **Acceso a datos:** ADO.NET mediante el proveedor `Microsoft.Data.Sqlite` 11.0.0.
- **Base de datos:** SQLite (motor embebido, sin servidor).
- **No se utiliza Entity Framework ni otro ORM:** todas las instrucciones SQL se ejecutan desde C# con `SqliteConnection`, `SqliteCommand` y `SqliteDataReader`.

## Estructura del proyecto

```text
Gestor-de-Tareas/
├── AGENTS.md
├── App.axaml / App.axaml.cs          Configuracion global; inicializa la base de datos
├── Program.cs                        Punto de entrada de Avalonia
├── MainWindow.axaml                  Interfaz definida en XAML
├── MainWindow.axaml.cs               Eventos de los botones y de la lista
├── Gestor-de-Tareas.csproj           Paquetes y configuracion del proyecto
├── Gestor-de-Tareas.slnx             Solucion de Visual Studio
├── Modelos/                          Clases del dominio
│   ├── Tarea.cs                      Entidad TaskItem (datos, validacion, Update)
│   ├── Prioridad.cs                  Enum Priority
│   ├── Estado.cs                     Enum Status
│   └── Etiquetas.cs                  Clase Labels (enum -> texto en espanol)
├── Datos/                            Acceso a SQLite con ADO.NET
│   ├── BaseDeDatos.cs                Clase Database: ruta, conexion y CREATE TABLE
│   ├── IRepositorioTareas.cs         Interface ITaskRepository (contrato CRUD)
│   └── RepositorioTareas.cs          Clase SqliteTaskRepository (SQL parametrizado)
└── ModelosDeVista/
    └── ModeloVistaPrincipal.cs       Clase MainViewModel: estado de la UI y acciones
```

### Clases principales y sus responsabilidades

| Clase | Responsabilidad |
| --- | --- |
| `TaskItem` | Representa una tarea. Guarda `Id`, `Title`, `Description`, `Priority`, `Status` y `CreatedAt`; valida los datos con `Validate()` y los modifica con `Update()`. |
| `Priority`, `Status` | Enumeraciones que limitan los valores permitidos de prioridad y estado. |
| `Labels` | Traduce los enumeradores a texto en espanol para la interfaz y viceversa. |
| `Database` | Construye la cadena de conexion, abre conexiones y crea la tabla si no existe. |
| `ITaskRepository` | Contrato con las cuatro operaciones CRUD; permite desacoplar la interfaz de la base de datos. |
| `SqliteTaskRepository` | Implementa el contrato con comandos SQL parametrizados. |
| `MainViewModel` | Mantiene los datos que muestra la ventana, avisa de cambios con `INotifyPropertyChanged` y ejecuta crear, actualizar, eliminar y cargar. |
| `MainWindow` | Define la ventana, enlaza el `DataContext` con `MainViewModel` y conecta los eventos XAML con sus metodos. |
| `App` | Crea la ventana principal y llama a `Database.Initialize()` al iniciar. |

### Como interactuan las clases

`Program` inicia Avalonia y `App` prepara la base de datos. `MainWindow` crea un `SqliteTaskRepository` y se lo entrega al constructor de `MainViewModel` (inyeccion de dependencias); por eso el modelo de vista conoce la interface `ITaskRepository` pero no sabe nada de SQL. La ventana enlaza sus controles al modelo de vista mediante `{Binding ...}`. Cuando el usuario pulsa un boton, el evento `Click` definido en XAML llama al metodo correspondiente de `MainWindow.axaml.cs`, que delega en `MainViewModel`; este valida, llama al repositorio y vuelve a cargar la lista. El evento `SelectionChanged` de la lista llama a `LoadSelectedTaskIntoForm()` para editar la tarea seleccionada.

### Donde se aplican los conceptos de POO

- **Clases y objetos:** `TaskItem` es la entidad del dominio; `MainViewModel` y `SqliteTaskRepository` son objetos con responsabilidades distintas.
- **Propiedades y constructores:** `TaskItem` tiene dos constructores (uno para una tarea nueva y otro para una tarea leida de la base de datos).
- **Encapsulamiento:** las propiedades de `TaskItem` tienen `private set`; los cambios pasan por `Update()` y `Validate()`, de modo que la clase controla su propio estado.
- **Abstraccion y polimorfismo:** `ITaskRepository` separa el "que hacer" (CRUD) del "como hacerlo" (SQL); el modelo de vista podria usar otra implementacion sin cambiar.
- **Separacion de responsabilidades:** la vista (`MainWindow`), la logica (`MainViewModel`, `TaskItem`) y el acceso a datos (`Database`, `SqliteTaskRepository`) estan en carpetas distintas.
- **SOLID:** dependencia de una interface en lugar de una clase concreta (DIP), una sola razon de cambio por clase (SRP) y metodos cortos y concretos.

## Instalacion de dependencias

1. Instalar el **SDK de .NET 10.0** desde <https://dotnet.microsoft.com/download>. Se puede comprobar con `dotnet --version`.
2. Obtener el codigo desde GitHub:

   ```bash
   git clone <URL-del-repositorio> Gestor-de-Tareas
   cd Gestor-de-Tareas
   ```

3. Restaurar las dependencias (descarga los paquetes NuGet indicados en `Gestor-de-Tareas.csproj`):

   ```bash
   dotnet restore
   ```

4. Paquetes utilizados: Avalonia 11.3.12 (`Avalonia`, `Avalonia.Desktop`, `Avalonia.Themes.Fluent`, `Avalonia.Fonts.Inter`, `Avalonia.Diagnostics`), `Tmds.DBus.Protocol` 0.21.3 y `Microsoft.Data.Sqlite` 11.0.0. No hay que instalarlos a mano: `dotnet restore` los obtiene desde NuGet.
5. Configuracion adicional: **ninguna**. No se necesita servidor de base de datos ni archivo de configuracion; el archivo SQLite se crea automaticamente la primera vez que se ejecuta la aplicacion.

## Ejecucion del programa

Desde la carpeta raiz del repositorio (donde esta `Gestor-de-Tareas.csproj`):

```bash
dotnet restore
dotnet run
```

Tambien se puede abrir `Gestor-de-Tareas.slnx` en Visual Studio y presionar F5, o ejecutar `dotnet build` y luego el ejecutable generado en `bin/Debug/net10.0/`. Al iniciar se crea la base de datos y aparece la ventana con el formulario y la lista de tareas.

## Base de datos

- **Que informacion almacena:** cada tarea con su titulo, descripcion, prioridad, estado y fecha de creacion.
- **Tabla y campos:** una unica tabla `Tasks`.

  | Campo | Tipo | Descripcion |
  | --- | --- | --- |
  | `Id` | `INTEGER PRIMARY KEY AUTOINCREMENT` | Identificador unico generado por SQLite. |
  | `Title` | `TEXT NOT NULL` | Titulo de la tarea. |
  | `Description` | `TEXT NOT NULL` | Descripcion opcional. |
  | `Priority` | `INTEGER NOT NULL` | Prioridad guardada como `0=Baja`, `1=Media`, `2=Alta`. |
  | `Status` | `INTEGER NOT NULL` | Estado guardado como `0=Pendiente`, `1=En curso`, `2=Completada`. |
  | `CreatedAt` | `TEXT NOT NULL` | Fecha de creacion en formato ISO 8601. |

- **Conexion:** `Database.CreateConnection()` devuelve un `SqliteConnection` abierto usando `Data Source=<ruta del archivo>`. La ruta se calcula con `AppContext.BaseDirectory`, por lo que el archivo queda junto al ejecutable (`bin/Debug/net10.0/gestor_tareas.db` al usar `dotnet run`).
- **Operaciones CRUD:** estan en `Datos/RepositorioTareas.cs`, en los metodos `GetAll()`, `Create()`, `Update()` y `Delete()`. Los valores que introduce el usuario se envian siempre con `command.Parameters.AddWithValue(...)`; nunca se concatenan directamente en la cadena SQL. Las conexiones y los comandos se liberan con `using`.
- **Creacion e inicializacion:** `Database.Initialize()` ejecuta un `CREATE TABLE IF NOT EXISTS Tasks (...)` al iniciar la aplicacion, desde `App.OnFrameworkInitializationCompleted()`. Si el archivo ya existe, la tabla se conserva y no se borran los registros.
- **Persistencia comprobada:** al crear una tarea, cerrar la aplicacion y volver a abrirla, la tarea sigue en la lista y el identificador autoincremental continua.

## Problemas encontrados y soluciones

1. **Enlaces compilados de Avalonia.** La plantilla del proyecto activa `AvaloniaUseCompiledBindingsByDefault`, por lo que los `{Binding ...}` necesitan conocer el tipo de datos en tiempo de compilacion. Solucion: se declararon los espacios de nombres `vm` y `modelos` en `MainWindow.axaml` y se agrego `x:DataType="vm:MainViewModel"` en la ventana y `x:DataType="modelos:TaskItem"` en la plantilla de la lista. Asi los enlaces a `Title`, `PriorityLabel` y `StatusLabel` funcionan sin errores.

2. **Fechas que podian cambiar entre ejecuciones.** SQLite no tiene un tipo de fecha nativo en este escenario, y el formato depende de la cultura del equipo. Solucion: guardar `CreatedAt` como texto con `ToString("o", CultureInfo.InvariantCulture)` y leerlo con `DateTime.Parse(..., CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)`. De esta forma la fecha se recupera exactamente igual despues de reiniciar la aplicacion.

3. **Evitar codigo SQL inseguro y fugas de recursos.** Concatenar los campos del formulario en la consulta podia romper el SQL o permitir inyeccion. Solucion: usar parametros (`$title`, `$description`, etc.) en todos los comandos y envolver conexiones, comandos y lectores en bloques `using`.
