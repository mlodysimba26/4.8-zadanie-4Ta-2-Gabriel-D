# Konstytucja projektu Notatnik-v2

Niniejszy dokument określa fundamentalne inwarianty i nienaruszalne zasady systemu, które muszą pozostać prawdziwe w każdej wersji programu, niezależnie od dodawanych w przyszłości funkcji, refaktoryzacji czy zmian technologicznych.

---

### 1. Całkowita lokalność i brak zależności sieciowych (Local-First)
Dane użytkownika nigdy nie opuszczają jego komputera. Aplikacja działa w stu procentach w trybie offline — nie wymaga aktywnego połączenia z internetem, nie komunikuje się z żadnym zewnętrznym API ani chmurą, nie gromadzi i nie wysyła telemetrii ani analityki.

### 2. Trwałość danych i brak zamknięcia w aplikacji (Zero Lock-In)
Notatki są zapisywane w otwartym, czytelnym dla człowieka formacie tekstowym w kodowaniu UTF-8. Użytkownik zachowuje pełny wgląd i możliwość bezpośredniej edycji swoich notatek w dowolnym zewnętrznym edytorze tekstu bez pośrednictwa naszej aplikacji i bez ryzyka utraty dostępu do swoich danych po jej odinstalowaniu.

### 3. Model architektury dla jednego użytkownika (Single-User Focus)
Program jest tworzony wyłącznie dla jednej osoby pracującej lokalnie na jednym stanowisku komputerowym. W systemie nie ma mechanizmu kont użytkowników, logowania, ról, haseł, uwierzytelniania ani wielodostępowej synchronizacji przez sieć.

### 4. Natychmiastowa gotowość do działania (Performance Budget)
Aplikacja pozostaje lekkim, natychmiastowym narzędziem pracy podręcznej. Czas od uruchomienia programu do pełnej gotowości do edycji i zapisu notatek nie przekracza dwóch sekund, niezależnie od liczby modułów pomocniczych, jakie zostaną dodane w przyszłości.

### 5. Spójność językowa kontaktu z użytkownikiem (Native Polish Interface)
Wszystkie teksty prezentowane użytkownikowi w interfejsie graficznym — w tym etykiety kontrolek, komunikaty o stanie operacji oraz powiadomienia o błędach — są zredagowane wyłącznie w języku polskim. Użytkownik nie styka się z nieprzetłumaczonymi terminami technicznymi.

### 6. Bezwzględny priorytet nienaruszalności istniejących danych (Data Safety First)
Żaden błąd walidacji, nieobsłużony wyjątek w nowej funkcji ani awaria procesu aplikacji nie ma prawa doprowadzić do utraty lub uszkodzenia dotychczas zgromadzonych notatek użytkownika. Operacje na plikach muszą chronić istniejącą zawartość przed przypadkowym zniszczeniem.
