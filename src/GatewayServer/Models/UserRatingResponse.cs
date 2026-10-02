using System.ComponentModel.DataAnnotations;

namespace GatewayServer.Models;

public record UserRatingResponse([Range(0, 100)]int Stars);