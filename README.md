Mango is an e-commerce application developed using ASP.NET Core MVC and Web API, designed to manage online shopping operations, including user authentication, product management, shopping cart, order processing, and payment integration.

The application follows a service-oriented architecture, where the MVC frontend communicates with independent backend Web APIs using `HttpClient`. JWT-based authentication is used for securing API communication, while cookie authentication manages user sessions in the MVC application.

The application integrates **Stripe** for online payment processing and **Azure Service Bus Topics** for asynchronous communication between services. After successful payment, order details are updated and a rewards event is published for independent processing by the Rewards service.

**Key Technologies:**
- ASP.NET Core MVC and Web API
- C#, Entity Framework Core, SQL Server
- REST APIs and HttpClient
- JWT and Cookie Authentication
- AutoMapper and Dependency Injection
- Stripe Payment Gateway
- Azure Service Bus (Topics and Subscribers)
- Asynchronous Programming (async/await)
