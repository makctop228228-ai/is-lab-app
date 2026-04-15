using IsLabApp.Models;

namespace IsLabApp.Services;

public interface INoteService
{
    IEnumerable<Note> GetAll();
    Note? GetById(int id);
    Note Create(Note note);
    bool Delete(int id);
}