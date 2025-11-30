# 🚀 DevOps Pipeline for a Scalable Microservices Chat System

<p align="center">
  <img src="./devops.png" alt="DevOps Lifecycle" style="width:85%;max-width:1100px;border-radius:14px;box-shadow:0 10px 30px rgba(0,0,0,0.25)"/>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Docker-Containerization-blue" alt="Docker"/>
  <img src="https://img.shields.io/badge/Monitoring-Prometheus-orange" alt="Prometheus"/>
  <img src="https://img.shields.io/badge/Dashboard-Grafana-yellow" alt="Grafana"/>
  <img src="https://img.shields.io/badge/Database-SQLite-lightgrey" alt="SQLite"/>
</p>

---

Real-Time Chat Application – DevOps Pipeline

This project showcases a complete DevOps pipeline for a microservices-based real-time chat application.
It demonstrates how modern DevOps practices are applied to build, test, deploy, and monitor a scalable cloud-native system.

🧩 Microservices Architecture

The system consists of four independent microservices:

🔐 Authentication Service – Handles user login, registration, and token management

👤 User Service – Manages user profiles and user-related data

💬 Chatting Service – Processes real-time chat operations

📨 Notification Service – Sends real-time notifications and updates

Each service is containerized and deployed independently for maximum scalability.

🛠️ DevOps Stack

This project integrates a full modern DevOps toolchain:

Docker

Containerization for all microservices

Reproducible environments and consistent builds

GitHub Actions (CI/CD)

Automated build and test pipelines

Push-to-deploy workflow (Docker Hub / Kubernetes)

Kubernetes

Deployment, scaling, and service orchestration

Supports rolling updates and auto-restarts

Prometheus

Real-time monitoring

Microservice-level metrics collection

🚀 Features

Microservices architecture

Real-time chat functionality

Automated CI/CD pipeline

Containerized and cloud-native

Scalable and observable system
---

## 🚀 Project Overview

The **Chat System** is built using a **microservices architecture**, where each service (authentication, messaging, user management, and notifications) runs independently and communicates through **RabbitMQ**.  

Each service is **containerized** with Docker, orchestrated by **Kubernetes**, and automatically deployed to **AWS** through a **CI/CD pipeline** powered by **GitHub Actions**.

---

## 🧩 Architecture

```mermaid
flowchart TD
    Client[Client UI] --> Gateway[API Gateway]
    Gateway --> Auth[Auth Service]
    Gateway --> Chat[Chat Service]
    Gateway --> User[User Service]
    Gateway --> Notify[Notification Service]
    
    Chat --> MQ[(RabbitMQ)]
    Notify --> MQ
    Auth --> DB1[(Database)]
    User --> DB2[(Database)]
    Chat --> DB3[(Database)]

    subgraph AWS_EKS["AWS EKS Cluster"]
        Gateway
        Auth
        Chat
        User
        Notify
        MQ
        DB1
        DB2
        DB3
    end

