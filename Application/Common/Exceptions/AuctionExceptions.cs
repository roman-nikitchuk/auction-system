namespace Application.Common.Exceptions;

public class AuctionNotFoundException(int auctionId)
    : BaseException(auctionId, $"Auction not found under id {auctionId}");

public class AuctionNotActiveException(int auctionId)
    : BaseException(auctionId, $"Auction with id {auctionId} is not active");

public class UnhandledAuctionException(int auctionId, Exception? innerException = null)
    : BaseException(auctionId, "Unhandled auction exception", innerException);