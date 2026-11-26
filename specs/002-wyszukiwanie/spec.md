# Specyfikacja: Wyszukiwanie notatek (Notatnik-v2 — moduł 002)

Dokument określa wymagania funkcjonalne i kryteria akceptacji dla funkcji wyszukiwania notatek w istniejącym magazynie `notatki.md`.

---

## 1. Cel

Umożliwienie użytkownikowi szybkiego odnalezienia notatek zawierających określone słowo kluczowe — zarówno w tytule, jak i w treści — bez konieczności ręcznego przewijania pliku magazynu.

---

## 2. Dla kogo

Ten sam użytkownik co w specyfikacji 001-notatnik: osoba pracująca lokalnie, która gromadzi notatki przez dłuższy czas i potrzebuje odnaleźć konkretny wpis spośród wielu.

---

## 3. Co robi

- Przyjmuje od użytkownika frazę wyszukiwania (jeden lub więcej wyrazów).
- Przeszukuje wczytane notatki — zarówno nagłówki (tytuły i znaczniki czasu), jak i treść notatek.
- Dopasowuje frazę jako ciąg znaków (podciąg) bez rozróżniania wielkości liter.
- Zwraca i wyświetla listę pasujących notatek w formacie identycznym z listą wszystkich notatek: znacznik czasu i tytuł, posortowane od najnowszej.
- Jeśli żadna notatka nie pasuje do frazy, wyświetla wyraźny komunikat o braku wyników.
- Pusta fraza wyszukiwania (lub złożona wyłącznie z białych znaków) jest traktowana jak brak filtra — wyświetlana jest pełna lista notatek.

---

## 4. Czego nie robi

- Nie obsługuje wyrażeń regularnych ani operatorów logicznych (`AND`, `OR`, `NOT`).
- Nie zmienia zawartości pliku magazynu (operacja wyłącznie odczytu).
- Nie zapamiętuje historii wyszukiwania między sesjami.
- Nie obsługuje wyszukiwania po zakresie dat ani podświetlania trafień w treści.

---

## 5. Kryteria akceptacji

### Kryterium 1: Wyniki dla istniejącej frazy
- **Sytuacja:** Plik magazynu zawiera notatki, przynajmniej jedna z nich zawiera frazę „zakupy" w tytule lub treści.
- **Wynik:** Program wyświetla listę wszystkich notatek pasujących do frazy „zakupy", każda ze znacznikiem czasu i tytułem.
- **Sposób sprawdzenia:** Dodaj notatkę z tytułem „Lista zakupów" i notatkę bez słowa „zakupy". Uruchom wyszukiwanie frazy „zakupy". Zweryfikuj, że wynik zawiera tylko pierwszą notatkę.

### Kryterium 2: Wyszukiwanie niezależne od wielkości liter
- **Sytuacja:** Użytkownik wpisuje frazę „ZAKUPY" (wielkie litery), a notatka zawiera słowo „zakupy" (małe litery).
- **Wynik:** Notatka jest zwracana w wynikach wyszukiwania.
- **Sposób sprawdzenia:** Dodaj notatkę z tytułem „Lista zakupów". Wyszukaj frazę „ZAKUPY". Sprawdź, że notatka pojawia się w wynikach.

### Kryterium 3: Brak wyników — komunikat
- **Sytuacja:** Żadna notatka nie zawiera wpisanej frazy.
- **Wynik:** Program wyświetla komunikat informujący o braku pasujących notatek.
- **Sposób sprawdzenia:** Wyszukaj frazę, która na pewno nie istnieje w żadnej notatce (np. `xyz_nieistniejacy_tekst_123`). Sprawdź, że wyświetlony jest komunikat o braku wyników, a nie pusta lista.

### Kryterium 4: Pusta fraza — pełna lista
- **Sytuacja:** Użytkownik zatwierdza wyszukiwanie bez wpisania żadnej frazy.
- **Wynik:** Program wyświetla wszystkie notatki w kolejności od najnowszej.
- **Sposób sprawdzenia:** Uruchom wyszukiwanie z pustą frazą. Zweryfikuj, że lista wyników jest identyczna z listą wszystkich notatek.

---

## 6. Ustalenia (rozstrzygnięte przed implementacją)

- **Wyszukiwanie wielowyrazowe:** Fraza jest wyszukiwana jako dokładny podciąg znaków (bez dzielenia na niezależne słowa), bez rozróżniania wielkości liter.
- **Podświetlanie trafień:** W wariancie konsolowym wyniki mają ten sam format co lista notatek (znacznik czasu i tytuł), bez podświetlania fragmentów.
- **Wydajność przy dużym pliku:** Przeszukiwanie w pamięci odbywa się po wczytaniu pliku z dysku i mieści się w budżecie czasu odpowiedzi aplikacji.
