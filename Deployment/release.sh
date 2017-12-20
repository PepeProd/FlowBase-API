#!/bin/bash

read -p "Enter environment to release {TEST|QA|PROD}: " env

echo "env: $env"
if [ $env = "TEST" ]; then
    read -p "Enter branch name to release: " branch
elif [ $env = "QA" ]; then
    branch="develop"
elif [ $env = "PROD" ]; then
    branch="master"
else
    echo "ERROR: Invalid environment name."
    exit 1
fi

echo "Branch to release: $branch" 

git checkout $branch
git pull

./publish.sh

cp deploy-task.json deploy-task-temp.json
while read -r line
do
    version="$line"
    echo "version"
done < version.txt

sed -i -e "s/flowbaseapi:*/flowbaseapi:$version/g" deploy-task-temp.json

aws ecs register-task-definition --cli-input-json file://deploy-task-temp.json

