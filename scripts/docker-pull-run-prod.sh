#!/bin/bash
dir=$(cd -P -- "$(dirname -- "$0")" && pwd -P)
cd $dir
cd ..
docker pull localhost:5000/pwr-msi_nginx:latest
docker pull localhost:5000/pwr-msi_backend:latest
docker pull localhost:5000/pwr-msi_msipay:latest
docker tag localhost:5000/pwr-msi_nginx:latest pwr-msi_nginx:latest
docker tag localhost:5000/pwr-msi_backend:latest pwr-msi_backend:latest
docker tag localhost:5000/pwr-msi_msipay:latest pwr-msi_msipay:latest
docker-compose --env-file .env.prod -f docker-compose.prod.yml up -d
