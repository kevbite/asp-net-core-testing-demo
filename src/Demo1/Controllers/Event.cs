using System;

namespace Demo1.Controllers
{
    public record Event(Guid Id, string Name, DateTimeOffset Start);
}