# Dziennik weryfikacji: Notatnik-v2

Każde kryterium ma jeden wpis: polecenie lub fragment wyjścia programu — nie zdanie „działa".

---

## Kryterium 1 — Zapis notatki z formatem daty i tytułu ✅

**Dowód (fragment wyjścia programu i plik magazynu):**

```
$ echo -e "n\nPlan na dzisiaj\n1. Napisac specyfikacje\n2. Zrobic commit\n\nq" | dotnet run --project notatnik

> Tytuł notatki: Treść notatki (zakończ pustą linią):
[OK] Zapisano: 2026-11-19 09:13 — Plan na dzisiaj
```

```
# Zawartość notatki.md po zapisie:
## 2026-11-19 09:13 — Plan na dzisiaj

1. Napisac specyfikacje
2. Zrobic commit

```

Nagłówek zawiera format `## YYYY-MM-DD HH:mm — Tytuł`, pola wyczyszczone ([OK] widoczne), plik na dysku zawiera poprawną strukturę.

---

## Kryterium 2 — Blokada zapisu przy pustym tytule ✅

**Dowód (pusta linia i same spacje — obie zablokowane):**

```
$ printf "n\n\n   \nTytul testowy\n\nq\n" | dotnet run --project notatnik

> Tytuł notatki: [Błąd] Tytuł nie może być pusty ani składać się z samych spacji. Podaj tytuł.
Tytuł notatki: [Błąd] Tytuł nie może być pusty ani składać się z samych spacji. Podaj tytuł.
Tytuł notatki: Treść notatki (zakończ pustą linią):
[OK] Zapisano: 2026-11-19 09:14 — Tytul testowy
```

Program odmówił zapisu przy pustym tytule (1. próba) i przy samych spacjach (2. próba). Dopiero po podaniu niepustego tytułu zapis się powiódł.

---

## Kryterium 3 — Odwrócona chronologia (najnowszy wpis na początku) ✅

**Dowód (fragment pliku magazynu po dodaniu dwóch notatek):**

```
## 2026-11-19 09:15 — Druga

Tresc drugiej notatki

## 2026-11-19 09:14 — Pierwsza

Tresc pierwszej notatki
```

Notatka „Druga" (dodana jako druga) znajduje się fizycznie wyżej w pliku niż „Pierwsza".

---

## Kryterium 4 — Obsługa polskich znaków diakrytycznych (UTF-8) ✅

**Dowód (tytuł i treść z polskimi znakami):**

```
$ cat test-utf8.txt | dotnet run --project notatnik

> Tytuł notatki: Treść notatki (zakończ pustą linią):
[OK] Zapisano: 2026-11-19 09:15 — Zażółć gęślą jaźń
```

```
# Fragment notatki.md:
## 2026-11-19 09:15 — Zażółć gęślą jaźń

ąćęłńóśźż ĄĆĘŁŃÓŚŹŻ
```

Polskie litery zachowane w pliku i wyświetlone bez zniekształceń.

---

## Kryterium 5 — Trwałość danych po ponownym uruchomieniu ✅

**Dowód (lista notatek automatycznie wczytana i wyświetlona od razu przy starcie programu + pomiar czasu < 2 s):**

```powershell
PS> "q" | .\notatnik\bin\Debug\net9.0\notatnik.exe
=== Notatnik-v2 (wariant konsolowy) ===
Plik magazynu: C:\Users\duczynski.gabriel\Documents\4.8-zadanie-4Ta-2-Gabriel-D\notatki.md
Polecenia: [n] nowa notatka  [l] lista  [q] wyjście

--- 5 notatek (od najnowszej) ---
  [1] 2026-11-26 08:09 — Finalny test
  [2] 2026-11-19 09:15 — Zażółć gęślą jaźń
  [3] 2026-11-19 09:15 — Zazolc gesla jazn
  [4] 2026-11-19 09:15 — Druga
  [5] 2026-11-19 09:14 — Pierwsza

> Czas uruchomienia: 207 ms
```

Program uruchomił się w 207 ms (< 2 s) i od razu na starcie wyświetlił wszystkie zapisane notatki w kolejności od najnowszej.

---

## Kryterium 6 — Bezpieczeństwo zapisu przy błędzie I/O ✅

**Dowód (plik z atrybutem tylko do odczytu, zachowanie danych i ponowienie zapisu po zdjęciu blokady):**

```powershell
PS> attrib +r notatki.md
PS> .\notatnik\bin\Debug\net9.0\notatnik.exe
> Tytuł notatki: Treść notatki (zakończ pustą linią):
[Błąd dostępu] Access to the path 'C:\Users\duczynski.gabriel\Documents\4.8-zadanie-4Ta-2-Gabriel-D\notatki.md' is denied.
[Dane zachowane] Tytuł: "Test blokady" | Treść: 15 zn.
Ponowić zapis tych samych danych? [t/n]: # po wykonaniu attrib -r notatki.md i wpisaniu "t":
[OK] Zapisano: 2026-11-26 08:22 — Test blokady
```

Program nie uległ awarii, wyświetlił komunikat po polsku, zachował tytuł i treść w pamięci i po zdjęciu blokady zapisał notatkę bez ponownego wpisywania.

---

## Refleksja: gdzie specyfikacja okazała się błędna lub niepełna

W sekcji 3 „Co robi" specyfikacja (wersja z 4.3) opisywała „jednoekranowy interfejs graficzny" i odwoływała się do konkretnych kontrolek XAML (`TextBox`, `ListBox`, `IsEnabled`). Okazało się to błędne nie dlatego, że opis był nieprawdziwy — GUI spełniłoby te same kryteria — ale dlatego, że specyfikacja mówiła **jak** (kontrolki, framework), a nie **co** (blokada zapisu, lista notatek, podgląd). To wyszło na jaw przy budowie wariantu konsolowego: każde kryterium było możliwe do spełnienia bez GUI, co potwierdziło, że oryginalne sformułowania były wymaganiami implementacyjnymi, a nie funkcjonalnymi.

Drugie miejsce błędu: sekcja 6 „Do ustalenia" używała nazw API (`File.Replace`, `.bak`) zamiast pojęć domenowych (`zapis atomowy`, `kopia zapasowa`). To zidentyfikował już recenzent w `przeglad.md` z 4.3.

**Co napiszę inaczej następnym razem:** kryteria akceptacji napiszę w formie scenariuszy wejście→wyjście, które nie wspominają technologii. Zamiast „przycisk IsEnabled = false" — „operacja zapisu jest zablokowana". Zamiast „ListBox na górze" — „nowy wpis pojawia się na pierwszym miejscu listy". Sekcja „Czego nie robi" i sekcja „Do ustalenia" nie będą zawierać nazw metod ani rozszerzeń plików.

---

# Moduł 002: Wyszukiwanie notatek

## 002 — Kryterium 1: Wyniki dla istniejącej frazy (tytuł i treść) ✅

**Dowód (wyszukanie frazy `zakup` z tytułu oraz `mleko` z treści po dodaniu „Lista zakupów" i „Spotkanie"):**

```powershell
PS> "s`nzakup`nq" | .\notatnik\bin\Debug\net9.0\notatnik.exe
> Szukana fraza: --- Wyniki wyszukiwania (1) ---
  [1] 2026-11-26 08:24 — Lista zakupów

PS> "s`nmleko`nq" | .\notatnik\bin\Debug\net9.0\notatnik.exe
> Szukana fraza: --- Wyniki wyszukiwania (1) ---
  [1] 2026-11-26 08:24 — Lista zakupów
```

Zwrócona została wyłącznie notatka pasująca do frazy (zarówno po tytule, jak i po treści).

---

## 002 — Kryterium 2: Wyszukiwanie niezależne od wielkości liter ✅

**Dowód (wyszukanie `ZAKUP` wielkimi literami przeciwko „Lista zakupów"):**

```powershell
PS> "s`nZAKUP`nq" | .\notatnik\bin\Debug\net9.0\notatnik.exe
> Szukana fraza: --- Wyniki wyszukiwania (1) ---
  [1] 2026-11-26 08:24 — Lista zakupów
```

Dopasowanie wielkości liter jest ignorowane (`StringComparison.OrdinalIgnoreCase`).
