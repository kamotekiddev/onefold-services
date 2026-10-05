#!/usr/bin/env bash

set -euo pipefail

BRANCH="${GITHUB_REF_NAME:-}"
EVENT_NAME="${GITHUB_EVENT_NAME:-}"
TARGET_ENV="${INPUT_ENVIRONMENT:-}"

# --------------------------------------------------
# Automatic push
# --------------------------------------------------

if [[ "$EVENT_NAME" == "push" ]]; then

    if [[ "$BRANCH" == "master" ]]; then
        echo "dev"
        exit 0
    fi

    echo "Unsupported push branch: $BRANCH" >&2
    exit 1
fi


# --------------------------------------------------
# Manual deployment
# --------------------------------------------------

if [[ -z "$TARGET_ENV" ]]; then
    echo "Deployment environment is required." >&2
    exit 1
fi


# --------------------------------------------------
# Any branch can deploy to dev
# --------------------------------------------------

if [[ "$TARGET_ENV" == "dev" ]]; then
    echo "dev"
    exit 0
fi


# --------------------------------------------------
# Only release/* can deploy to prod
# --------------------------------------------------

if [[ "$TARGET_ENV" == "prod" ]]; then

    if [[ "$BRANCH" == release/* ]]; then
        echo "prod"
        exit 0
    fi

    echo "Only release/* branches can be deployed to prod." >&2
    exit 1
fi


echo "Unsupported deployment environment: $TARGET_ENV" >&2
exit 1