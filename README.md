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



## 🚀 Project Overview

🚀 Real-Time Chat Application – <span style="color:#00bfff;">DevOps Pipeline</span>

A fully containerized, microservices-based real-time chat system built with a modern
<strong><span style="color:#ff9800;">CI/CD + Cloud-Native DevOps Pipeline</span></strong>.
This project demonstrates how real systems are built → tested → deployed → monitored at scale.

🧩 Microservices Architecture

This system contains four independent microservices — each deployed & scaled separately:

🔐 Authentication Service
<span style="color:#9c27b0;">Handles login, registration, and JWT tokens</span>

👤 User Service
<span style="color:#4caf50;">Manages user profiles & user-related data</span>

💬 Chatting Service
<span style="color:#2196f3;">Handles real-time chat operations</span>

📨 Notification Service
<span style="color:#f44336;">Sends real-time notifications & updates</span>

🛠️ DevOps & Cloud-Native Stack
🐳 Docker

Containerized microservices

<span style="color:#03a9f4;">Reproducible builds</span>

Lightweight & portable

⚙️ GitHub Actions – CI/CD

Automated builds on every push

Test workflows per service

Auto-publish images to Docker Hub

<span style="color:#ffa726;">Continuous delivery → Kubernetes</span>

☸️ Kubernetes

Deployment orchestration

<strong><span style="color:#4caf50;">Rolling updates + self-healing</span></strong>

Service discovery & load balancing

Horizontal scaling

📊 Prometheus Monitoring

Real-time service metrics

Performance and health dashboards

Alerting-ready architecture

🌟 Features

✨ <strong><span style="color:#ffeb3b;">Microservices architecture</span></strong>
✨ Real-time chat
✨ Full CI/CD pipeline
✨ Dockerized & cloud-native
✨ Kubernetes-ready
✨ <span style="color:#80deea;">Scalable & resilient</span>
✨ Prometheus monitoring

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

