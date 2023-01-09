KafkaVersionCompare
=================
Taking inspiration from Umbraco compare [web site](https://our.umbraco.com/download/releases/compare).

This asp.net core application builds a version compare page for Kafka. It uses the Kafka [releases page](https://archive.apache.org/dist/kafka/) and for each version
retrieves the release page and parses it to build the versions.  The dropdowns allow you to pick 2 versions and a delta is created and displayed to show the differences.

### Run using Docker ###
cd KafkaVersionCompare

docker-compose up -d
