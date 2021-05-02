#!/bin/bash
dir=$(cd -P -- "$(dirname -- "$0")" && pwd -P)
cd $dir
cd ..
echo "Ensure the repository is at localhost:5000 (try ssh -L 5000:localhost:5000 destination)"
read confirm
docker-compose -f docker-compose.prod.yml build
docker tag pwr-msi_nginx:latest localhost:5000/pwr-msi_nginx:latest
docker tag pwr-msi_backend:latest localhost:5000/pwr-msi_backend:latest
docker tag pwr-msi_msipay:latest localhost:5000/pwr-msi_msipay:latest
docker push localhost:5000/pwr-msi_nginx:latest
docker push localhost:5000/pwr-msi_backend:latest
docker push localhost:5000/pwr-msi_msipay:latest
