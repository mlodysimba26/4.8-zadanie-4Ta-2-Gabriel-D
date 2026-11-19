# Plan implementacji: Podręczny Notatnik Osobisty (Notatnik-v2)

Dokument zawiera architekturę techniczną, mapę komponentów oraz strukturę plików źródłowych aplikacji, które realizują wymagania zawarte w specyfikacji `specs/001-notatnik/spec.md`.

---

## 1. Wybór platformy technologicznej

- **Framework:** Avalonia UI (.NET 9)
- **Wzorzec architektoniczny:** MVVM / Code-Behind z obsługą zdarzeń Avalonia
- **Magazyn danych:** Plik tekstowy `notatki.md` w kodowaniu UTF-8

---

## 2. Struktura plików projektu (przeniesiona ze specyfikacji)

Poniższe pliki źródłowe odpowiadają za realizację logiki i interfejsu (zgodnie z zasadą izolacji specyfikacji od nazw plików z kodem):

1. **`notatnik.csproj`**  
   Plik projektu .NET 9 z pakietami Avalonia (`Avalonia`, `Avalonia.Desktop`, `Avalonia.Themes.Fluent`, `Avalonia.Fonts.Inter`).

2. **`Program.cs`**  
   Punkt wejścia aplikacji inicjalizujący środowisko Avalonia (`BuildAvaloniaApp()`).

3. **`App.axaml` oraz `App.axaml.cs`**  
   Definicja stylów aplikacji (motyw `FluentTheme`) oraz cyklu życia programu.

4. **`MainWindow.axaml`**  
   Deklaracja interfejsu graficznego XAML:
   - Kontrolki `TextBox` dla tytułu (`TitleBox`) i treści (`ContentBox`).
   - Kontrolka `Button` do zapisu (`SaveButton`).
   - Kontrolka `TextBlock` dla komunikatów statusu (`StatusText`).
   - Kontrolka `ListBox` dla listy zapisanych notatek (`NotesList`).
   - Kontrolka `TextBlock` dla podglądu wybranej notatki (`PreviewText`).

5. **`MainWindow.axaml.cs`**  
   Logika widoku (Code-Behind) obsługująca:
   - Zdarzenia `TextChanged` w polach edycyjnych i walidację stanu przycisku `SaveButton.IsEnabled`.
   - Zdarzenie kliknięcia przycisku zapisu `OnSaveClicked`.
   - Zdarzenie `SelectionChanged` kontrolki `NotesList` aktualizujące podgląd.
   - Odczyt i inicjalizację listy podczas uruchomienia programu.

6. **`NoteModel.cs`**  
   Klasa modelu reprezentująca pojedynczą notatkę w pamięci:
   - Właściwości: `Title` (string), `Timestamp` (DateTime), `Content` (string), `DisplayHeader` (string).

7. **`NotesStorage.cs`**  
   Klasa serwisowa odpowiadająca za operacje wejścia/wyjścia na pliku magazynu `notatki.md`:
   - Metoda `LoadNotes()` parsująca nagłówki `## YYYY-MM-DD HH:mm — Tytuł` i wydzielająca treść.
   - Metoda `SaveNote(NoteModel note)` realizująca odwróconą chronologię (wstawianie na początek) z kodowaniem `Encoding.UTF8` oraz obsługą wyjątków `IOException` / `UnauthorizedAccessException`.
