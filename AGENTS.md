# Agent.md

## Task: Exception Handling & Generic API Response

Inside the **current project**, implement the following.

### 1. Create `Extensions` folder

Create an `Extensions` folder inside the current project.

### 2. Create `AppException`

Inside `Extensions`, create the main `AppException` class.

Requirements:

* `AppException` must inherit from `Exception`.
* It must accept **only a message**.
* It must not contain any additional properties, objects, error codes, metadata, etc.

Example usage:

`new AppException("Something went wrong.")`

### 3. Create Custom Exceptions

Create all common application/HTTP exceptions, including:

* `BadRequestException`
* `UnauthorizedException`
* `ForbiddenException`
* `NotFoundException`
* `ConflictException`
* `UnprocessableEntityException`
* `InternalServerException`

All of these classes must inherit from `AppException`.

Every exception must contain **only a message constructor**.

The intended usage must be:

`new BadRequestException("Invalid request.")`

`new NotFoundException("Blog not found.")`

`new UnauthorizedException("Unauthorized.")`

Do NOT add:

* Error codes
* Status code properties
* Error objects
* Error lists
* Validation error objects
* Entity IDs
* Metadata
* Additional constructor parameters
* Additional custom properties

Keep the exception classes extremely simple.

### 4. Create `ApiResponse<T>`

Inside the `Extensions` folder, create a generic `ApiResponse<T>` class.

The class must contain exactly these four properties:

* `int StatusCode`
* `string Message`
* `T Data`
* `bool Success`

Do not add any other properties.

### 5. `SuccessResponse`

Add a method named exactly:

`SuccessResponse`

It must accept:

* `T? data`
* `int statusCode`
* `string message`

Conceptually:

`SuccessResponse(T? data, int statusCode, string message)`

It should return an `ApiResponse<T>` with:

* `StatusCode = statusCode`
* `Message = message`
* `Data = data`
* `Success = true`

### 6. `ErrorResponse`

Add a method named exactly:

`ErrorResponse`

It must accept only:

* `int statusCode`
* `string message`

Conceptually:

`ErrorResponse(int statusCode, string message)`

**Do not accept `T` or any data parameter in `ErrorResponse`.**

For an error response:

* `StatusCode = statusCode`
* `Message = message`
* `Data = default`
* `Success = false`

### 7. Important Restrictions

Do not create separate classes such as:

* `ErrorResponse`
* `ErrorData`
* `ErrorDetails`
* `ValidationError`
* `SuccessResponse`

There should only be one response class:

`ApiResponse<T>`

Do not introduce additional abstractions or unnecessary complexity.

Follow the existing project's namespace and coding conventions.

Do not modify unrelated existing code.

### Expected Structure

`CurrentProject/Extensions/`

* `AppException.cs`
* `BadRequestException.cs`
* `UnauthorizedException.cs`
* `ForbiddenException.cs`
* `NotFoundException.cs`
* `ConflictException.cs`
* `UnprocessableEntityException.cs`
* `InternalServerException.cs`
* `ApiResponse.cs`

For Creating DTO use proper Folder Like if u r creating the DTO for auth then folder 
name follows PascalCase

like Auth , Product 

DTO class Naming Convention : for request use RequestDTO as suffix & for response use ResponseDTO