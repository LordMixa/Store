# TeachersOfTomorrow

TeachersOfTomorrow is a modular application designed for creating a full-featured website for managing and accessing course-related information. The project is structured into four distinct layers, ensuring a clean architecture and separation of concerns. The primary programming language is C#.


## Project Structure

The solution consists of the following projects:

### 1. **TeachersOfTomorrow\.Api**

- **Purpose:** Entry point for the application, hosting the API and exposing endpoints for external use.
- **Key Features:**
  - Contains `CoursesController`, which provides the `GetCourseById` method to retrieve course details by ID.
    - Calls the `CoursesService` in the Business layer and passes the ID as a parameter.
  - **Swagger:**
    - Configured for API testing and documentation.
  - **MapperConfigurations:**
    - `ContractsMapperProfile` to configure AutoMapper mappings between `models` and `contracts`.
  - **Configuration:**
    - Includes a `ConnectionStrings` section in `appsettings.json` with a test database connection string.
  - **Program.cs:**
    - Configures Swagger, AutoMapper, and initializes the `Business` layer configuration.

### 2. **TeachersOfTomorrow\.Business**

- **Purpose:** Implements business logic and serves as a bridge between the API and data layers.
- **Key Features:**
  - **MapperConfigurations:**
    - `ModelsMapperProfile` to configure AutoMapper mappings between `entities` and `models`.
  - **Services:**
    - Contains `CoursesService`:
      - Method `GetCourseByIdAsync`: Calls the `CoursesRepository` in the Data layer to retrieve course data by ID.
  - **Models:**
    - Represents domain models used within the business layer.
  - **Configuration:**
    - Registers services, AutoMapper configurations, and initializes the `Data` layer configuration.

### 3. **TeachersOfTomorrow\.Data**

- **Purpose:** Handles data access and database interactions using Entity Framework Core.
- **Key Features:**
  - **Entities:**
    - Defines database entities representing the data structure.
  - **Repositories:**
    - Contains `CoursesRepository`:
      - Method `GetCourseByIdAsync`: Executes queries on the database using `DbSet` from EF Core.
  - **Context:**
    - Defines the database context and configures EF Core.
  - **Configuration:**
    - Registers repositories and configures the connection string for the database.

### 4. **TeachersOfTomorrow\.Contracts**

- **Purpose:** Defines contracts for communication between the API and its consumers.
- **Key Features:**
  - **Requests:**
    - Contains request models for API endpoints.
  - **Responses:**
    - Contains response models for data returned from API endpoints.

