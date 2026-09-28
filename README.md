
# Audit Assist Infrastructure

## Technology
- Next.js frontend
- .NET API
- MsSQL
- Redis (when required)
- Docker and Docker Compose
- GitHub Actions
- GitHub Container Registry

## Environments

- Local: developer workstation
- Staging: QA and acceptance testing
- Production: live customer workloads

Staging and production must use separate databases, storage,
credentials, and deployment configurations.

## Branch and deployment policy

- feature/*: feature development
- develop: integration and staging
- main: production release

Pull requests must pass CI and receive required reviews.

## Local startup

docker compose \
  -f infrastructure/docker/compose/docker-compose.dev.yml up --build

## Local shutdown

docker compose \
  -f infrastructure/docker/compose/docker-compose.dev.yml down

## Secrets

Never commit actual credentials, production environment files,
private keys, customer documents, or database backups.

Production and staging deployments require separate credentials.

## Deployment safety

- Pin releases to immutable image tags (Git commit SHA).
- Review database migrations before deployment.
- Verify application health after deployment.
- Maintain encrypted, off-server backups.
- Test restoration before relying on backups.
- Restrict production access to authorized operators.