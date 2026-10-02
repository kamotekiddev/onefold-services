#!/usr/bin/env bash

set -euo pipefail

ENVIRONMENT="${1:?Environment is required. Usage: ./configure-appservice.sh dev|prod}"

RESOURCE_GROUP="d-one-labs"
APP_SERVICE_NAME="onefold-api-${ENVIRONMENT}"
SETTINGS_FILE="$(dirname "$0")/appsettings.json"

echo "Environment: $ENVIRONMENT"
echo "App Service: $APP_SERVICE_NAME"
echo "Settings: $SETTINGS_FILE"

mapfile -t APP_SETTINGS < <(
  jq -r '.[] | "\(.key)=\(.value)"' "$SETTINGS_FILE"
)

az webapp config appsettings set \
  --resource-group "$RESOURCE_GROUP" \
  --name "$APP_SERVICE_NAME" \
  --settings "${APP_SETTINGS[@]}"

echo "App Service configuration updated successfully."