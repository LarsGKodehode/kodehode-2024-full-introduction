# Azure Provider source and version being used
terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=4.7.0"
    }
  }
}

# Configure the Microsoft Azure Provider
provider "azurerm" {
  features {}
  subscription_id = var.subscription_id
}

# Generate a random string to ensure unique names
resource "random_string" "rs" {
  length  = 8
  special = false
  upper   = false

  # This ensures the value is stable across applies unless this value changes.
  keepers = {
    seed = var.random_seed
  }
}

# Create a Resource Group
resource "azurerm_resource_group" "example_resource_group" {
  name     = "test-resource-group-${random_string.rs.result}"
  location = "norwayeast"
}

# Create a Service Plan
resource "azurerm_service_plan" "example_service_plan" {
  name                = "test-service-plan-${random_string.rs.result}"
  os_type             = "Linux"
  sku_name            = "F1"
  location            = azurerm_resource_group.example_resource_group.location
  resource_group_name = azurerm_resource_group.example_resource_group.name
}

# Create a Web App
resource "azurerm_linux_web_app" "example_web_app" {
  name                = "test-web-app-${random_string.rs.result}"
  location            = azurerm_resource_group.example_resource_group.location
  resource_group_name = azurerm_resource_group.example_resource_group.name
  service_plan_id     = azurerm_service_plan.example_service_plan.id

  site_config {
    always_on = false

    application_stack {
      dotnet_version = "8.0"
    }
  }
}

#  Deploy code from a public GitHub repo
resource "azurerm_app_service_source_control" "example_sourcecontrol" {
  app_id   = azurerm_linux_web_app.example_web_app.id
  repo_url = var.source_url
  branch   = var.source_branch

  use_manual_integration = false

  github_action_configuration {
    generate_workflow_file = false
  }
}
