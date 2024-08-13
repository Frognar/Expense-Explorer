using System.Collections;

namespace ExpenseExplorer.GUI.Helpers;

public sealed class QueryParameters : IEnumerable<string>
{
  private readonly List<string> _parameters = [];

  public IEnumerator<string> GetEnumerator() => _parameters.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

  public void Add(decimal? value, string name)
  {
    if (value.HasValue)
    {
      _parameters.Add($"{name}={value.Value}");
    }
  }

  public void Add(string? value, string name)
  {
    if (!string.IsNullOrWhiteSpace(value))
    {
      _parameters.Add($"{name}={value.Trim()}");
    }
  }

  public void Add(DateTime? value, string name)
  {
    if (value.HasValue)
    {
      _parameters.Add($"{name}={value.Value:MM/dd/yyyy}");
    }
  }

  public void Add(int? value, string name)
  {
    if (value.HasValue)
    {
      _parameters.Add($"{name}={value.Value}");
    }
  }

  public override string ToString()
  {
    return string.Join("&", _parameters);
  }
}
