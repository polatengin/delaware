#!/usr/bin/env bash
set -euo pipefail

RANDOM_SUFFIX="${RANDOM}"
RESOURCE_GROUP="rg-delaware-${RANDOM_SUFFIX}"
LOCATION="westus"
SEARCH_SERVICE_NAME="search-delaware-${RANDOM_SUFFIX}"
SEARCH_ENDPOINT="https://${SEARCH_SERVICE_NAME}.search.windows.net"

az group create \
  --name "$RESOURCE_GROUP" \
  --location "$LOCATION"

az search service create \
  --name "$SEARCH_SERVICE_NAME" \
  --resource-group "$RESOURCE_GROUP" \
  --location "$LOCATION" \
  --sku "basic" \
  --partition-count 1 \
  --replica-count 1

SEARCH_ADMIN_KEY=$(az search admin-key show \
  --service-name "$SEARCH_SERVICE_NAME" \
  --resource-group "$RESOURCE_GROUP" \
  --query primaryKey \
  --output tsv)

export DELAWARE_AZURE_RESOURCE_GROUP="$RESOURCE_GROUP"
export DELAWARE_AZURE_LOCATION="$LOCATION"
export DELAWARE_AZURE_SEARCH_SERVICE_NAME="$SEARCH_SERVICE_NAME"
export DELAWARE_AZURE_SEARCH_ENDPOINT="$SEARCH_ENDPOINT"
export DELAWARE_AZURE_SEARCH_ADMIN_KEY="$SEARCH_ADMIN_KEY"
