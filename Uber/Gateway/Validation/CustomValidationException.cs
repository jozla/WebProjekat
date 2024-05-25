using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Validation;

public class CustomValidationException
{
    private readonly RequestDelegate _next;

    public CustomValidationException(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BadPasswordException)
        {
            var problemDetails = new ValidationProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "ValidationFailure",
                Title = "Validation error",
                Detail = "One or more validation errors has occurred",
                Errors = new Dictionary<string, string[]>()
            };
            problemDetails.Errors.Add("Password", ["Password is not correct."]);
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
        catch (EntityNotFoundException)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            var problemDetails = new ValidationProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Type = "ValidationFailure",
                Title = "Validation error",
                Detail = "One or more validation errors has occurred",
                Errors = new Dictionary<string, string[]>()
            };
            problemDetails.Errors.Add("Entity", ["Entity is not found."]);
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
        catch (UserExistsException)
        {
            var problemDetails = new ValidationProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "ValidationFailure",
                Title = "Validation error",
                Detail = "One or more validation errors has occurred",
                Errors = new Dictionary<string, string[]>()
            };
            problemDetails.Errors.Add("User", ["User already exists."]);
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
        catch (RideConfirmedException)
        {
            var problemDetails = new ValidationProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "ValidationFailure",
                Title = "Validation error",
                Detail = "One or more validation errors has occurred",
                Errors = new Dictionary<string, string[]>()
            };
            problemDetails.Errors.Add("Ride", ["Ride already confirmed."]);
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
        catch (RideNotConfirmedException)
        {
            var problemDetails = new ValidationProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "ValidationFailure",
                Title = "Validation error",
                Detail = "One or more validation errors has occurred",
                Errors = new Dictionary<string, string[]>()
            };
            problemDetails.Errors.Add("Ride", ["Ride is not confirmed."]);
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
        catch (ValidationException exception)
        {
            var problemDetails = new ValidationProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "ValidationFailure",
                Title = "Validation error",
                Detail = "One or more validation errors has occurred",
            };

            if (exception.Errors is not null)
            {
                CopyErrorsFromValidationException(problemDetails, (IEnumerable<ValidationFailure>)exception.Errors);
            }

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }

    private static void CopyErrorsFromValidationException(ValidationProblemDetails problemDetails, IEnumerable<ValidationFailure> validationExceptionErrors)
    {
        foreach (var validationExceptionError in validationExceptionErrors)
        {
            var key = validationExceptionError.PropertyName;
            key = char.ToLower(key[0]) + key.Substring(1);
            if (!problemDetails.Errors.TryGetValue(key, out var messages))
            {
                messages = Array.Empty<string>();
            }

            messages = messages.Concat(new[] { validationExceptionError.ErrorMessage }).ToArray();
            problemDetails.Errors[key] = messages;
        }
    }

}
