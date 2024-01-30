# DocumGen
[YouTube demo](https://youtu.be/mfYGZKJMIVo)

## Task description

Implement a web service. The service should accept an HTML file from a web client, convert it to PDF using Puppeteer Sharp (https://www.puppeteersharp.com/), and return it somehow to the client.
Create a simple web client that will communicate with the web service. The user should send files for conversion by uploading them from computer. The client should display the list of files sent to conversion (keep it in memory). Each list item should contain filename, option to download result, option to remove item.

Things to consider
- The incoming HTML files may be big. It means conversion may take a long time. Like a few minutes.
- The web service should be stable. It means if client sent a file for conversion and then, after receiving request from a client IIS was restarted, the web service should still be able to return the result PDF to a client after restarting.
- The web service should be scalable. It means we should be able to extend our infrastructure without the need to change service code. Only by the hands of system administrators. 

## Implementation

### Services interaction
![Services](https://github.com/vashov/docum-gen/blob/main/docs/services.png)

### Dependencies
- .NET 6
- docker (to run RabbitMQ)
- npm, Vue

### How to test on local machine

1. Configure LocalStorage path:
```
- for ConverterWorker project .\DocumGen.ConverterWorker\appsettings.json -> LocalStorage -> Path
- for API project .\DocumGen.API\appsettings.json -> LocalStorage -> Path
```
2. Run RabbitMQ:
```
docker pull rabbitmq:3-management
docker run --rm -it -p 15672:15672 -p 5672:5672 rabbitmq:3-management
```
3. Run ConverterWorker project:
```
cd .\DocumGen.ConverterWorker\
dotnet run
```
4. Run API project:
```
cd .\DocumGen.Api\
dotnet run
```
5. Run Vue client:
```
cd .\documgen.ui.vue\
npm run serve
```
6. Open localhost:5002 in browser.

### Out of solution scope
- Authentication/Authorization
- Mapping of Domain entities to DTOs on server side.
- Display errors on client side.
