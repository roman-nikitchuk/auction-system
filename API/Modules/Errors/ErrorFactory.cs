using Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class ErrorFactory
{
    public static ObjectResult ToObjectResult(this BaseException error)
    {
        return new ObjectResult(error.Message)
        {
            StatusCode = error switch
            {
                UserAlreadyExistsException => StatusCodes.Status409Conflict,
                UserNotFoundException => StatusCodes.Status404NotFound,
                UnhandledUserException => StatusCodes.Status500InternalServerError,

                AuctionNotFoundException => StatusCodes.Status404NotFound,
                AuctionNotActiveException => StatusCodes.Status400BadRequest,
                UnhandledAuctionException => StatusCodes.Status500InternalServerError,

                BidTooLowException => StatusCodes.Status400BadRequest,
                BidOnOwnAuctionException => StatusCodes.Status400BadRequest,
                UnhandledBidException => StatusCodes.Status500InternalServerError,

                CategoryNotFoundException => StatusCodes.Status404NotFound,
                CategoryAlreadyExistsException => StatusCodes.Status409Conflict,
                UnhandledCategoryException => StatusCodes.Status500InternalServerError,

                _ => StatusCodes.Status500InternalServerError
            }
        };
    }
}