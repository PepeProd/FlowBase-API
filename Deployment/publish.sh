#!/bin/bash
cd ..

version=$(date +%Y%m%d%H%M%s).git.$(git rev-parse --short HEAD)
image=bryansmi/flowbaseapi

docker build -t $image:$version .
docker kill "flowbaseapi_validate"
docker rm "flowbaseapi_validate"
echo "$version" > Deployment/version.txt

docker run -d --name "flowbaseapi_validate" -p 5000:5000 $image:$version; sleep 5
curl --retry-delay 3 --retry 10 -v http://localhost:5000/chemicals
printf "\n"

read -p "DockerHub username: " username 
read -s -p "DockerHub password: " password
printf "\n"

docker login --username=$username --password=$password
docker push $image:$version
