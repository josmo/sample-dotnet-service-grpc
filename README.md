* [![Build Status](https://drone.seattleslow.com/api/badges/josmo/sample-dotnet-service-grpc/status.svg)](https://drone.seattleslow.com/josmo/sample-dotnet-service-grpc)

#C# Template for services - 

##Tech to know about

* GRPC
* .NET Core 2
* Entity Framework - might switch for speed
* Docker

##Tools needed

* Docker
* Drone CLI


##To get started

```sh
drone exec
docker build -t service .
docker run -e SQL_HOST=sqlhost -e SQL_USER=sa -e SQL_PASSWORD=mypassword service
```


