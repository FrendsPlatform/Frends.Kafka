cd ./Frends.Kafka.Produce.Tests

$file = Join-Path -Path ($pwd).path -ChildPath "docker-compose.yml"
if (-not(Test-Path -Path $file)) {
	curl -sSL https://raw.githubusercontent.com/bitnami/containers/main/bitnami/kafka/docker-compose.yml > docker-compose.yml
}

docker-compose up -d

cd ../