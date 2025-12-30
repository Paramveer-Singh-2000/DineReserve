namespace DineReserve.Domain.Entities;

public class Restaurant
{
     public int Id { get; set; } 
     public string Name { get; set; }
     public string Description { get; set; }
     public string Category { get; set; }
     public bool HasDelivery { get; set; }
     
     public string? ContactEmail { get; set; }
     public string? ContactPhone { get; set; }
     
     public Address? Address { get; set; }
     public List<Dish> Dishes { get; set; } = new(); 
}