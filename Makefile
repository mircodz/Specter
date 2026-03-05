.PHONY: help build test clean pack coverage format

# Variables
VERSION := $(shell cat version)
NUPKG_DIR := ./nupkgs

help: ## Show this help message
	@echo 'Usage: make [target]'
	@echo ''
	@echo 'Available targets:'
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | awk 'BEGIN {FS = ":.*?## "}; {printf "  %-20s %s\n", $$1, $$2}'

build: ## Build the solution in Debug mode
	dotnet build specter.sln -c Debug

build-release: ## Build the solution in Release mode
	dotnet build specter.sln -c Release

test: ## Run all tests
	dotnet test specter.sln

coverage: ## Run tests with coverage report
	dotnet test specter.sln \
		/p:CollectCoverage=true \
		/p:CoverletOutputFormat=cobertura \
		/p:CoverletOutput=./TestResults/

clean: ## Clean build artifacts
	dotnet clean specter.sln || true
	rm -rf $(NUPKG_DIR)
	rm -rf */bin */obj src/*/bin src/*/obj tests/*/bin tests/*/obj

pack: clean build-release ## Pack NuGet packages
	dotnet pack src/Specter/Specter.csproj -c Release -o $(NUPKG_DIR) /p:Version=$(VERSION)
	dotnet pack src/Specter.Generator/Specter.Generator.csproj -c Release -o $(NUPKG_DIR) /p:Version=$(VERSION)
	@echo ""
	@echo "Packages created in $(NUPKG_DIR):"
	@ls $(NUPKG_DIR)/*.nupkg

setup: ## Initial setup (restore packages)
	dotnet restore

bump-major: ## Bump major version (1.0.0 -> 2.0.0)
	@echo "Current version: $(VERSION)"
	@echo $(VERSION) | awk -F. '{print $$1+1".0.0"}' > version
	@echo "New version: $$(cat version)"

bump-minor: ## Bump minor version (1.0.0 -> 1.1.0)
	@echo "Current version: $(VERSION)"
	@echo $(VERSION) | awk -F. '{print $$1"."$$2+1".0"}' > version
	@echo "New version: $$(cat version)"

bump-patch: ## Bump patch version (1.0.0 -> 1.0.1)
	@echo "Current version: $(VERSION)"
	@echo $(VERSION) | awk -F. '{print $$1"."$$2"."$$3+1}' > version
	@echo "New version: $$(cat version)"

all: clean build test ## Clean, build, and test
