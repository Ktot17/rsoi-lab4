namespace GatewayBL.Models;

public record FullInfo(Reservation Reservation, Book? Book, Library? Library, int Rating = 0);
