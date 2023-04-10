using SpoRE.Infrastructure.Database;
using SpoRE.Services;

namespace SpoRE.Models.Response;

public record SelectableRider(RiderParticipation Details, SelectableEnum Selectable);
