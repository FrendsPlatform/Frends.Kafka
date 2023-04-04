#!/bin/bash

cd ./Frends.Kafka.Produce.Tests

FILE=docker-compose.yml
[ ! -f "$FILE" ] && curl -sSL https://raw.githubusercontent.com/bitnami/containers/main/bitnami/kafka/docker-compose.yml > docker-compose.yml

docker-compose up -d