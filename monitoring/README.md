```markdown
# Monitoring (Prometheus + Grafana)

This folder contains manifests and configs to run Prometheus and Grafana.

Options:
- Kubernetes: apply the YAMLs in k8s/monitoring (creates namespace `monitoring`)
  kubectl apply -f k8s/monitoring/namespace.yaml
  kubectl apply -f k8s/monitoring/prometheus-config.yaml
  kubectl apply -f k8s/monitoring/prometheus-deployment.yaml
  kubectl apply -f k8s/monitoring/prometheus-service.yaml
  kubectl apply -f k8s/monitoring/grafana-datasource-configmap.yaml
  kubectl apply -f k8s/monitoring/grafana-deployment.yaml
  kubectl apply -f k8s/monitoring/grafana-service.yaml

- Local Docker Compose (useful for development):
  - Copy k8s/monitoring/prometheus-config.yaml -> prometheus.yml (root or same folder) or tweak paths in docker-compose.monitoring.yml
  - Ensure grafana provisioning files exist at ./grafana/provisioning/datasources/datasource.yml
  - Run: docker compose -f docker-compose.monitoring.yml up -d

Important:
- The .NET services must expose Prometheus metrics at /metrics. See edited Program.cs files.
- Add prometheus-net.AspNetCore to each .NET service:
  from each service folder:
    dotnet add package prometheus-net.AspNetCore
```
