# Field mapping strategy

## Decision

Keep DTOs as plain serializable contracts. Do **not** put EF expressions or mapping behavior in DTO instances.

The reusable unit is a canonical server-side field definition:

```csharp
PatientFields.FirstName
// patient.firstName => Patient.FirstName
```

A field can be reused through a relationship:

```csharp
PatientFields.FirstName.Through<Endoscopy>(x => x.Encounter.Patient)
// same patient.firstName semantic field => Endoscopy.Encounter.Patient.FirstName
```

This preserves the original goal — define what a patient first name means exactly once — without coupling the wire contract to EF Core or leaking entity paths to clients.

`GIPractice.Application.Querying.QueryField<TEntity,TValue>` is the first implementation of this pattern. Its selector is a LINQ expression, so EF Core can translate it into SQL for filtering, ordering and projection.

## Validation

Incoming DTO validation belongs in `GIPractice.Application` and uses **FluentValidation**. A request validator should reject impossible ranges, invalid combinations and malformed identifiers before a query/store sees the request.

Entity/value-object invariants remain in `GIPractice.Core`; request-shape validation is not a domain entity's job.

## Mapping packages

There is no standard package that directly models this exact combination of canonical semantic fields plus reusable relationship paths. LINQ expression trees are the native abstraction used by EF Core and keep query translation explicit.

For ordinary command DTO-to-entity copying, a source generator such as Mapperly may be considered later. It should not own search/sort/query semantics. AutoMapper-style runtime mapping is deliberately not the foundation of the query system.

## Public sorting/filter fields

Clients send stable field identifiers such as `patient.firstName`, never CLR property paths such as `Encounter.Patient.FirstName`. The API resolves those identifiers against an allow-listed field catalog. This prevents arbitrary-property querying and decouples contracts from entity refactors.
