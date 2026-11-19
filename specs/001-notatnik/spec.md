# Specyfikacja: Podręczny Notatnik Osobisty (Notatnik-v2)

Dokument określa wymagania funkcjonalne, ograniczenia zakresowe oraz kryteria akceptacji dla modułu notatnika osobistego tworzonego w architekturze Spec-Driven Development.

---

## 1. Cel

Stworzenie minimalistycznej, w pełni lokalnej aplikacji do natychmiastowego zapisywania i przeglądania podręcznych notatek tekstowych. Program ma umożliwiać utrwalenie myśli w czasie poniżej kilku sekund, zapisując dane w czytelnym dla człowieka formacie Markdown z zachowaniem odwróconej chronologii (najnowsze wpisy na początku), bez potrzeby połączenia z siecią i bez uzależnienia od zamkniętych formatów bazodanowych.

---

## 2. Dla kogo

- **Użytkownik docelowy:** Osoba pracująca lokalnie na pojedynczym komputerze (deweloper, uczeń, pracownik biurowy), potrzebująca szybkiego notatnika do codziennych zapisków, list zadań i notatek podręcznych.
- **Charakterystyka potrzeb:** Ceni natychmiastową gotowość narzędzia (uruchomienie poniżej dwóch sekund), prywatność (dane nie opuszczają urządzenia), przejrzysty interfejs oraz bezpośredni dostęp do swoich danych w zewnętrznych edytorach tekstu. Nie potrzebuje kont użytkowników, logowania, pracy grupowej ani chmurowej synchronizacji.

---

## 3. Co robi

- Prezentuje prosty interfejs umożliwiający wpisanie tytułu i treści notatki oraz jej zapis.
- Wykorzystuje plik magazynu danych w formacie Markdown o domyślnej nazwie `notatki.md`, zlokalizowany w bieżącym katalogu roboczym programu.
- Przyjmuje jednoliniowy tytuł notatki.
- Przyjmuje wieloliniową treść notatki.
- Udostępnia polecenie zapisu notatki, które utrwala ją w pliku magazynu danych na dysku.
- Blokuje operację zapisu (widocznie i funkcjonalnie), dopóki pole tytułu pozostaje puste lub zawiera wyłącznie białe znaki.
- Formatuje każdy wpis w standardzie Markdown z nagłówkiem poziomu drugiego: `## YYYY-MM-DD HH:mm — Tytuł`, po którym następuje pusty wiersz, treść notatki i kolejny pusty wiersz oddzielający wpisy.
- Zapisuje wpisy w porządku odwróconej chronologii — każda nowo utworzona notatka wstawiana jest na samym początku pliku magazynu, powyżej starszych notatek.
- Zapisuje i odczytuje wszystkie dane w standardzie kodowania UTF-8, bezbłędnie zachowując polskie znaki diakrytyczne (`ąćęłńóśźżĄĆĘŁŃÓŚŹŻ`).
- Natychmiast po udanym zapisie czyści pola formularza i wyświetla potwierdzenie operacji.
- Wyświetla listę wszystkich dotychczas zapisanych notatek od najnowszej do najstarszej z widocznym tytułem i znacznikiem czasu.
- Przy starcie programu automatycznie wczytuje istniejący plik magazynu i prezentuje listę wszystkich zapisanych dotąd notatek w czasie krótszym niż dwie sekundy.
- W przypadku błędu wejścia/wyjścia (np. plik zablokowany, brak uprawnień zapisu) nie ulega awarii procesu, lecz wyświetla zrozumiały komunikat o błędzie i zachowuje wpisany tekst, umożliwiając ponowienie próby.

---

## 4. Czego nie robi

- Nie łączy się z internetem, nie odpytuje zewnętrznych API ani nie wysyła telemetrii (zgodnie z zasadą 1 Konstytucji).
- Nie wymaga ani nie posiada systemu kont, logowania, profili, haseł ani ról (zgodnie z zasadą 3 Konstytucji).
- Nie umożliwia edycji ani usuwania pojedynczych historycznych notatek z poziomu interfejsu w tej wersji programu (ewentualne korekty użytkownik wykonuje bezpośrednio w pliku Markdown).
- Nie stosuje formatowania bogatego, kolorowania składni, osadzania tabel ani dołączania plików graficznych.
- Nie zawiera wyszukiwania pełnotekstowego, tagowania ani hierarchicznych folderów.
- Nie szyfruje pliku magazynu danych — zawartość pozostaje otwartym, bezpośrednio czytelnym tekstem (zgodnie z zasadą 2 Konstytucji).
- Nie integruje się z usługami chmurowymi.

---

## 5. Kryteria akceptacji

### Kryterium 1: Poprawny zapis nowej notatki z formatem daty i tytułu
- **Sytuacja:** Użytkownik wpisuje tytuł „Plan na dzisiaj", treść „1. Napisać specyfikację\n2. Zrobić commit" i zatwierdza zapis.
- **Wynik:** Nowa notatka zostaje zapisana w pliku magazynu z nagłówkiem `## YYYY-MM-DD HH:mm — Plan na dzisiaj` (gdzie data i godzina odpowiadają chwili zapisu), pustą linią, zadaną treścią i kolejną pustą linią. Pola formularza zostają wyczyszczone i pojawia się potwierdzenie zapisu.
- **Sposób sprawdzenia:** Uruchom program, wprowadź powyższy tytuł i treść, zatwierdź zapis. Zweryfikuj, czy pola edycyjne stały się puste i widoczne jest potwierdzenie. Otwórz plik magazynu w edytorze tekstowym i sprawdź obecność nagłówka `## ` z dokładną datą i separatorem `—`.

### Kryterium 2: Blokada zapisu przy pustym tytule
- **Sytuacja:** Pole tytułu notatki jest puste lub zawiera wyłącznie białe znaki (spacje, tabulacje).
- **Wynik:** Operacja zapisu jest zablokowana i wyłączona dla interakcji użytkownika; próba jej wywołania nie wywołuje żadnego efektu i żaden wpis nie zostaje dodany do magazynu.
- **Sposób sprawdzenia:** Uruchom aplikację. Sprawdź stan operacji zapisu bez wpisywania tekstu (brak reakcji). Następnie wprowadź do pola tytułu trzy spacje i sprawdź, czy zapis nadal pozostaje nieaktywny. Wprowadź dowolną treść przy pustym tytule — upewnij się, że zapis wciąż jest zablokowany.

### Kryterium 3: Odwrócona chronologia zapisów (najnowszy wpis na początku)
- **Sytuacja:** W magazynie danych istnieją już wcześniej utrwalone notatki, a użytkownik dodaje kolejny wpis.
- **Wynik:** Nowo utworzona notatka zostaje wstawiona na sam początek pliku magazynu (przed wszystkimi wcześniejszymi wpisami) oraz pojawia się na pierwszej pozycji listy w programie.
- **Sposób sprawdzenia:** Dodaj notatkę o tytule „Pierwsza". Następnie dodaj notatkę o tytule „Druga". Otwórz plik magazynu na dysku i zweryfikuj, że sekcja z notatką „Druga" znajduje się fizycznie wyżej w pliku niż sekcja „Pierwsza". W programie sprawdź, czy notatka „Druga" jest na pierwszym miejscu listy.

### Kryterium 4: Bezbłędna obsługa polskich znaków diakrytycznych (UTF-8)
- **Sytuacja:** Użytkownik wprowadza polskie litery diakrytyczne (`ąćęłńóśźż ĄĆĘŁŃÓŚŹŻ`) w tytule i treści notatki (np. „Zażółć gęślą jaźń") i zatwierdza zapis.
- **Wynik:** Wszystkie polskie litery zostają utrwalone w magazynie danych w kodowaniu UTF-8 i są wyświetlane bez zniekształceń, znaków zapytania czy błędnych sekwencji bajtów.
- **Sposób sprawdzenia:** Wpisz notatkę z tytułem i treścią zawierającą komplet polskich znaków diakrytycznych. Zapisz notatkę, zamknij program, po czym uruchom go ponownie. Zweryfikuj poprawność wyświetlania znaków na liście oraz po otwarciu pliku magazynu w zewnętrznym edytorze z kodowaniem UTF-8.

### Kryterium 5: Trwałość danych i odczyt po ponownym uruchomieniu programu
- **Sytuacja:** Użytkownik zapisał kilka notatek, zamknął aplikację, a następnie uruchomił ją ponownie.
- **Wynik:** Program startuje w czasie krótszym niż dwie sekundy i od razu wczytuje wszystkie dotychczasowe notatki z pliku magazynu, prezentując pełną listę z zachowaniem odwróconej chronologii.
- **Sposób sprawdzenia:** Zapisz trzy zróżnicowane notatki w aplikacji. Zamknij proces programu. Uruchom program ponownie i upewnij się, że lista zawiera dokładnie trzy elementy w prawidłowej kolejności od najnowszej do najstarszej.

### Kryterium 6: Bezpieczeństwo zapisu w przypadku awarii wejścia/wyjścia
- **Sytuacja:** Podczas próby zapisu notatki występuje błąd wejścia/wyjścia na dysku (np. plik magazynu ma zablokowane uprawnienia do zapisu lub jest oznaczony jako tylko do odczytu).
- **Wynik:** Aplikacja nie ulega awarii procesu, wyświetla czytelny komunikat błędu, a wprowadzony tekst w polach tytułu i treści NIE zostaje utracony, umożliwiając ponowienie próby po usunięciu blokady.
- **Sposób sprawdzenia:** Wprowadź tytuł i treść notatki do programu. Skonfiguruj plik magazynu tak, aby proces nie posiadał uprawnień do zapisu (np. atrybut tylko do odczytu lub blokada uprawnień w systemie plików). Spróbuj zapisać notatkę. Sprawdź, czy program nadal działa, czy pojawił się komunikat o błędzie oraz czy wpisany tekst pozostał w polach. Przywróć uprawnienia i zapisz ponownie — notatka powinna zostać pomyślnie zapisana.

---

## 6. Do ustalenia

- **Zewnętrzne modyfikacje pliku magazynu w trakcie działania programu:** Czy jeśli użytkownik zmodyfikuje zawartość pliku magazynu w zewnętrznym programie podczas gdy aplikacja jest uruchomiona, program powinien automatycznie wykrywać zmiany i odświeżać listę, czy odczyt ma następować jedynie przy starcie i własnych zapisach?
- **Zasady dopuszczalności notatki z pustą treścią:** Czy dozwolony jest zapis notatki, która posiada poprawny tytuł, lecz pole treści jest całkowicie puste, czy oba pola muszą być wypełnione?
- **Odporność parsera na znaczniki nagłówków Markdown w treści:** W jaki sposób program powinien zabezpieczać treść notatki, jeśli użytkownik wpisze w niej wiersz zaczynający się od `## `, aby parser nie zinterpretował go jako początku nowego wpisu?
- **Zabezpieczenie przed uszkodzeniem pliku przy nagłej awarii (zapis atomowy):** W jaki sposób zrealizować zapis do pliku magazynu poprzez plik tymczasowy z atomową zamianą w systemie plików, aby spełnić Zasadę 6 Konstytucji i wyeliminować ryzyko utraty danych w przypadku nagłego zaniku zasilania w trakcie operacji zapisu?
- **Reakcja na brak lub uszkodzenie pliku magazynu:** Czy przy pierwszym uruchomieniu, gdy plik magazynu jeszcze nie istnieje, program powinien utworzyć pusty plik od razu, czy dopiero przy pierwszym zapisie; oraz jak program powinien zareagować na uszkodzoną strukturę pliku?
- **Obsługa kolizji znacznika czasu w tej samej minucie:** Czy format czasu `YYYY-MM-DD HH:mm` powinien zostać rozszerzony o sekundy lub unikalny sufiks, aby zapobiec powstawaniu identycznych nagłówków przy szybkim dodaniu dwóch notatek?
- **Tolerancja wariantów separatora tytułu:** Czy parser powinien akceptować zarówno długą pauzę `—` (em-dash), jak i zwykły myślnik `-` (dywiz) oraz półpauzę `–`, na wypadek ręcznej edycji pliku przez użytkownika?
