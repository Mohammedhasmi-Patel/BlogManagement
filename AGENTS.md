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

code review requirement 


1. **Every controller action has `CancellationToken ct`** where the action is async.
2. **Always pass `ct` through the service and EF Core async operations.**
3. **Correct HTTP status codes**

   * Create → `201 Created`
   * Successful GET/update/action → `200 OK`
   * Successful delete with no body → `204 NoContent`
4. **Controller should remain thin.**

   * Extract claims/user info.
   * Call service.
   * Return response.
5. **User email is required only for protected/user-specific routes.**

   * Public endpoints like login don't need it.
6. **Every request payload uses a DTO**, even if it contains only one field.
7. **DTO naming:**

   * `FilenameRequestDTO`
   * `FilenameResponseDTO`
   * Example: `LoginUserRequestDTO`, `LoginUserResponseDTO`
8. **No N+1 queries.**
9. **Avoid unnecessary DB queries** — don't query the same data multiple times when one query can handle it.
10. **Read-only EF queries should use `AsNoTracking()`** where tracking isn't required.
11. **Don't return EF/domain entities directly from controllers**; map to `ResponseDTO`.
12. **Business logic belongs in services**, not controllers.
13. **Use your existing custom exceptions**, e.g.

```csharp
throw new UnauthorizedException("Invalid email or password.");
```

14. **Don't catch exceptions unnecessarily in services/controllers**; let your global exception middleware handle them.
15. **Don't flag things just because they're a different coding style.** Only report actual issues or violations of these project conventions.

