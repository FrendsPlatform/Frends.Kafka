cd ./Frends.Kafka.Produce.Tests

$file = Join-Path -Path ($pwd).path -ChildPath "\Prebuild\docker-compose.yml"
if (-not(Test-Path -Path $file -PathType Leaf)) {
	curl -sSL https://raw.githubusercontent.com/bitnami/containers/main/bitnami/kafka/docker-compose.yml > docker-compose.yml
}

docker-compose up -d