# Plan implementacji: Podręczny Notatnik Osobisty (Notatnik-v2)

Dokument zawiera architekturę techniczną, mapę komponentów oraz strukturę plików źródłowych aplikacji realizujących wymagania ze specyfikacji `specs/001-notatnik/spec.md`.

---

## 1. Wybór platformy technologicznej

- **Platforma:** .NET 9 (aplikacja konsolowa w języku C#)
- **Interfejs:** Pętla poleceń w terminalu (`[n]`, `[l]`, `[q]`) z obsługą UTF-8
- **Magazyn danych:** Plik tekstowy `notatki.md` w kodowaniu UTF-8 w bieżącym katalogu roboczym

---

## 2. Struktura plików projektu

1. **`notatnik/notatnik.csproj`**  
   Plik projektu .NET 9 (`Exe`, `TargetFramework: net9.0`, bez zewnętrznych zależności NuGet).

2. **`notatnik/Program.cs`**  
   Zawiera trzy logiczne komponenty aplikacji:
   - **`NoteModel`** — model pojedynczej notatki (`Timestamp`, `Title`, `Content`, `DisplayHeader`).
   - **`NotesStorage`** — operacje wejścia/wyjścia na pliku `notatki.md` w kodowaniu UTF-8:
     - `LoadNotes()` parsująca nagłówki `## YYYY-MM-DD HH:mm — Tytuł` (z tolerancją separatora `—` i `-`) oraz treść.
     - `SaveNote(NoteModel note)` dopisująca nowy wpis na początek pliku (odwrócona chronologia).
   - **`Program`** — pętla interakcji z użytkownikiem:
     - Automatyczne wczytanie i wyświetlenie listy notatek przy uruchomieniu programu.
     - Walidacja niepustego tytułu przed zapisem (odrzucanie pustych wierszy i samych białych znaków).
     - Obsługa wyjątków `IOException` i `UnauthorizedAccessException` z zachowaniem wpisanych danych i możliwością ponowienia zapisu.
