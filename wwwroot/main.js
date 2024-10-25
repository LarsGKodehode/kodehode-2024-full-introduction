// Configurations
const taskEndpoint = "/task";

// Elements from the DOM
const inputFormElement = document.querySelector("#input-form");
const taskListElement = document.querySelector("#task-list");
const taskCardTemplate = document.querySelector("#task-card-template");

// Grab the latest entries from the backend
update();

// Setup the form event listner
inputFormElement.addEventListener("submit", (event) => {
  event.preventDefault();

  const form = event.target;
  const formData = new FormData(form);

  const newTitle = formData.get("title");

  addNewTask(newTitle);

  form.reset();
});

// DOM Utilities
async function update() {
  // Fetch the latest data
  const response = await fetch(taskEndpoint);
  // Convert from common language (JSON) to JavaScript
  const taskList = await response.json();

  // Before we insert into the DOM, clean it out
  taskListElement.innerHTML = "";

  // Create a new element for each entry
  for (const task of taskList) {
    // Create a new card element
    const newCard = createCard(task);

    // Insert it into the DOM
    taskListElement.append(newCard);
  }
}

function createCard(task) {
  // Make a copy of the template
  const newCard = taskCardTemplate.content.cloneNode(true);

  // configure it
  const container = newCard.querySelector("li");
  container.classList.toggle("bg-green", task.isComplete);
  container.classList.toggle("bg-gray", !task.isComplete);

  const titleElement = newCard.querySelector("p");
  titleElement.textContent = task.title;

  // Add (bind) event listners
  const deleteButton = newCard.querySelector("[data-card='delete']");
  deleteButton.addEventListener("click", () => {
    removeTask(task.id);
  });

  const toggleComplete = newCard.querySelector("[data-card='update']");
  toggleComplete.addEventListener("click", () => {
    updateTask(task.id, {
      title: task.title,
      isComplete: !task.isComplete,
    });
  });

  return newCard;
}

// Functions for the API
async function addNewTask(title) {
  // Create a new Task object
  const newTask = {
    title: title,
  };

  // Send the message
  await fetch(taskEndpoint, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    // Convert to a JSON string
    body: JSON.stringify(newTask),
  });

  // Update the DOM
  await update();
}

async function removeTask(id) {
  // Send the message
  await fetch(`${taskEndpoint}/${id}`, {
    method: "DELETE",
  });

  // Update the DOM
  await update();
}

async function updateTask(id, updatedTask) {
  // Send the message
  await fetch(`${taskEndpoint}/${id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    // Convert to a JSON string
    body: JSON.stringify(updatedTask),
  });

  // Update the DOM
  await update();
}
