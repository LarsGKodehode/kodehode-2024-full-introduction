var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// These two allows us to serve the static files from the wwwroot directory
// ie. index.html, style.css, main.js
app.UseDefaultFiles();
app.UseStaticFiles();

// This will be the list that stores our Tasks
// mind it's only in memory here, so it will reset when the program restarts
var taskList = new List<Task>();

// All of these app.MapXXX specifies what should happen
// when the server recieves a HTTP request at
// the specified path

// Handling creating a new task and storing it
app.MapPost("/task", (Task task) =>
{
  taskList.Add(task);

  return Results.Ok(task);
});

// Handles returning the stored Tasks
app.MapGet("/task", () =>
{
  return Results.Ok(taskList);
});

// Handles deleting a task
app.MapDelete("/task/{Id}", (int Id) =>
{
  // To delete a specific Task we first need to find it,
  // lists store them by an index so we return it's position
  int taskIndex = taskList.FindIndex(task => task.Id == Id);

  // We might not have a task with the id (we don't controll what can be sent to us)
  if (taskIndex == -1)
  {
    // In that case return a 404 Not Found result
    return Results.NotFound();
  }

  // Otherwise delete it
  taskList.RemoveAt(taskIndex);

  return Results.Ok();
});

// Handles updating a task
app.MapPut("/task/{Id}", (int Id, Task updatedTask) =>
{
  // Same logic applies here, first find out where it's stored
  Task? task = taskList.Find(task => task.Id == Id);

  // We might not have a task with the id (we don't controll what can be sent to us)
  if (task == null)
  {
    // In that case return a 404 Not Found result
    return Results.NotFound();
  }

  // Otherwise we have the task, so make the update
  task.Title = updatedTask.Title;
  task.IsComplete = updatedTask.IsComplete;

  return Results.Ok(task);
});

// Everything before this was to configure the server
// This is the point where it actually starts listening
// for HTTP requests
app.Run();


// This is the Task object definition (class)
// Here we specify what a Task consists of
// and logic related to creating it
// along with how to update it
class Task
{
  // This is a special variable, the static keyword
  // makes it something shared between instances
  // so we can use it to create new Identifiers
  // without risking them overlapping
  private static int _startId = 0;

  // This three properties store the information
  // for the Task. The {get; set;} is defined
  // and allows us to specify logic for how
  // to read from them as well as logic for
  // updating their content
  public int Id { get; init; }
  public bool IsComplete { get; set; }
  public required string Title { get; set; }

  // This is the constructor, it's a method
  // that allow us to specify how new instances
  // will be created. It's quite simple here.
  public Task(string title)
  {
    Id = _startId++;
    IsComplete = false;
    Title = title;
  }
}
