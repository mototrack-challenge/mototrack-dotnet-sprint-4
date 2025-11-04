namespace MT.Domain.Entities;

public class PageResultModel<T>
{
    public required T Data { get; set; }
    public int Deslocamento { get; set; }
    public int RegistrosRetornados { get; set; }
    public int TotalRegistros { get; set; }
}
