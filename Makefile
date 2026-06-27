SHELL := /bin/bash
.SHELLFLAGS := -euo pipefail -c
.ONESHELL:

PROJECT_DIR := src
PROJECT := delaware.csproj
STATE_FILE := .delaware.env

-include $(STATE_FILE)

RANDOM_SUFFIX := $(shell bash -c 'echo $$RANDOM')
LOCATION ?= westus
RESOURCE_GROUP ?= rg-delaware-$(RANDOM_SUFFIX)
SEARCH_SERVICE_NAME ?= search-delaware-$(RANDOM_SUFFIX)
SEARCH_ENDPOINT ?= https://$(SEARCH_SERVICE_NAME).search.windows.net
SEARCH_SKU ?= basic
PARTITION_COUNT ?= 1
REPLICA_COUNT ?= 1

.PHONY: provision run

provision:
	az group create \
		--name "$(RESOURCE_GROUP)" \
		--location "$(LOCATION)"

	az search service create \
		--name "$(SEARCH_SERVICE_NAME)" \
		--resource-group "$(RESOURCE_GROUP)" \
		--location "$(LOCATION)" \
		--sku "$(SEARCH_SKU)" \
		--partition-count "$(PARTITION_COUNT)" \
		--replica-count "$(REPLICA_COUNT)"

	cat > "$(STATE_FILE)" <<EOF
	RESOURCE_GROUP := $(RESOURCE_GROUP)
	LOCATION := $(LOCATION)
	SEARCH_SERVICE_NAME := $(SEARCH_SERVICE_NAME)
	SEARCH_ENDPOINT := $(SEARCH_ENDPOINT)
	SEARCH_SKU := $(SEARCH_SKU)
	PARTITION_COUNT := $(PARTITION_COUNT)
	REPLICA_COUNT := $(REPLICA_COUNT)
	EOF

	export DELAWARE_AZURE_RESOURCE_GROUP="$(RESOURCE_GROUP)"
	export DELAWARE_AZURE_LOCATION="$(LOCATION)"
	export DELAWARE_AZURE_SEARCH_SERVICE_NAME="$(SEARCH_SERVICE_NAME)"
	export DELAWARE_AZURE_SEARCH_ENDPOINT="$(SEARCH_ENDPOINT)"

	echo "Search endpoint: $$DELAWARE_AZURE_SEARCH_ENDPOINT"

run:
	[[ -f "$(STATE_FILE)" ]] || { echo "Run 'make provision' first." >&2; exit 1; }

	SEARCH_ADMIN_KEY=$$(az search admin-key show \
		--service-name "$(SEARCH_SERVICE_NAME)" \
		--resource-group "$(RESOURCE_GROUP)" \
		--query primaryKey \
		--output tsv)

	export DELAWARE_AZURE_RESOURCE_GROUP="$(RESOURCE_GROUP)"
	export DELAWARE_AZURE_LOCATION="$(LOCATION)"
	export DELAWARE_AZURE_SEARCH_SERVICE_NAME="$(SEARCH_SERVICE_NAME)"
	export DELAWARE_AZURE_SEARCH_ENDPOINT="$(SEARCH_ENDPOINT)"
	export DELAWARE_AZURE_SEARCH_ADMIN_KEY="$$SEARCH_ADMIN_KEY"

	cd "$(PROJECT_DIR)"
	dotnet run --project "$(PROJECT)"
