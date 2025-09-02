# E-Commerce System

## DB Schema

**System Diagram**
![DB Schema](./Images/E-CommerceSystemDiagram.png)
**[DB Schema File](./Schema/E-CommerceSystemSchema.sql)**

## System Structure

### Models

- User
```CSharp
    public class User
    {
        // Fields ...
        public int UID { get; set; }
        public string UName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone {  get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        //Navigation Properties ...
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
```
- Order
```CSharp
    public class Order
    {
        // Fields ...
        public int OID { get; set; }
        public int UID { get; set; } // Foreign Key to User Table ...
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        //Navigation Properties ...
        public virtual User User { get; set; }
        public virtual ICollection<OrderProducts> OrderProducts { get; set; }
    }
```
- Product
```CSharp
    public class Product
    {
        // Fields ...
        public int PID { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public decimal OverallRating { get; set; }
        //Navigation Properties ...
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<OrderProducts> OrderProducts { get; set; }
    }
```
- Review
```CSharp
    public class Review
    {
        // Fields ...
        public int ReviewID { get; set; }
        public int PID { get; set; } // Foreign Key to Product Table ...
        public int UID { get; set; } // Foreign Key to User Table ...
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime ReviewDate { get; set; }
        //Navigation Properties ...
        public virtual Product Product { get; set; }
        public virtual User User { get; set; }
    }
```
- OrderProducts
```CSharp
    public class OrderProducts
    {
        // Fields ...
        public int OID { get; set; } // Foreign Key to Order Table ...
        public int PID { get; set; } // Foreign Key to Product Table ...
        public int Quantity { get; set; }
        //Navigation Properties ...
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
```
### DbContext
### Repositories
- UserRepository
- ProductRepository
- OrderRepository
- ReviewRepository
- OrderProductsRepository

### Services
- UserService
- ProductService
- OrderService
- ReviewService
- OrderProductsService

### Controllers
- UserController
- ProductController
- OrderController
- ReviewController


## System Features

### User Features
### Product Features
### Order Features
### Review Features
