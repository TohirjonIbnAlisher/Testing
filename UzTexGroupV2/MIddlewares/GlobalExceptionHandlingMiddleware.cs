﻿using System.Net;
using System.Text.Json;
using UzTexGroupV2.Domain;

namespace UzTexGroupV2.MIddlewares;

public class GlobalExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandlingMiddleware> logger;

    public GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        this.logger = logger;
    }
    public async Task InvokeAsync(
        HttpContext context,
        RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (InvalidIdException invalidIdException)
        {
            logger.LogError(invalidIdException, invalidIdException.Message);

            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var serializedObject = JsonSerializer.Serialize(new
            {
            invalidIdException.Message
            });

            await HandleExceptionAsync(context, serializedObject);
        }
        catch(NotFoundException notFoundException)
        {
            logger.LogError(notFoundException, notFoundException.Message);

            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var serializedObject = JsonSerializer.Serialize(new
            {
                notFoundException.Message
            });

            await HandleExceptionAsync(context, serializedObject);
        }

        catch(Exception exception)
        {
            logger.LogError(exception, exception.Message);

            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var serializedObject = JsonSerializer.Serialize(new
            {
                exception.Message
            });

            await HandleExceptionAsync(context, serializedObject);
        }
    }
    private async Task HandleExceptionAsync(
        HttpContext context,
        string message)
    {
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(message);
    }
}
