#!/bin/bash
TLD="local"
OU="my-org-unit"
O="my-org"
L="my-city"
S="my-state"
C="AU"
PASSWORD="kafkatest" 

#openssl genrsa -out fake-ca-1.key -passout pass:$PASSWORD 2048
#openssl req -x509 -new -nodes -key fake-ca-1.key -sha256 -days 9999 -out fake-ca-1.pem -subj "/C=US/ST=New York/L=Brooklyn/O=Example Brooklyn Company/CN=examplebrooklyn.com"
#openssl req -new -key fake-ca-1.key -out fake-ca-1.csr
#openssl x509 -req -in fake-ca-1.csr -CA ./fake-ca-1.pem -CAkey ./fake-ca-1.key -CAcreateserial -out fake-ca-1.crt -days 9999 -sha256


# Repeat the following steps for each component in a loop
for i in broker control-center metrics schema-registry kafka-tools; do
	echo "Generating keys for component: ${i}"	# Create a Key Store
	keytool -genkey -noprompt \
		-alias ${i} \
		-dname "CN=${i}.${TLD}, OU=${OU}, O=${O}, L=${L}, S=${S}, C=${C}" \
		-keystore kafka.${i}.keystore.jks \
		-keyalg RSA \
		-storepass $PASSWORD \
		-keypass $PASSWORD	# Create a Certificate Signing Request
	keytool -keystore kafka.$i.keystore.jks -alias $i -certreq -file $i.csr -storepass $PASSWORD -keypass $PASSWORD  # Sign the Key
	openssl x509 -req -CA fake-ca-1.crt -CAkey fake-ca-1.key -in $i.csr -out $i-ca1-signed.crt -days 9999 -CAcreateserial -passin pass:$PASSWORD  # Import the CA cert into the Key Store
	keytool -keystore kafka.$i.keystore.jks -alias CARoot -import -file fake-ca-1.crt -storepass $PASSWORD -keypass $PASSWORD  # Import thesigned cert into the Key Store
	keytool -keystore kafka.$i.keystore.jks -alias $i -import -file $i-ca1-signed.crt -storepass $PASSWORD -keypass $PASSWORD	# Create truststore and import the CA cert.
	keytool -keystore kafka.$i.truststore.jks -alias CARoot -import -file fake-ca-1.crt -storepass $PASSWORD -keypass $PASSWORD  # Some Kafka components need the password to be available as plain text files...
	echo $PASSWORD >${i}_sslkey_creds
	echo $PASSWORD >${i}_keystore_creds
	echo $PASSWORD >${i}_truststore_creds
done