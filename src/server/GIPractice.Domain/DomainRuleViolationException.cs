namespace GIPractice.Domain;

public sealed class DomainRuleViolationException(string message) : InvalidOperationException(message);
