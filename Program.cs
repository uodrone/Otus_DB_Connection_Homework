using ConsoleApp3.Interfaces;
using ConsoleApp3.Models;
using LinqToDB;
using System.Linq.Expressions;
using static LinqToDB.Common.Configuration;

namespace ConsoleApp3
{
    internal class Program
    {
        static string _connectionString = "Host=localhost;Port=5432;Database=Shop;Username=postgres;Password=SomePassword;";
        static void Main(string[] args)
        {


            AddCustomer("Иван", "Иванов", 25);
            AddCustomer("Мария", "Петрова", 30);
            AddCustomer("Сергей", "Сидоров", 40);

            UpdateCustomer(2, "Порфирий", "Димитриевич", 77);
            UpdateCustomer(3, "Серафим", "Серафимов", 55);

            var CustomerList = GetCustomersByNameStarts("С");
            CustomerList.ForEach(customer =>
            {
                Console.WriteLine($"{customer.firstname} {customer.lastname}: {customer.age} годиков");
            });

            var Customer = GetById<customer>(1);
            if (Customer != null)
            {
                Console.WriteLine($"Customer: {Customer.firstname} {Customer.lastname}, Age: {Customer.age}");
            }

            var Order = GetById<order>(1);
            if (Order != null)
            {
                Console.WriteLine($"Order: CustomerID: {Order.customerid}, ProductID: {Order.productid}, Quantity: {Order.quantity}");
            }

            var Product = GetById<product>(1);
            if (Product != null)
            {
                Console.WriteLine($"Product: {Product.name}, Price: {Product.price}, Stock: {Product.stockquantity}");
            }


            var purchasedProducts = GetPurchasedProducts(1);

            if (purchasedProducts.Any())
            {
                foreach (var (productId, productName, totalQuantity) in purchasedProducts)
                {
                    Console.WriteLine($"Id продукта: {productId}, Название продукта: {productName}, Количество продуктов: {totalQuantity}");
                }
            }
        }

        static List<customer> GetCustomersByNameStarts(string nameLetterStarts)
        {
            using (var db = new LinqToDB.Data.DataConnection(ProviderName.PostgreSQL, _connectionString))
            {
                var table = db.GetTable<customer>();
                var list = table.Where(x => x.firstname.StartsWith(nameLetterStarts)).OrderBy(x => x.firstname).ToList();

                return list;
            }
        }

        static customer AddCustomer(string firstName, string lastName, int age)
        {
            using (var db = new LinqToDB.Data.DataConnection(ProviderName.PostgreSQL, _connectionString))
            {
                var newCustomer = new customer
                {
                    firstname = firstName,
                    lastname = lastName,
                    age = age
                };

                db.Insert(newCustomer);

                return newCustomer;
            }
        }

        static bool UpdateCustomer(int id, string firstName, string lastName, int age)
        {
            using (var db = new LinqToDB.Data.DataConnection(ProviderName.PostgreSQL, _connectionString))
            {
                var customerToUpdate = db.GetTable<customer>().FirstOrDefault(x => x.id == id);

                if (customerToUpdate != null)
                {
                    customerToUpdate.firstname = string.IsNullOrEmpty(firstName) ? customerToUpdate.firstname : firstName;
                    customerToUpdate.lastname = string.IsNullOrEmpty(lastName) ? customerToUpdate.lastname : lastName;
                    customerToUpdate.age = age < 0 ? customerToUpdate.age : age;

                    db.Update(customerToUpdate);
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Универсальный запрос сущности из любой таблицы по id элемента
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        static T GetById<T>(int id) where T : class, IEntity
        {
            using (var db = new LinqToDB.Data.DataConnection(LinqToDB.ProviderName.PostgreSQL, _connectionString))
            {
                var table = db.GetTable<T>();
                return table.FirstOrDefault(x => x.id == id);
            }
        }

        /// <summary>
        /// Запрашиваем количество всех купленных товаров пользователем по переданному id пользователя
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        static List<(int ProductId, string ProductName, int TotalQuantity)> GetPurchasedProducts(int customerId)
        {
            using (var db = new LinqToDB.Data.DataConnection(LinqToDB.ProviderName.PostgreSQL, _connectionString))
            {
                var ordersTable = db.GetTable<order>();
                var productsTable = db.GetTable<product>();

                // Выполняем запрос с использованием Join и GroupBy
                var result = (
                    from order in ordersTable
                    join product in productsTable on order.productid equals product.id
                    where order.customerid == customerId
                    group new { order, product } by new { product.id, product.name } into g
                    select new
                    {
                        ProductId = g.Key.id,
                        ProductName = g.Key.name,
                        TotalQuantity = g.Sum(x => x.order.quantity)
                    }
                ).ToList();

                // Преобразуем анонимный тип в кортеж
                return result.Select(r => (ProductId: r.ProductId, ProductName: r.ProductName, TotalQuantity: r.TotalQuantity)).ToList();
            }
        }
    }
}
