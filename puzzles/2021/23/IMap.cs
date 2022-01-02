using System.Collections.Immutable;

public interface IMap
{
    List<Edge> Edges { get; set; }
    Dictionary<Edge, List<Segment>> Segments { get; set; }

    IEnumerable<(ImmutableDictionary<int, Letter> newState, long energy)> CalculateNewStates(ImmutableDictionary<int, Letter> state, long currentEnergy);
}