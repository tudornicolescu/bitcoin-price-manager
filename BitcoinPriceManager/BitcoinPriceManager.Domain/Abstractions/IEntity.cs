namespace BitcoinPriceManager.Domain.Abstractions;

public interface IEntity
{
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
}

public interface IEntity<T> : IEntity
{
    public T Id { get; set; }
}
