using IsLabApp.Models;

namespace IsLabApp.Services;

public class InMemoryNoteService : INoteService
{
    private readonly List<Note> _notes = new();
    private int _nextId = 1;

    public IEnumerable<Note> GetAll() => _notes;

    public Note? GetById(int id) => _notes.FirstOrDefault(n => n.Id == id);

    public Note Create(Note note)
    {
        note.Id = _nextId++;
        note.CreatedAt = DateTime.UtcNow;
        _notes.Add(note);
        return note;
    }

    public bool Delete(int id)
    {
        var note = GetById(id);
        if (note == null) return false;
        _notes.Remove(note);
        return true;
    }
}