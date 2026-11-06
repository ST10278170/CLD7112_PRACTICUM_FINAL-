CLD7112 Practicum Final - AgriStreamIoT

Project Overview

AgriStreamIoT is an agricultural Internet of Things application developed to demonstrate the use of web development, relational database storage, cloud storage concepts, serverless functions, asynchronous processing, monitoring, load testing, and scalability.

The solution contains two main projects:

- **AgriStreamIoT** - ASP.NET Core MVC web application.
- **AgriStreamFunctions** - .NET isolated Azure Functions project.

The application was developed and tested using local development services, including SQL Server LocalDB and Azurite for Azure Storage emulation.

Main Features

IoT Device Management

The application provides a device registry for agricultural IoT equipment.

Users can:

- Register IoT devices.
- View registered devices.
- Edit device information.
- Delete devices.
- Monitor operational status.

The irrigation controller used during testing is associated with Field A - North.

User Management

The system contains user-management functionality with support for different application roles.

The application demonstrates:

- User creation.
- User details.
- User editing.
- User deletion.
- Role assignment.

Farmer and Agronomist roles were used during testing.

Agricultural Telemetry

The telemetry dashboard stores and displays agricultural sensor readings.

Telemetry information includes:

- Soil moisture.
- Temperature.
- Humidity.
- Irrigation status.
- Device information.
- Field location.
- Recorded date and time.

Sensor readings can also be simulated through the web application.

Irrigation Command Processing

The irrigation module allows commands to be submitted for registered irrigation controllers.

Commands are placed in Queue Storage for asynchronous processing.

Supported commands include:

- Start Irrigation.
- Stop Irrigation.

The queue-triggered function processes the command and updates the operational status of the corresponding device in the SQL database.

Drone Image Storage

Agricultural drone images can be uploaded through the web application.

Uploaded images are stored using Blob Storage through the local Azurite storage emulator.

Azure Functions

The AgriStreamFunctions project contains four Azure Functions.

1. LatestTelemetryFunction

HTTP GET triggered function used to retrieve the latest agricultural telemetry information.

2. TriggerIrrigationFunction

HTTP POST triggered function used to accept an irrigation command and place the command into Queue Storage.

3. ProcessIrrigationQueueFunction

Queue triggered function used to process irrigation commands asynchronously and update the corresponding IoT device status in SQL Server.

4. DroneImageBlobFunction

Blob triggered function used to detect newly uploaded agricultural drone images and record image-processing information.

Local Azure Storage

Azurite was used to provide local Azure Storage services during development and testing.

The solution makes use of:

- Blob Storage for agricultural drone images.
- Queue Storage for irrigation commands.
- Table Storage for agricultural telemetry and processing information.

This approach allowed the Azure Storage workflow to be developed and tested locally.

Database

The application uses SQL Server LocalDB with Entity Framework Core.

The relational database stores information such as:

- Application users.
- Registered IoT devices.
- Device operational status.

Entity Framework Core migrations are included with the web application.

End-to-End Irrigation Workflow

The irrigation workflow was tested from request submission through database update.

The process follows this sequence:

1. An irrigation command is submitted.
2. The command is placed into Queue Storage.
3. ProcessIrrigationQueueFunction detects the queue message.
4. The function processes the irrigation command.
5. The SQL device registry is updated.
6. The updated operational status is displayed by the web application.

During testing, Device 1 was successfully updated to an Irrigating operational state.

Load Testing

A local automated load test was developed for the AgriStreamIoT telemetry endpoint.

The final test configuration used:

- 10 concurrent users.
- 20 requests per user.
- 200 total HTTP requests.

Final test results:

- Successful requests: 200.
- Failed requests: 0.
- Success rate: 100%.
- Average response time: 70.24 ms.
- Minimum response time: 29.49 ms.
- Maximum response time: 147.48 ms.
- Total test duration: 16.19 seconds.
- Throughput: 12.35 requests per second.

Detailed load-test results are included in the repository.

Performance Monitoring

Application CPU and process activity were monitored during the concurrent load test.

Windows Task Manager was used to observe the AgriStreamIoT process while the application handled simulated concurrent traffic.

The application remained operational throughout the successful 200-request test.

Scaling Simulation

A local scaling-policy simulation was created using measurements obtained during the load test.

The simulation evaluated:

- Request throughput.
- Average response time.
- Maximum response time.
- Current application instances.
- Minimum and maximum instance limits.

Based on the observed workload, the simulation produced a SCALE OUT recommendation from one application instance to two instances.

This represents a local simulation of an Azure-style autoscaling policy. Actual Azure App Service autoscaling was not performed because an active Azure subscription was not available.

Testing and Verification

The completed solution was tested for:

- Device management.
- User management.
- Telemetry creation and retrieval.
- Irrigation command submission.
- Queue processing.
- SQL device status updates.
- Blob image uploads.
- Azure Functions execution.
- Concurrent HTTP load.
- Resource monitoring.
- Local scaling-policy evaluation.

The final solution was rebuilt in Release configuration with both projects compiling successfully.

## Technologies Used

- C#
- ASP.NET Core MVC
- .NET 8
- Entity Framework Core
- SQL Server LocalDB
- Azure Functions
- Azure Blob Storage concepts
- Azure Queue Storage concepts
- Azure Table Storage concepts
- Azurite
- PowerShell
- HTML
- CSS
- Bootstrap
- Git
- GitHub

## Repository Structure
