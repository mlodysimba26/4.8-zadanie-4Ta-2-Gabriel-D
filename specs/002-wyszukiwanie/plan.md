# Plan implementacji: Wyszukiwanie notatek (moduł 002)

Dokument opisuje zmiany techniczne realizujące wymagania ze specyfikacji `specs/002-wyszukiwanie/spec.md` w istniejącej aplikacji konsolowej.

---

## 1. Zakres zmian w plikach

1. **`notatnik/Program.cs`**
   - Dodanie polecenia `[s] szukaj` w menu głównym `Program.Main`.
   - Dodanie metody `SearchNotes(string query)` w klasie `NotesStorage` (lub pomocniczo nad wczytaną listą z `LoadNotes()`):
     - Dla pustej frazy (`string.IsNullOrWhiteSpace(query)`) zwraca pełną listę notatek w odwróconej chronologii.
     - Dla niepustej frazy filtruje notatki po `Title`, `Content` oraz `DisplayHeader` z użyciem `StringComparison.OrdinalIgnoreCase`.
   - Dodanie metody `SearchNotesCommand(NotesStorage storage)` w klasie `Program`:
     - Pobiera frazę od użytkownika.
     - W przypadku pustej listy wyników wypisuje komunikat o braku dopasowań.
     - W przeciwnym razie wypisuje listę w formacie `[i] YYYY-MM-DD HH:mm — Tytuł`.

---

## 2. Kolejność realizacji według kryteriów

1. **Zadanie 7 (002 — Kryterium 1):** Polecenie `s` oraz filtrowanie notatek po frazie w tytule i treści.
2. **Zadanie 8 (002 — Kryterium 2):** Dopasowanie niezależne od wielkości liter (`OrdinalIgnoreCase`).
3. **Zadanie 9 (002 — Kryterium 3):** Czytelny komunikat przy braku wyników.
4. **Zadanie 10 (002 — Kryterium 4):** Pusta fraza zwraca pełną listę notatek.
