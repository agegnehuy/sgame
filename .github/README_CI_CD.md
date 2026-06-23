# CI/CD Configuration

This repository includes a complete CI/CD pipeline for the Safe Steps Addis mobile game that:

## Workflow: `.github/workflows/build-deploy.yml`

**Automated Build and Release**
- Triggers on push to `main` and PRs
- Validates build configuration
- Builds Android APK with Unity 2022.3.62f3
- Creates tagged releases (v0.0.10, v0.0.11, etc.)
- Pushes containers to Docker Hub
- Publishes to GitHub Releases

## Version Tagging Strategy

**Configured to increment from 0.0.9** after your current version:

1. **Current**: v0.0.9
2. **Next**: v0.0.10  
3. **Next**: v0.0.11
4. **etc.**

The workflow detects the latest tag and increments the patch version automatically.

## Features

### ✅ Automated Builds
- Unity 2022.3.62f3 (matching project)
- Android APK targeting low/mid-tier devices
- Offline-first functionality preserved

### ✅ Release Management
- GitHub Releases with automated changelog
- Version bump commits to `main`
- Container image tagging with version
- Sequential build → release automation

### ✅ Security
- Snyk security scanning (PRs only)
- Validation steps before building
- GitHub Actions security checks

### ✅ Deployment
- Manual triggers for urgent releases
- Staging deployment for PRs
- Artifact retention and backup

## Usage

### Manual Build/Release
```bash
# Trigger workflow manually
gh workflow run build-deploy.yml

# Build only (for testing)
gh workflow run build-deploy.yml -f workflow_dispatch
```

### Local Development
```bash
# Build Android APK locally (existing script)
./build-android.sh

# For testing workflow changes
mkdir -p test-build && cd test-build
# Fork and test CI locally
```

## Required Secrets

Add these to your repository:

```env
UNITY_LICENSE="your-unity-license-key"
DOCKER_TOKEN="your-docker-registry-token"
SSH_PRIVATE_KEY="your-staging-server-key"
SNYK_TOKEN="your-snyk-api-token"
```

## Features Added

1. **Version 0.0.9 → 0.0.10**: Tagging now increments properly from your current version
2. **GitHub Container Registry Integration**: Builds and pushes Docker images
3. **Release Documentation**: Updated `plan.md` and `MVP_OVERVIEW.md` with version
4. **Multi-stage Deployment**: Separate workflows for build, test, and production
5. **Security Scanning**: Snyk integration for vulnerability checks

## Workflow Breakdown

1. **validate**: Sanity check Unity setup and dependencies
2. **build-android**: Compile APK, upload as artifact
3. **create-release**: Create GitHub release, push images
4. **deploy-staging**: Deploy to staging environment (PRs only)
5. **security-scan**: Check for vulnerabilities (PRs only)

## Version Increment Logic

```bash
# From: v0.0.9
# To: v0.0.10 (patch +1)

major=0
minor=0  
patch=9
new_patch=$((patch + 1))
new_version="$major.$minor.$new_patch"
```

The workflow enforces semantic versioning and ensures you start at v0.0.10 from your current v0.0.9 state.
