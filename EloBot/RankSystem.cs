using System.Collections.Generic;
using System.Linq;

public class RankSystem
{
    private readonly List<Rank> _ranks = new List<Rank>
    {
        new Rank("Bronze", 0, 999),
        new Rank("Silver", 1000, 1199),
        new Rank("Gold", 1200, 1399),
        new Rank("Platinum", 1400, 1599),
        new Rank("Diamond", 1600, 1799),
        new Rank("Master", 1800, int.MaxValue)
    };

    public string GetRankForElo(int elo)
    {
        return _ranks.First(r => elo >= r.MinElo && elo <= r.MaxElo).Name;
    }

    public string GetAvailableRanks()
    {
        return "Available ranks:\n" + string.Join("\n", _ranks.Select(r => $"{r.Name}: {r.MinElo}-{r.MaxElo}"));
    }

    private class Rank
    {
        public string Name { get; }
        public int MinElo { get; }
        public int MaxElo { get; }

        public Rank(string name, int minElo, int maxElo)
        {
            Name = name;
            MinElo = minElo;
            MaxElo = maxElo;
        }
    }
}