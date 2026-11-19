# Notatnik-v2

Podręczny notatnik osobisty — wariant konsolowy (.NET 9 / C#).

## Wymagania

- .NET 9 SDK

## Uruchomienie

```
dotnet run --project notatnik
```

Notatki zapisywane są do pliku `notatki.md` w bieżącym katalogu roboczym.

## Struktura repozytorium

```
specs/001-notatnik/
  spec.md       — specyfikacja wymagań i kryteria akceptacji
  plan.md       — plan implementacji
konstytucja.md  — nienaruszalne inwarianty projektu
decyzje/        — rekordy decyzji architektonicznych
notatnik/       — kod źródłowy (projekt .NET)
dziennik.md     — dziennik weryfikacji kryteriów
```
