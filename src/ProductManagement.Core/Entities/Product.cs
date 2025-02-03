﻿namespace ProductManagement.Core.Entities
{
    public class Product: BaseEntity
    {        
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
    }
}
