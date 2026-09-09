# Releases — Pixault.Client

How a release actually happens in this repo, as of 2026-09-09:

> `.github/workflows/publish-nuget.yml` fires on **push to `main` touching `src/**`**. It reads
> `<Version>` from `src/Pixault.Client/Pixault.Client.csproj`, asks nuget.org whether that version
> already exists, and publishes via Trusted Publishing only if it does not. **Tags are not
> involved**, and this file is not read by CI — it is for humans, and for
> `pixault/scripts/release.sh`, which requires a matching `## <version>` section before it will
> bump anything.
>
> So: to release, bump `<Version>` and push to `main`. Pushing `src/` changes *without* bumping is
> safe — the version-exists check skips the publish.

---

## 1.6.1

**Fixed**

- A null `publicId` no longer throws out of `PixaultUrlBuilder.Build()`. `IsLegacyId` called
  `StartsWith` on its argument with no null check, so any caller passing a null id — a record with
  no image, which is an ordinary state rather than a bug — got a `NullReferenceException` from
  deep inside URL construction. On 2026-08-09 one such row took all 309 barber.shop profile pages
  down.

  Null and empty are now treated as "not a legacy id", which is what the server's
  `DeliveryGrammar.IsLegacyId` has done since 2026-08-10. This method's doc comment always
  promised it matched that function exactly; now it does.

  Both branches that consult it are covered: `Build()` directly, and `VariantUrl()` by way of
  `ToImgTag` / `ToPictureTag`.

**Compatibility** — no API surface change. `IsLegacyId` is private; its parameter widened from
`string` to `string?`. Every URL this version builds for a non-null id is byte-identical to 1.6.0.

**Known, not fixed here** — `baseUrl.TrimEnd('/')` in the constructor and `fmt.TrimStart('.')` in
`Format()` have the same shape. Both are reached from configuration or an explicit literal rather
than from data, and both throw at the call site rather than mid-render, so failing fast is
arguably correct for them. Deliberate decision, not an oversight.

---

## 1.6.0

Dual-mode `For()`: legacy grammar for id-shaped ids (`img_` / `vid_` / `eps_` prefixes),
publicId grammar for slugs.

## 1.5.0

publicId URL grammar in Cloudinary order, with `f_auto` as the default format.

## 1.4.0

Extracted `IPixaultAdminClient` and `IPixaultUploadClient` interfaces.

---

Entries before 1.4.0 predate this file. They are recoverable from the commit history and from the
published packages on nuget.org.
