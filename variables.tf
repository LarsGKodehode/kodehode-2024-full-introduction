variable "random_seed" {
  type        = string
  description = "Random Value Seed"
}

variable "subscription_id" {
  type        = string
  description = "Azure Subscription ID"
}

variable "source_url" {
  type        = string
  description = "Source code repository URL"
}

variable "source_branch" {
  type        = string
  description = "Branch to deploy from"
}
