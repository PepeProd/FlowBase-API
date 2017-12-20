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


echo "foobar!"
