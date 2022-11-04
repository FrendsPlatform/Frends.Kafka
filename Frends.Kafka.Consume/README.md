# Frends.Kafka.Consume
Frends Task for Kafka consume operation.

[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Build](https://github.com/FrendsPlatform/Frends.Kafka/actions/workflows/Consume_build_and_test_on_main.yml/badge.svg)](https://github.com/FrendsPlatform/Frends.Kafka/actions)
![MyGet](https://img.shields.io/myget/frends-tasks/v/Frends.Kafka.Consume)
![Coverage](https://app-github-custom-badges.azurewebsites.net/Badge?key=FrendsPlatform/Frends.Kafka/Frends.Kafka.Consume|main)

# Installing

You can install the Task via Frends UI Task View or you can find the NuGet package from the following NuGet feed https://www.myget.org/F/frends-tasks/api/v2.

## Building


Rebuild the project

`dotnet build`

Run tests

 Run command in .\Frends.Kafka.Send.Tests\Files to create a local Kafka service to docker:
`docker-compose up -d`
 
`dotnet test`


Create a NuGet package

`dotnet pack --configuration Release`

# License
This task is using Confluent.Kafka which is licensed under Apache 2.0 (..\LICENSE-Apache2.0).