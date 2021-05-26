#!/bin/bash
dir=$(cd -P -- "$(dirname -- "$0")" && pwd -P)
cd $dir
cd ..
source .env.prod-nodocker
export MSI_DB_CONNECTION_STRING
dotnet ef database update --connection "$MSI_DB_CONNECTION_STRING"
