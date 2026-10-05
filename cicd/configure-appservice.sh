#!/usr/bin/env bash

set -euo pipefail

ENVIRONMENT="${1:?Environment is required. Usage: ./configure-appservice.sh dev|prod}"

RESOURCE_GROUP="d-one-labs"
APP_SERVICE_NAME="onefold-api-${ENVIRONMENT}"

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SETTINGS_DIR="${SCRIPT_DIR}/app-settings-variables/${ENVIRONMENT}"

configure_slot() {
  local slot="$1"
  local settings_file="$2"

  echo ""
  echo "========================================"
  echo "Configuring App Service"
  echo "Environment: $ENVIRONMENT"
  echo "App Service: $APP_SERVICE_NAME"
  echo "Slot: $slot"
  echo "Settings: $settings_file"
  echo "========================================"

  if [[ ! -f "$settings_file" ]]; then
    echo "ERROR: Settings file not found: $settings_file"
    exit 1
  fi

  mapfile -t APP_SETTINGS < <(
    jq -r '.[] | "\(.key)=\(.value)"' "$settings_file"
  )

  if [[ "$slot" == "production" ]]; then
    az webapp config appsettings set \
      --resource-group "$RESOURCE_GROUP" \
      --name "$APP_SERVICE_NAME" \
      --settings "${APP_SETTINGS[@]}"
  else
    az webapp config appsettings set \
      --resource-group "$RESOURCE_GROUP" \
      --name "$APP_SERVICE_NAME" \
      --slot "$slot" \
      --settings "${APP_SETTINGS[@]}"
  fi

  echo "Successfully configured $slot slot."
}

# Configure production
configure_slot \
  "production" \
  "${SETTINGS_DIR}/appsettings.json"

# Configure staging
configure_slot \
  "staging" \
  "${SETTINGS_DIR}/appsettings-staging.json"

echo ""
echo "App Service configuration completed successfully."