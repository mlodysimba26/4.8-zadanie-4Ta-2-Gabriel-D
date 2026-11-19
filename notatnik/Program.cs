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

            if (line.StartsWith("## ") && line.Contains(" — "))
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
}

// ============================================================
// Program — pętla konsolowa
// ============================================================
class Program
{
    static void Main(string[] args)
    {
        string storagePath = Path.Combine(Directory.GetCurrentDirectory(), "notatki.md");
        var storage = new NotesStorage(storagePath);

        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        Console.WriteLine("=== Notatnik-v2 (wariant konsolowy) ===");
        Console.WriteLine($"Plik magazynu: {storagePath}");
        Console.WriteLine("Polecenia: [n] nowa notatka  [l] lista  [q] wyjście");
        Console.WriteLine();

        bool running = true;
        while (running)
        {
            Console.Write("> ");
            string? cmd = Console.ReadLine()?.Trim().ToLower();

            switch (cmd)
            {
                case "n":
                    AddNote(storage);
                    break;
                case "l":
                    ListNotes(storage);
                    break;
                case "q":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Nieznane polecenie. Użyj: n, l, q");
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
            title = Console.ReadLine() ?? "";
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

        try
        {
            storage.SaveNote(note);
            Console.WriteLine($"[OK] Zapisano: {note.DisplayHeader}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"[Błąd zapisu] {ex.Message}");
            Console.WriteLine("[Dane zachowane — podaj polecenie ponownie po usunięciu blokady]");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"[Błąd dostępu] {ex.Message}");
            Console.WriteLine("[Dane zachowane — podaj polecenie ponownie po usunięciu blokady]");
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
}
