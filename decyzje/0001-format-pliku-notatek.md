# 0001. Format pliku notatek i odwrócona chronologia

- **Status:** Przyjęta
- **Data podjęcia:** 2026-10-29 (w ramach prac nad Budową 2)
- **Autor:** Gabriel Duczyński

---

## 1. Kontekst

Aplikacja notatnika potrzebuje prostego i trwałego sposobu zapisywania notatek wprowadzanych przez użytkownika w oknie programu. Każdy wpis składa się z tytułu, treści (często wieloliniowej) oraz znacznika czasu utworzenia. 

Kluczowym założeniem projektu od samego początku była pełna niezależność danych od aplikacji — użytkownik powinien móc podejrzeć lub ręcznie zedytować swoje zapiski w dowolnym standardowym edytorze tekstu (Notatnik, VS Code, Obsidian) bez konieczności uruchamiania programu. Równocześnie z punktu widzenia wygody użytkownika najbardziej pożądane jest natychmiastowe widzenie najświeższych wpisów zaraz po otwarciu pliku lub widoku listy, bez konieczności każdorazowego przewijania setek linii na sam koniec dokumentu.

---

## 2. Decyzja

Wybieramy zapis wszystkich wpisów do jednego wspólnego pliku tekstowego `notatki.md` w formacie Markdown z kodowaniem UTF-8. 

Struktura każdego wpisu w pliku jest ściśle zdefiniowana:
1. Każda notatka rozpoczyna się od nagłówka drugiego stopnia: `## YYYY-MM-DD HH:mm — Tytuł`
2. W kolejnych wierszach znajduje się treść notatki (jedno- lub wieloliniowa).
3. Poszczególne notatki są oddzielone od siebie dokładnie jedną pustą linią.
4. Nowo dodawana notatka jest umieszczana **na samym początku pliku** (odwrócona chronologia). W kodzie aplikacji realizujemy to poprzez odczytanie całej dotychczasowej zawartości pliku (`File.ReadAllText`), sformatowanie nowego bloku tekstu i ponowne nadpisanie pliku całością (`File.WriteAllText`).

---

## 3. Odrzucone alternatywy

- **Format JSON (`notes.json` jako tablica obiektów):**
  Odrzucono, ponieważ plik JSON jest mało czytelny przy bezpośrednim otwarciu w zwykłym edytorze (widoczne znaki ucieczki `\n`, nawiasy klamrowe i cudzysłowy). Ponadto jakakolwiek przypadkowa ręczna edycja przez użytkownika (np. zgubiony przecinek) powoduje unieważnienie całej struktury dokumentu i błąd parsowania przy starcie aplikacji.

- **Zwykły plik tekstowy z separatorem (`notes.txt` z separatorem `---` na końcu):**
  Odrzucono, ponieważ brak ustandaryzowanego nagłówka utrudnia jednoznaczne rozróżnienie tytułu od treści przy notatkach wielowierszowych, a dopisywanie na końcu pliku wymusza przewijanie dokumentu w edytorze lub odwracanie tablicy w pamięci programu przy każdym uruchomieniu.

- **Osobny plik `.md` dla każdej notatki w dedykowanym katalogu:**
  Odrzucono na tym etapie jako zbyt skomplikowane architektonicznie. Wymagałoby to generowania unikalnych nazw plików, bezpiecznego czyszczenia tytułów z niedozwolonych znaków systemu plików (`/`, `\`, `:`, `*`, `?`) oraz zarządzania katalogami na dysku, co było nieproporcjonalnym narzutem w stosunku do prostej aplikacji.

- **Baza SQLite lub format binarny:**
  Odrzucono ze względu na brak czytelności danych bez specjalistycznych narzędzi oraz konieczność dołączania zewnętrznych bibliotek NuGet, co łamałoby regułę prostoty projektu.

---

## 4. Skutki

### Co ta decyzja daje:
- **Pełna czytelność i interoperacyjność:** Plik `notatki.md` jest estetycznym dokumentem Markdown, który można wygodnie przeglądać i edytować w dowolnym edytorze tekstu, podglądzie GitHub czy narzędziach typu Personal Knowledge Base (np. Obsidian).
- **Natychmiastowy dostęp do najnowszych wpisów:** Otwarcie pliku w edytorze od razu pokazuje najświeższe notatki na samej górze strony.
- **Brak zewnętrznych bibliotek:** Parsowanie i zapis opierają się na wbudowanych mechanizmach platformy .NET (`System.IO`, `System.Text.StringBuilder`), co utrzymuje zerowy narzut zależności.
- **Wierność polskich znaków:** Wymuszenie kodowania UTF-8 zapobiega uszkodzeniom polskich znaków diakrytycznych.

### Co ta decyzja utrudnia (kompromisy i wady):
- **Nieefektywność zapisu $O(N)$ i brak skalowalności:** Systemy plików nie obsługują operacji wstawiania na początku pliku (*prepend*). Zapisanie każdej pojedynczej notatki wymaga wczytania całego pliku do pamięci RAM, alokacji nowego ciągu znaków i ponownego zapisu całego pliku na dysk od zera. Wraz ze wzrostem bazy notatek do setek wpisów operacja ta staje się coraz bardziej kosztowna pod kątem czasu procesora i I/O dysku.
- **Wysokie ryzyko utraty danych przy awarii:** Ponieważ cała zawartość pliku jest nadpisywana w jednej operacji `File.WriteAllText` (bez mechanizmu zapisu atomowego do pliku tymczasowego z podmianą), nagła awaria zasilania, zawieszenie systemu operacyjnego lub błąd I/O w trakcie zapisu może doprowadzić do wyzerowania lub uszkodzenia całego pliku `notatki.md` ze wszystkimi historycznymi notatkami użytkownika.
- **Kruchość parsowania przy treściach zawierających Markdown:** Ponieważ początek notatki jest rozpoznawany wyłącznie po prefiksie wiersza `## `, jeśli użytkownik w treści swojej notatki użyje nagłówka drugiego stopnia (np. wklei notatki z wykładu lub kod ze znacznikiem `##`), parser potraktuje ten wiersz jako nową notatkę, co rozbije spójność wpisu i zafałszuje datę oraz tytuł.
- **Znaczne utrudnienie edycji i usuwania pojedynczych wpisów:** Brak unikalnych identyfikatorów (ID/UUID) w nagłówku sprawia, że aktualizacja lub usunięcie konkretnej notatki wymaga żmudnego parsowania całego dokumentu tekstowego, odnalezienia odpowiedniego zakresu wierszy i ponownego zapisu. Dwie notatki o identycznym tytule utworzone w tej samej minucie są programistycznie nierozróżnialne.
- **Blokada współbieżności:** Monolityczny plik uniemożliwia bezpieczną pracę wielu wątków czy procesów jednocześnie bez mechanizmów blokowania pliku na poziomie systemu operacyjnego.
