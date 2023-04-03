using NewVariant.Exceptions;
using NewVariant.Interfaces;
using NewVariant.Models;

namespace DataBaseLib;

public class DataAccessLayer : IDataAccessLayer
{
    /// <summary>
    /// Метод генерирует список всех товаров, купленных покупателем с самым длинным именем.
    /// </summary>
    /// <param name="dataBase"> База данных.</param>
    /// <returns> Список всех товаров, купленных покупателем с самым длинным именем.</returns>
    public IEnumerable<Good> GetAllGoodsOfLongestNameBuyer(IDataBase dataBase)
    {
        try
        {
            var idOfBuyerWithLongestName =
                dataBase.GetTable<Buyer>().MaxBy(buyer => (buyer.Name.Length, buyer.Name, buyer.Id))?.Id;

            return from sale in dataBase.GetTable<Sale>()
                join good in dataBase.GetTable<Good>() on sale.GoodId equals good.Id into goods
                where sale.BuyerId == idOfBuyerWithLongestName
                from goodAns in goods
                select goodAns;
        }
        catch (NullReferenceException)
        {
            return new List<Good>();
        }
    }


    /// <summary>
    /// Метод ищет название категории самого дорогого товара.
    /// </summary>
    /// <param name="dataBase"> База данных.</param>
    /// <returns> Категории самого дорогого товара.</returns>
    public string? GetMostExpensiveGoodCategory(IDataBase dataBase) =>
        dataBase.GetTable<Good>().MaxBy(good => good.Price)?.Category;

    /// <summary>
    /// Метод ищет название города, в котором было потрачено меньше всего денег.
    /// </summary>
    /// <param name="dataBase"> База данных.</param>
    /// <returns> Название города, в котором было потрачено меньше всего денег.</returns>
    public string? GetMinimumSalesCity(IDataBase dataBase) => (from sale in dataBase.GetTable<Sale>()
            join good in dataBase.GetTable<Good>() on sale.GoodId equals good.Id
            join shop in dataBase.GetTable<Shop>() on sale.ShopId equals shop.Id
            group good.Price * sale.GoodCount by shop.City).MinBy(shop => shop.Sum())?.Key;
    
    
    /// <summary>
    /// Метод генерирует список покупателей, которые купили самый популярный товар.
    /// </summary>
    /// <param name="dataBase"> База данных.</param>
    /// <returns> Список покупателей, которые купили самый популярный товар.</returns>
    public IEnumerable<Buyer> GetMostPopularGoodBuyers(IDataBase dataBase)
    {
        var sales = dataBase.GetTable<Sale>().ToArray();
        var mostPopularGood = (from sale in sales group sale.GoodCount by sale.GoodId).MaxBy(sale => sale.Sum());
        return mostPopularGood is null
            ? new List<Buyer>()
            : from sale in sales
            where sale.GoodId == mostPopularGood.Key
            join buyer in dataBase.GetTable<Buyer>() on sale.BuyerId equals buyer.Id
            select buyer;
    }
    
    /// <summary>
    /// Метод ищет минимальное количество магазинов в стране.
    /// </summary>
    /// <param name="dataBase"> База данных.</param>
    /// <returns> Минимальное количество магазинов в стране.</returns>
    public int GetMinimumNumberOfShopsInCountry(IDataBase dataBase)
    {
        try
        {
            return (from shop in dataBase.GetTable<Shop>() group 1 by shop.Country).Min(country => country.Count());
        }
        catch (InvalidOperationException)
        {
            return 0;
        }
    }
    
    /// <summary>
    /// Метод генерирует список покупок, совершенных покупателями во всех городах, отличных от города их проживания.
    /// </summary>
    /// <param name="dataBase"> База данных.</param>
    /// <returns> Список покупок, совершенных покупателями во всех городах, отличных от города их проживания.</returns>
    public IEnumerable<Sale> GetOtherCitySales(IDataBase dataBase) => from sale in dataBase.GetTable<Sale>()
        join shop in dataBase.GetTable<Shop>() on sale.ShopId equals shop.Id
        join buyer in dataBase.GetTable<Buyer>() on sale.BuyerId equals buyer.Id
        where shop.City != buyer.City
        select sale;
    
    /// <summary>
    /// Метод считает общую стоимость покупок, совершенных всеми покупателями.
    /// </summary>
    /// <param name="dataBase"> База данных.</param>
    /// <returns> Общая стоимость покупок, совершенных всеми покупателями.</returns>
    public long GetTotalSalesValue(IDataBase dataBase) => (from sale in dataBase.GetTable<Sale>()
        join good in dataBase.GetTable<Good>() on sale.GoodId equals good.Id
        select sale.GoodCount * good.Price).Sum();

    /// <summary>
    /// Метод проверяет наличие данного магазина в базе данных.
    /// </summary>
    /// <param name="dataBase"> База данных.</param>
    /// <param name="shopId"> Идентификатор проверяемого магазина.</param>
    /// <returns> True - если есть в базе, False - иначе.</returns>
    public static bool ShopIdIsExist(IDataBase dataBase, int shopId)
    {
        try
        {
            return dataBase.GetTable<Shop>().Any(shop => shop.Id == shopId);
        }
        catch (DataBaseException)
        {
            return false;
        }
    }
    
    /// <summary>
    /// Метод проверяет наличие данного покупателя в базе данных.
    /// </summary>
    /// <param name="dataBase"> База данных.</param>
    /// <param name="buyerId"> Идентификатор проверяемого покупателя.</param>
    /// <returns> True - если есть в базе, False - иначе.</returns>
    public static bool BuyerIdIsExist(IDataBase dataBase, int buyerId)
    {
        try
        {
            return dataBase.GetTable<Buyer>().Any(buyer => buyer.Id == buyerId);
        }
        catch (DataBaseException)
        {
            return false;
        }
    }
    
    /// <summary>
    /// Метод проверяет наличие данного товара в базе данных.
    /// </summary>
    /// <param name="dataBase"> База данных.</param>
    /// <param name="goodId"> Идентификатор проверяемого товара.</param>
    /// <returns> True - если есть в базе, False - иначе.</returns>
    public static bool GoodIdIsExist(IDataBase dataBase, int goodId)
    {
        try
        {
            return dataBase.GetTable<Good>().Any(good => good.Id == goodId);
        }
        catch (DataBaseException)
        {
            return false;
        }
    }
    
    /// <summary>
    /// Метод проверяет на корректность объектов таблицы Buyer.
    /// </summary>
    /// <param name="dataBase"> База данных.</param>
    /// <returns> True - если таблица корректна, False - иначе.</returns>
    public static bool BuyerTableIsCorrect(IDataBase dataBase) => !dataBase.GetTable<Buyer>().Any(buyer =>
        new[] { buyer.City, buyer.Country, buyer.Name, buyer.Surname }.Any(str => str == null!) || buyer.Id < 0);

    /// <summary>
    /// Метод проверяет на корректность объектов таблицы Good.
    /// </summary>
    /// <param name="dataBase"> База данных.</param>
    /// <returns> True - если таблица корректна, False - иначе.</returns>
    public static bool GoodTableIsCorrect(IDataBase dataBase) => !dataBase.GetTable<Good>().Any(good =>
        new[] { good.Name, good.Category }.Any(str => str == null!) || good.Price < 0 || good.Id < 0 ||
        good.ShopId < 0);

    /// <summary>
    /// Метод проверяет на корректность объектов таблицы Shop.
    /// </summary>
    /// <param name="dataBase"> База данных.</param>
    /// <returns> True - если таблица корректна, False - иначе.</returns>
    public static bool ShopTableIsCorrect(IDataBase dataBase) => !dataBase.GetTable<Shop>().Any(shop =>
        new[] { shop.Name, shop.City, shop.Country }.Any(str => str == null!) || shop.Id < 0);

    /// <summary>
    /// Метод проверяет на корректность объектов таблицы Sale.
    /// </summary>
    /// <param name="dataBase"> База данных.</param>
    /// <returns> True - если таблица корректна, False - иначе.</returns>
    public static bool SaleTableIsCorrect(IDataBase dataBase) => !dataBase.GetTable<Sale>().Any(sale =>
        sale.Id < 0 || sale.GoodId < 0 || sale.GoodCount <= 0 || sale.BuyerId < 0 || sale.ShopId < 0);
}