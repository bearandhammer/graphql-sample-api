using Microsoft.EntityFrameworkCore.Internal;
using PizzaOrder.Data.Entities;
using PizzaOrder.Data.Enums;
using System.Collections.Generic;
using System.Linq;

namespace PizzaOrder.Data
{
    public static class DataSeeder
    {
        public static void EnsureDataSeeding(this PizzaDBContext dbContext)
        {
            if (!dbContext.OrderDetails.Any())
            {
                dbContext.OrderDetails.AddRange(new List<OrderDetails>
                {
                    new OrderDetails("1 Test Street", "Test Town", "07589654745", 100),
                    new OrderDetails("2 Test Street", "Test Town", "07545864741", 180),
                    new OrderDetails("3 Test Street", "Other Test Town", "0776584524", 50),
                    new OrderDetails("4 Test Street", "Other Test Town", "07685045863", 120),
                });

                dbContext.SaveChanges();
            }

            if (!dbContext.PizzaDetails.Any())
            {
                dbContext.PizzaDetails.AddRange(new List<PizzaDetails>
                {
                    new PizzaDetails("Neapolitan Pizza", Toppings.ExtraCheese | Toppings.Onions, 100, 11, 1),
                    new PizzaDetails("Greek Pizza", Toppings.Mushrooms | Toppings.Pepperoni, 160, 16, 2),
                    new PizzaDetails("Neapolitan Pizza", Toppings.ExtraCheese | Toppings.Onions, 80, 11, 3),
                    new PizzaDetails("Sicilian Pizza", Toppings.None, 50, 9, 4)
                });

                dbContext.SaveChanges();
            }
        }
    }
}
