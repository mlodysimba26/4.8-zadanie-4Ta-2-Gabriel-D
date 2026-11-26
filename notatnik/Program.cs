using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

// ============================================================
// NoteModel — model pojedynczej notatki
// ============================================================
class NoteModel
{
    public DateTime Timestamp { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";

    public string DisplayHeader =>
        $"{Timestamp:yyyy-MM-dd HH:mm} — {Title}";
}

// ============================================================
// NotesStorage — operacje I/O na pliku notatki.md
// ============================================================
class NotesStorage
{
    private readonly string _filePath;

    public NotesStorage(string filePath) => _filePath = filePath;

    public List<NoteModel> LoadNotes()
    {
        var notes = new List<NoteModel>();
        if (!File.Exists(_filePath))
            return notes;

        string raw = File.ReadAllText(_filePath, Encoding.UTF8);
        string[] lines = raw.Split('\n');

        NoteModel? current = null;
        var contentLines = new List<string>();

        foreach (string rawLine in lines)
        {
            string line = rawLine.TrimEnd('\r');

            if (line.StartsWith("## ") && (line.Contains(" \u2014 ") || line.Contains(" - ")))
            {
                if (current != null)
                {
                    current.Content = string.Join("\n", contentLines).Trim();
                    notes.Add(current);
                }
                // Parsuj nagłówek: ## YYYY-MM-DD HH:mm — Tytuł
                string header = line.Substring(3); // bez "## "
                int sepIdx = header.IndexOf(" \u2014 "); // em-dash
                if (sepIdx < 0) sepIdx = header.IndexOf(" - ");
                if (sepIdx >= 0)
                {
                    string dateStr = header.Substring(0, sepIdx).Trim();
                    string title = header.Substring(sepIdx).TrimStart(' ', '\u2014', '-').Trim();
                    if (DateTime.TryParseExact(dateStr, "yyyy-MM-dd HH:mm",
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None, out DateTime ts))
                    {
                        current = new NoteModel { Timestamp = ts, Title = title };
                        contentLines = new List<string>();
                        continue;
                    }
                }
                // Jeśli nie pasuje — traktuj jako treść poprzedniej notatki
                if (current != null) contentLines.Add(line);
            }
            else
            {
                if (current != null)
                    contentLines.Add(line);
            }
        }

        if (current != null)
        {
            current.Content = string.Join("\n", contentLines).Trim();
            notes.Add(current);
        }

        return notes;
    }

    public void SaveNote(NoteModel note)
    {
        string newBlock = $"## {note.Timestamp:yyyy-MM-dd HH:mm} \u2014 {note.Title}\n\n{note.Content}\n\n";

        string existing = File.Exists(_filePath)
            ? File.ReadAllText(_filePath, Encoding.UTF8)
            : "";

        string combined = newBlock + existing;
        File.WriteAllText(_filePath, combined, Encoding.UTF8);
    }

    public List<NoteModel> SearchNotes(string query)
    {
        var all = LoadNotes();
        var results = new List<NoteModel>();
        foreach (var note in all)
        {
            if (note.DisplayHeader.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                note.Content.Contains(query, StringComparison.OrdinalIgnoreCase))
            {
                results.Add(note);
            }
        }
        return results;
    }
}

// ============================================================
// Program — pętla konsolowa
// ============================================================
class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = new UTF8Encoding(false);
        Console.InputEncoding = new UTF8Encoding(false);

        string storagePath = Path.Combine(Directory.GetCurrentDirectory(), "notatki.md");
        var storage = new NotesStorage(storagePath);

        Console.WriteLine("=== Notatnik-v2 (wariant konsolowy) ===");
        Console.WriteLine($"Plik magazynu: {storagePath}");
        Console.WriteLine("Polecenia: [n] nowa notatka  [l] lista  [s] szukaj  [q] wyjście");
        Console.WriteLine();

        ListNotes(storage);
        Console.WriteLine();

        bool running = true;
        while (running)
        {
            Console.Write("> ");
            string? rawCmd = Console.ReadLine();
            if (rawCmd == null)
                break;
            string cmd = rawCmd.Trim('\uFEFF', ' ', '\t', '\r', '\n').ToLower();

            switch (cmd)
            {
                case "n":
                    AddNote(storage);
                    break;
                case "l":
                    ListNotes(storage);
                    break;
                case "s":
                    SearchNotesCommand(storage);
                    break;
                case "q":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Nieznane polecenie. Użyj: n, l, s, q");
                    break;
            }
        }
    }

    static void AddNote(NotesStorage storage)
    {
        string title;
        while (true)
        {
            Console.Write("Tytuł notatki: ");
            string? rawTitle = Console.ReadLine();
            if (rawTitle == null)
                return;
            title = rawTitle;
            if (!string.IsNullOrWhiteSpace(title))
                break;
            Console.WriteLine("[Błąd] Tytuł nie może być pusty ani składać się z samych spacji. Podaj tytuł.");
        }

        Console.WriteLine("Treść notatki (zakończ pustą linią):");
        var contentLines = new List<string>();
        while (true)
        {
            string? line = Console.ReadLine();
            if (line == null || line == "")
                break;
            contentLines.Add(line);
        }
        string content = string.Join("\n", contentLines);

        var note = new NoteModel
        {
            Timestamp = DateTime.Now,
            Title = title,
            Content = content
        };

        while (true)
        {
            try
            {
                storage.SaveNote(note);
                Console.WriteLine($"[OK] Zapisano: {note.DisplayHeader}");
                break;
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                string label = ex is UnauthorizedAccessException ? "Błąd dostępu" : "Błąd zapisu";
                Console.WriteLine($"[{label}] {ex.Message}");
                Console.WriteLine($"[Dane zachowane] Tytuł: \"{note.Title}\" | Treść: {note.Content.Length} zn.");
                Console.Write("Ponowić zapis tych samych danych? [t/n]: ");
                string? retry = Console.ReadLine()?.Trim().ToLower();
                if (retry != "t")
                    break;
            }
        }
    }

    static void ListNotes(NotesStorage storage)
    {
        try
        {
            var notes = storage.LoadNotes();
            if (notes.Count == 0)
            {
                Console.WriteLine("[Brak notatek]");
                return;
            }
            Console.WriteLine($"--- {notes.Count} notatek (od najnowszej) ---");
            for (int i = 0; i < notes.Count; i++)
            {
                Console.WriteLine($"  [{i + 1}] {notes[i].DisplayHeader}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Błąd odczytu] {ex.Message}");
        }
    }

    static void SearchNotesCommand(NotesStorage storage)
    {
        Console.Write("Szukana fraza: ");
        string? rawQuery = Console.ReadLine();
        if (rawQuery == null)
            return;
        string query = rawQuery.Trim('\uFEFF');

        try
        {
            var results = storage.SearchNotes(query);
            if (results.Count == 0)
            {
                Console.WriteLine($"[Brak wyników dla frazy: \"{query}\"]");
                return;
            }
            Console.WriteLine($"--- Wyniki wyszukiwania ({results.Count}) ---");
            for (int i = 0; i < results.Count; i++)
            {
                Console.WriteLine($"  [{i + 1}] {results[i].DisplayHeader}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Błąd wyszukiwania] {ex.Message}");
        }
    }
}
