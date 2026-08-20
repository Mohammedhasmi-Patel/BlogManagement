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

# DTO Creation Guidelines

## 1. DTO Folder Structure

When creating DTOs, always organize them into a folder based on their feature/module.

Use **PascalCase** for folder names.

Example:

* `Auth`
* `Product`
* `Blog`
* `User`
* `Category`

Do not place all DTOs directly inside one common `DTO` folder.

### Example

For authentication DTOs:

`DTOs/Auth/`

For product DTOs:

`DTOs/Product/`

---

## 2. Folder Naming Convention

Folder names must follow **PascalCase**.

Correct:

* `Auth`
* `Product`
* `BlogPost`
* `UserProfile`

Incorrect:

* `auth`
* `product`
* `blogpost`
* `user_profile`

---

## 3. Request DTO Naming Convention

For request DTOs, always use the suffix:

`RequestDTO`

Examples:

* `LoginRequestDTO`
* `RegisterRequestDTO`
* `CreateProductRequestDTO`
* `UpdateProductRequestDTO`
* `CreateBlogRequestDTO`

Example structure:

`DTOs/Auth/LoginRequestDTO.cs`

`DTOs/Auth/RegisterRequestDTO.cs`

---

## 4. Response DTO Naming Convention

For response DTOs, always use the suffix:

`ResponseDTO`

Examples:

* `LoginResponseDTO`
* `RegisterResponseDTO`
* `ProductResponseDTO`
* `ProductListResponseDTO`
* `BlogResponseDTO`

Example structure:

`DTOs/Auth/LoginResponseDTO.cs`

`DTOs/Auth/RegisterResponseDTO.cs`

---

## 5. Complete Example

For an Auth feature:

`DTOs/Auth/`

* `LoginRequestDTO.cs`
* `LoginResponseDTO.cs`
* `RegisterRequestDTO.cs`
* `RegisterResponseDTO.cs`

For a Product feature:

`DTOs/Product/`

* `CreateProductRequestDTO.cs`
* `UpdateProductRequestDTO.cs`
* `ProductResponseDTO.cs`
* `ProductListResponseDTO.cs`

---

## 6. Strict Rules

* Use **PascalCase** for DTO folder names.
* Group DTOs by feature/module.
* Request DTOs must end with `RequestDTO`.
* Response DTOs must end with `ResponseDTO`.
* Do not use ambiguous names such as `LoginDTO`, `ProductDTO`, or `UserDTO`.
* Keep request and response DTOs separate.
* Follow the existing project's namespace structure.

# Request Validation Guidelines

## 1. Validation Library

For all API request validation, use:

* `FluentValidation`
* `FluentValidation.DependencyInjectionExtensions`

Do not implement request validation manually when FluentValidation can handle it.

---

## 2. Validator Location

Create validators according to the feature/module they belong to.

Use the same feature-based folder structure as DTOs.

Example:

`DTOs/Auth/`

* `LoginRequestDTO.cs`
* `RegisterRequestDTO.cs`

`Validators/Auth/`

* `LoginRequestValidator.cs`
* `RegisterRequestValidator.cs`

For Product:

`Validators/Product/`

* `CreateProductRequestValidator.cs`
* `UpdateProductRequestValidator.cs`

---

## 3. Validator Naming Convention

Validator classes must follow:

`[RequestDTOName]Validator`

Examples:

* `LoginRequestDTO` → `LoginRequestValidator`
* `RegisterRequestDTO` → `RegisterRequestValidator`
* `CreateProductRequestDTO` → `CreateProductRequestValidator`
* `UpdateProductRequestDTO` → `UpdateProductRequestValidator`

Validators should inherit from:

`AbstractValidator<T>`

where `T` is the corresponding request DTO.

---

## 4. Dependency Injection

Register FluentValidation validators through dependency injection using:

`FluentValidation.DependencyInjectionExtensions`

Validators should be automatically discovered and registered.

Do not manually register every validator individually unless there is a specific project requirement.

---

## 5. Validation Rules

Keep validation inside the corresponding FluentValidation validator.

Examples of validations that should be handled through FluentValidation:

* Required fields
* String length
* Minimum/maximum values
* Email format
* Password requirements
* Numeric ranges
* Date validation
* Conditional validation
* Collection validation

Do not put validation logic inside controllers unless it is specifically related to HTTP/request handling rather than DTO validation.

---

## 6. Important Rules

* Every request DTO that requires validation should have its own validator.
* Do not put validation rules directly inside DTO classes.
* Do not create custom validation frameworks when FluentValidation already provides the required functionality.
* Keep validators focused on their corresponding request DTO.
* Use meaningful validation messages.
* Follow the existing project namespace and folder conventions.
* Do not add unnecessary validation rules that are unrelated to the request's business requirements.

### Example Structure

`DTOs/Auth/LoginRequestDTO.cs`

`Validators/Auth/LoginRequestValidator.cs`

`DTOs/Product/CreateProductRequestDTO.cs`

`Validators/Product/CreateProductRequestValidator.cs`

The overall structure should remain feature-based and consistent throughout the project.
