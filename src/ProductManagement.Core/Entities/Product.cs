namespace ProductManagement.Core.Entities
{
    public class Product: BaseEntity
    {        
        public string Name { get; private set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }

        // Private default constructor for EF
        private Product()
        {
            Name = "Default Product Name";
            Price = 0.0m;
            Description = string.Empty;
        }

        private Product(string name, decimal price, string description)
        {
            Name = name;
            Price = price;
            Description = description;
        }

        private static void ValidateInputs(string name, decimal price, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"{nameof(name)} cannot be null or empty.", nameof(name));
            }

            if(name.Length > 100)
            {
                throw new ArgumentException($"{nameof(name)} must be less than 100 characters.", nameof(name));
            }

            if (price <= 0)
            {
                throw new ArgumentException($"{nameof(price)} must be greater than zero.", nameof(price));
            }

            if (description != null && description.Length > 1000)
            {
                throw new ArgumentException($"{nameof(description)} must be less than 1000 characters.", nameof(description));
            }
        }

        public static Product Create(string name, decimal price, string description)
        {
            ValidateInputs(name, price, description);
            return new Product(name, price, description);
        }

        public void Update(string name, decimal price, string description)
        {
            ValidateInputs(name, price, description);
            Name = name;
            Price = price;
            Description = description;
            UpdateModifiedOn();
        }
    }
}
