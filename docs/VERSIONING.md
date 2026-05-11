# Versioning

This backend uses Semantic Versioning: `MAJOR.MINOR.PATCH`.

- `MAJOR`: incompatible API, database, or deployment changes.
- `MINOR`: backward-compatible features or endpoints.
- `PATCH`: bug fixes and small safe changes.

## Version Sources

- `Gestionale.Api.csproj`
- `VERSION`
- `CHANGELOG.md`

All three must be updated together for a release.

## Tag Format

Use backend tags with the `be/` prefix:

```bash
git tag be/v0.1.0
git push origin be/v0.1.0
```

## Release Checklist

1. Confirm the working tree contains only intended changes.
2. Run `dotnet build /p:UseAppHost=false`.
3. Update `VERSION`, `Gestionale.Api.csproj`, and `CHANGELOG.md`.
4. Commit with a clear message, for example `chore(release): backend 0.1.0`.
5. Create and push the tag.
