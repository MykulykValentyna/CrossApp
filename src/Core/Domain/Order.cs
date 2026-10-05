namespace Core.Domain;

public enum OrderStatus
{
    Draft,    
    Confirmed,
    Cancelled  
}

public sealed class Order
{
    public string Id { get; }
    public OrderStatus Status { get; private set; }

    private Order(string id)
    {
        Id = id;
        Status = OrderStatus.Draft; // При створенні завжди чернетка
    }

    public static Order Create(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID замовлення не може бути порожнім", nameof(id));
            
        return new Order(id);
    }

    public void ChangeStatus(OrderStatus newStatus)
    {
        Status = (Status, newStatus) switch
        {
            (OrderStatus.Draft, OrderStatus.Confirmed) => newStatus,
            (OrderStatus.Draft, OrderStatus.Cancelled) => newStatus,
            
            (OrderStatus.Confirmed, OrderStatus.Cancelled) => 
                throw new InvalidOperationException($"Замовлення {Id} вже підтверджене, його не можна скасувати."),
                
            var (oldState, newState) when oldState == newState => 
                throw new InvalidOperationException($"Замовлення вже перебуває у статусі {oldState}."),
                
            _ => throw new InvalidOperationException($"Недопустимий перехід статусу з {Status} у {newStatus}.")
        };
    }

    public override string ToString() => $"Замовлення {Id} | Статус: {Status}";
}