CI workflows: local vs full

Files added:
- `ci-local.yml` — manual workflow that builds images locally (no push). Use this to validate builds on GitHub Actions or to manually run builds without publishing images.
- `ci-cd.yml` — full CI/CD workflow that runs on pushes to `main`, builds and pushes images to Docker Hub.

How to run locally
- Option A — manual GitHub Actions dispatch (run from Actions UI): open the `Local CI (manual)` workflow and click "Run workflow".
- Option B — run locally using `act` (third-party runner): install `act` and run:

```bash
# from the repo root
act -j build-local --secret DOCKERHUB_USERNAME=<your-user> --secret DOCKERHUB_TOKEN=<your-token>
```

Secrets required for `ci-cd.yml`
- `DOCKERHUB_USERNAME` — Docker Hub username
- `DOCKERHUB_TOKEN` — Docker Hub access token/password

Notes
- `ci-local.yml` builds images with `push: false` and `load: true`, producing images on the runner. When run on `ubuntu-latest` GitHub-hosted runners the images exist only during the job.
- `ci-cd.yml` tags images with `latest` and `${{ github.sha }}` and uses registry cache to speed up rebuilds.
- If you want automated deployment after push, add a `deploy` job and provide cloud / kube credentials (example placeholder is in the file).
