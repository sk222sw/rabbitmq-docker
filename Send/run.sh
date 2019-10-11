docker build -t send .

docker container stop send
docker container rm send
docker run -d -p 8080:80 --name send send