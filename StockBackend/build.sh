#!/bin/bash

# Clean the project
rm -rf bin obj

# Build the project
dotnet build

# Run the project
dotnet run 