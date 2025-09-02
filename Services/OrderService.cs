using AutoMapper;
using E_CommerceSystem.Models;
using E_CommerceSystem.Repositories;

namespace E_CommerceSystem.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepo _orderRepo;
        private readonly IProductService _productService;
        private readonly IOrderProductsService _orderProductsService;
        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepo orderRepo,
            IProductService productService,
            IOrderProductsService orderProductsService,
            IMapper mapper)
        {
            _orderRepo = orderRepo;
            _productService = productService;
            _orderProductsService = orderProductsService;
            _mapper = mapper;
        }

        // get all orders for login user ...
        public List<OrderProducts> GetAllOrders(int uid)
        {
            var orders = _orderRepo.GetOrderByUserId(uid);
            if (orders == null || !orders.Any())
                throw new InvalidOperationException($"No orders found for user ID {uid}.");

            var allOrderProducts = new List<OrderProducts>();
            foreach (var order in orders)
            {
                var orderProducts = _orderProductsService.GetOrdersByOrderId(order.OID);
                if (orderProducts != null)
                    allOrderProducts.AddRange(orderProducts);
            }

            return allOrderProducts;
        }

        // get order by order id for the login user ...
        public IEnumerable<OrdersOutputOTD> GetOrderById(int oid, int uid)
        {
            var order = _orderRepo.GetOrderById(oid);
            if (order == null)
                throw new InvalidOperationException("No orders found.");

            if (order.UID != uid)
                return Enumerable.Empty<OrdersOutputOTD>();

            var products = _orderProductsService.GetOrdersByOrderId(oid);
            var items = new List<OrdersOutputOTD>();

            foreach (var op in products)
            {
                // Ensure navigations are available for AutoMapper ...
                var product = _productService.GetProductById(op.PID);
                op.product = product;   // set navigation for mapping ...
                op.Order = order;       // set navigation for mapping ...

                // AutoMapper builds OrdersOutputOTD (OrderDate, ProductName, Quantity, TotalAmount) ...
                var dto = _mapper.Map<OrdersOutputOTD>(op);
                items.Add(dto);
            }

            return items;
        }
        // get all orders for a specific user by user id ...
        public IEnumerable<Order> GetOrderByUserId(int uid)
        {
            var order = _orderRepo.GetOrderByUserId(uid);
            if (order == null)
                throw new KeyNotFoundException($"order with user ID {uid} not found.");

            return order;
        }
        // delete order by order id ...
        public void DeleteOrder(int oid)
        {
            var order = _orderRepo.GetOrderById(oid);
            if (order == null)
                throw new KeyNotFoundException($"order with ID {oid} not found.");

            _orderRepo.DeleteOrder(oid);
            throw new Exception($"order with ID {oid} is deleted");
        }
        // add new order ...
        public void AddOrder(Order order)
        {
            _orderRepo.AddOrder(order);
        }
        // update existing order ...
        public void UpdateOrder(Order order)
        {
            _orderRepo.UpdateOrder(order);
        }

        // Places an order for the given list of items and user ID.
        public void PlaceOrder(List<OrderItemDTO> items, int uid)
        {
            Product existingProduct = null;
            decimal totalOrderPrice = 0;

            // Validate stock for each item first ...
            foreach (var item in items)
            {
                existingProduct = _productService.GetProductByName(item.ProductName)
                    ?? throw new Exception($"{item.ProductName} not Found");

                if (existingProduct.Stock < item.Quantity)
                    throw new Exception($"{item.ProductName} is out of stock");
            }

            // Create order
            var order = new Order { UID = uid, OrderDate = DateTime.Now, TotalAmount = 0 };
            AddOrder(order);

            // Process items ...
            foreach (var item in items)
            {
                existingProduct = _productService.GetProductByName(item.ProductName)!;

                var totalPrice = item.Quantity * existingProduct.Price;// Calculate total price for this item ...
                existingProduct.Stock -= item.Quantity;// Deduct stock ...
                totalOrderPrice += totalPrice; // Accumulate to order total ...

                // Use AutoMapper for the basic mapping, then enrich with FKs ...
                var orderProducts = _mapper.Map<OrderProducts>(item);
                orderProducts.OID = order.OID;
                orderProducts.PID = existingProduct.PID;

                _orderProductsService.AddOrderProducts(orderProducts);
                _productService.UpdateProduct(existingProduct);
            }

            // Update order total
            order.TotalAmount = totalOrderPrice;
            UpdateOrder(order);
        }
    }
}
