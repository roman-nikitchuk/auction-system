namespace Application.Common.Exceptions;

public class BidTooLowException(int auctionId)
    : BaseException(auctionId, $"Bid amount is too low for auction {auctionId}");

public class BidOnOwnAuctionException(int auctionId)
    : BaseException(auctionId, $"Owner cannot place a bid on their own auction {auctionId}");

public class UnhandledBidException(int bidId, Exception? innerException = null)
    : BaseException(bidId, "Unhandled bid exception", innerException);