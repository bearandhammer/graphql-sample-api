using PizzaOrder.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PizzaOrder.Data.Entities
{
    public class PizzaDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(40)]
        public string Name { get; set; }

        [Required]
        public Toppings Toppings { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public int Size { get; set; }

        [Required]
        public int OrderDetailsId { get; set; }

        public PizzaDetails()
        {
        }

        public PizzaDetails(
            string name,
            Toppings toppings,
            double price,
            int size,
            int orderDetailsId)
        {
            Name = name;
            Toppings = toppings;
            Price = price;
            Size = size;
            OrderDetailsId = orderDetailsId;
        }
    }
}
