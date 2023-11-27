namespace KeyLookup.Models;

public record KeyLookupNode(string IpAddress, DateTime StartedAtUtc, DateTime LastHeartBeatUtc);
