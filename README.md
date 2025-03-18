# StreamMS
Este es el ms a cargo de la gestion de los stream, 


## Requisitos Previos
- .NET 8 SDK
- PostgreSQL
- RabbitMQ Server
- URI Tickets Service y API Key

## Estructura del Proyecto

- **API/Controllers/**: Contiene los controladores de la API.
- **Domain/Entities/**: Contiene los modelos o entidades de datos.
- **Application/Services/**: Contiene los servicios de la aplicación.
- **Infrastructure/Data/**: Contiene el contexto de la base de datos.
- **Infrastructure/EventBus/**: Contiene la config del event bus/rabbitmq.
- **Program.cs**: Punto de entrada del proyecto.


# Instrucciones de Ejecución
Para ejecutar el proyecto UsersAuthorization, sigue estos pasos:

- Asegúrate de tener una base de datos PostgreSQL en funcionamiento.
- Configura las variables de entorno necesarias o modifica los archivos appsettings.json, según sea necesario.
- Navega al directorio del proyecto UsersAuthorization.
- Ejecuta el siguiente comando para aplicar las migraciones de la base de datos: `dotnet ef database update`
- Ejecuta el siguiente comando para iniciar el proyecto: `dotnet run`

Esto iniciará el proyecto y estará listo para poder ser usado.

Generar Documentación con Swagger
Swagger automáticamente genera la documentación de la API. Para ver la documentación generada, inicia la aplicación y navega a http://localhost:<puerto>/swagger.


## Endpoints Controller

### Streams

- [POST] `/api/v1/streams`: crea un nuevo stream para un partido valido, valida la plataforma(de acuerdo a las disponibles del sistema), ademas del rol del usuario que intenta cambiar el stream, y otras validaciones adicionales

***Headers***:
    - **Authorization**: Bearer some-token

***Body***:
```
{
  "idPlatform": 1,
  "streamUrl": "http://string.com",
  "idMatch": 3
}
```

***Respuesta***:
```
{
    "result": {
        "namePlatform": "YouTube",
        "url": "http://string.com/",
        "idMatch": 3,
        "idStream": 2
    },
    "message": "Successfully created"
}
```

- [PATCH] `/api/v1/streams/{streamId}/url`: cambia la url de un stream por una nueva que se proporcione, esto requiere el id de un Stream que viene como parametro dentro de la ruta de la peticion `streamId`

***Path Param***:
    - `streamId`: number

***URI***: `/api/v1/streams/2/url`

***Body***:
```
{
  "newUrl": "http://string2.com"
}
```

***Respuesta***:
```
{
    "result": null,
    "message": "Url successfully changed"
}
```


- [GET] `/api/v1/streams/{matchId}/viewers`: retorna los espectadores dentro de un partido

***Param***:
    - **matchId**:  number

***URI***: `/api/v1/streams/1/viewers`


***Respuesta***:
```
{
    "matchId": 1,
    "viewers": 0
}
```


- [POST] `/api/v1/streams/kick`: para sacar a una persona del hub del stream por medio de un socker en SignalR

***Headers***:
    - **Authorization**: Bearer some-token

***Body***:
```
{
  "idMatch": 2,
  "idUser2Kick": 0
}
```

- [POST] `/api/v1/streams/viewer/join`


- [POST] `/api/v1/streams/participant/join`


### Platforms

- [GET] `/api/v1/platforms`: se usa para obtener las plataformas disponibles para stream de la plataforma

***Headers***:
    - **Authorization**: Bearer some-token

***Respuesta***:
```
{
    "result": [
        {
            "idPlatform": 1,
            "namePlatform": "YouTube"
        },
        {
            "idPlatform": 2,
            "namePlatform": "Twitch"
        },
        {
            "idPlatform": 3,
            "namePlatform": "Zoom"
        },
        {
            "idPlatform": 4,
            "namePlatform": "Meet"
        }
    ],
    "message": "Succedded"
}
```

## RabbitMQ(EventBus)

En el proyecto se hace uso de RabbitMQ como Message Broker para procesamiento de eventos sincronos con el patron de integracion Request/Reply. 