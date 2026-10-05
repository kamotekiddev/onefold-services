#!/usr/bin/env bash

set -euo pipefail

BRANCH="${GITHUB_REF_NAME:-}"
EVENT_NAME="${GITHUB_EVENT_NAME:-}"
TARGET_ENV="${INPUT_ENVIRONMENT:-}"

# Automatic push to master → dev
if [[ "$EVENT_NAME" == "push" ]]; then

    if [[ "$BRANCH" == "master" ]]; then
        echo "environment=dev"
        exit 0
    fi

    echo "Unsupported push branch: $BRANCH" >&2
    exit 1
fi

# Manual deployment
if [[ -z "$TARGET_ENV" ]]; then
    echo "Deployment environment is required." >&2
    exit 1
fi

# master → dev only
if [[ "$BRANCH" == "master" ]]; then

    if [[ "$TARGET_ENV" != "dev" ]]; then
        echo "master can only be deployed to dev." >&2
        exit 1
    fi

    echo "environment=dev"
    exit 0
fi

# feature/* → dev only
if [[ "$BRANCH" == feature/* ]]; then

    if [[ "$TARGET_ENV" != "dev" ]]; then
        echo "Feature branches can only be deployed to dev." >&2
        exit 1
    fi

    echo "environment=dev"
    exit 0
fi

# release/* → dev or prod
if [[ "$BRANCH" == release/* ]]; then

    if [[ "$TARGET_ENV" != "dev" && "$TARGET_ENV" != "prod" ]]; then
        echo "Release branches can only be deployed to dev or prod." >&2
        exit 1
    fi

    echo "environment=$TARGET_ENV"
    exit 0
fi

echo "Unsupported branch for deployment: $BRANCH" >&2
exit 1