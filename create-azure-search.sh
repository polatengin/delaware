#!/usr/bin/env bash
set -euo pipefail

RANDOM_SUFFIX="${RANDOM}"
RESOURCE_GROUP="rg-delaware-${RANDOM_SUFFIX}"
LOCATION="westus"
SEARCH_SERVICE_NAME="search-delaware-${RANDOM_SUFFIX}"

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

echo "Search endpoint: https://${SEARCH_SERVICE_NAME}.search.windows.net"
