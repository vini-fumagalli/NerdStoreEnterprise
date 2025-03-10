using System.Text.Json.Serialization;
using NSE.Core.Messages;

namespace NSE.Core.DomainObjects;

public abstract class Entity
{
    public int Id { get; protected set; }

    [JsonIgnore] private List<Event> _notificacoes;
    [JsonIgnore] public IReadOnlyCollection<Event> Notificacoes => _notificacoes;

    public void AdicionarEvento(Event evento)
    {
        _notificacoes = _notificacoes is not null ? _notificacoes : new List<Event>();
        _notificacoes.Add(evento);
    }

    public void RemoverEvento(Event evento)
    {
        _notificacoes?.Remove(evento);
    }

    public void LimparEventos()
    {
        _notificacoes?.Clear();
    }
    
    public override bool Equals(object obj)
    {
        var compareTo = obj as Entity;

        if (ReferenceEquals(this, compareTo)) return true;
        if (ReferenceEquals(null, compareTo)) return false;

        return Id.Equals(compareTo.Id);
    }

    public static bool operator ==(Entity a, Entity b)
    {
        if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
            return true;

        if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(Entity a, Entity b)
    {
        return !(a == b);
    }

    public override int GetHashCode()
    {
        return (GetType().GetHashCode() * 907) + Id.GetHashCode();
    }

    public override string ToString()
    {
        return $"{GetType().Name} [Id={Id}]";
    }
}