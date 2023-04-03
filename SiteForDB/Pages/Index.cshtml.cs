using DataBaseLib;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewVariant.Exceptions;
using NewVariant.Models;

namespace SiteForDB.Pages;

/// <summary>
/// Класс описывает модель главной страницы.
/// </summary>
public class IndexModel : PageModel
{
    /// <summary>
    /// Сообщение для пользователя.
    /// </summary>
    public static string Message { get; set; } = string.Empty;

    /// <summary>
    /// Тип сообщения для пользователя.
    /// </summary>
    public static string MessageType { get; set; } = string.Empty;

    private readonly ILogger<IndexModel> _logger;

    /// <summary>
    /// Результат запросов.
    /// </summary>
    public static IEnumerable<object> Result { get; set; } = new List<object>();


    /// <summary>
    /// Конструктор создаёт модель главной страницы, на основе логгера.
    /// </summary>
    /// <param name="logger"> Используемый логгер.</param>
    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Метод обрабатывает POST - запрос на получение списка всех товаров, купленных покупателем с самым длинным именем.
    /// </summary>
    public void OnPostAllGoodsOfLongestNameBuyer()
    {
        try
        {
            Message = "Cписок всех товаров, купленных покупателем с самым длинным именем";
            MessageType = "alert-info";
            Result = Program.DataAccessLayer.GetAllGoodsOfLongestNameBuyer(Program.DataBase);
        }
        catch (DataBaseException exception)
        {
            Message = exception.Message;
            MessageType = "alert-danger";
            _logger.Log(LogLevel.Error, "{ExceptionMessage}", exception.Message);
        }
    }

    /// <summary>
    /// Метод обрабатывает POST - запрос на получение названия категории самого дорогого товара.
    /// </summary>
    public void OnPostMostExpensiveGoodCategory()
    {
        try
        {
            Message = "Название категории самого дорогого товара";
            MessageType = "alert-info";
            Result = new object[]
                { Program.DataAccessLayer.GetMostExpensiveGoodCategory(Program.DataBase) ?? string.Empty };
        }
        catch (DataBaseException exception)
        {
            Message = exception.Message;
            MessageType = "alert-danger";
            _logger.Log(LogLevel.Error, "{ExceptionMessage}", exception.Message);
        }

    }

    /// <summary>
    /// Метод обрабатывает POST - запрос на получение названия города, в котором было потрачено меньше всего денег.
    /// </summary>
    public void OnPostMinimumSalesCity()
    {
        try
        {
            Message =
                "Название города, в котором было потрачено меньше всего денег";
            MessageType = "alert-info";
            Result = new object[] { Program.DataAccessLayer.GetMinimumSalesCity(Program.DataBase) ?? string.Empty };
        }
        catch (DataBaseException exception)
        {
            Message = exception.Message;
            MessageType = "alert-danger";
            _logger.Log(LogLevel.Error, "{ExceptionMessage}", exception.Message);
        }
    }

    /// <summary>
    /// Метод обрабатывает POST - запрос на получение списка покупателей, которые купили самый популярный товар.
    /// </summary>
    public void OnPostMostPopularGoodBuyers()
    {
        try
        {
            Message =
                "Список покупателей, которые купили самый популярный товар";
            MessageType = "alert-info";
            Result = Program.DataAccessLayer.GetMostPopularGoodBuyers(Program.DataBase);
        }
        catch (DataBaseException exception)
        {
            Message = exception.Message;
            MessageType = "alert-danger";
            _logger.Log(LogLevel.Error, "{ExceptionMessage}", exception.Message);
        }
    }

    /// <summary>
    /// Метод обрабатывает POST - запрос на получение минимального количества магазинов в стране.
    /// </summary>
    public void OnPostMinimumNumberOfShopsInCountry()
    {
        try
        {
            Message = "Минимальное количество магазинов в стране";
            MessageType = "alert-info";
            Result = new object[] { Program.DataAccessLayer.GetMinimumNumberOfShopsInCountry(Program.DataBase) };
        }
        catch (DataBaseException exception)
        {
            Message = exception.Message;
            MessageType = "alert-danger";
            _logger.Log(LogLevel.Error, "{ExceptionMessage}", exception.Message);
        }
    }

    /// <summary>
    /// Метод обрабатывает POST - запрос на получение списка покупок,
    /// совершенных покупателями во всех городах, отличных от города их проживания.
    /// </summary>
    public void OnPostOtherCitySales()
    {
        try
        {
            Message = "Cписок покупок, совершенных покупателями во всех городах, отличных от города их проживания";
            MessageType = "alert-info";
            Result = Program.DataAccessLayer.GetOtherCitySales(Program.DataBase);
        }
        catch (DataBaseException exception)
        {
            Message = exception.Message;
            MessageType = "alert-danger";
            _logger.Log(LogLevel.Error, "{ExceptionMessage}", exception.Message);
        }
    }

    /// <summary>
    /// Метод обрабатывает POST - запрос на получение общей стоимости покупок, совершенных всеми покупателями.
    /// </summary>
    public void OnPostTotalSalesValue()
    {
        try
        {
            Message = "Общая стоимость покупок, совершенных всеми покупателями";
            MessageType = "alert-info";
            Result = new object[] { Program.DataAccessLayer.GetTotalSalesValue(Program.DataBase) };
        }
        catch (DataBaseException exception)
        {
            Message = exception.Message;
            MessageType = "alert-danger";
            _logger.Log(LogLevel.Error, "{ExceptionMessage}", exception.Message);
        }
    }

    /// <summary>
    /// Метод обрабатывает POST - запрос на сериализацию таблицы типа type.
    /// </summary>
    /// <param name="type"> Тип сериализуемой таблицы.</param>
    /// <param name="path"> Путь файла сериализации.</param>
    /// <exception cref="DataBaseException"> Выбрасывается при отсутствии такого типа.</exception>
    public void OnPostSerialize(string type, string path)
    {
        try
        {
            Message = "Таблица успешно сериализовалась.";
            MessageType = "alert-success";
            switch (type)
            {
                case "Buyer":
                    Program.DataBase.Serialize<Buyer>(path);
                    break;
                case "Good":
                    Program.DataBase.Serialize<Good>(path);
                    break;
                case "Shop":
                    Program.DataBase.Serialize<Shop>(path);
                    break;
                case "Sale":
                    Program.DataBase.Serialize<Sale>(path);
                    break;
                default:
                    throw new DataBaseException();
            }
        }
        catch (DataBaseException exception)
        {
            Message = exception.Message;
            MessageType = "alert-danger";
            _logger.Log(LogLevel.Error, "{ExceptionMessage}", exception.Message);
        }
    }

    /// <summary>
    /// Метод обрабатывает POST - запрос на десериализацию таблицы типа type.
    /// </summary>
    /// <param name="type"> Тип десериализуемой таблицы.</param>
    /// <param name="path"> Путь файла десериализации.</param>
    /// <exception cref="DataBaseException"> Выбрасывается при отсутствии такого типа.</exception>
    public void OnPostDeserialize(string type, string path)
    {
        try
        {
            Message = "Таблица успешно десериализовалась";
            MessageType = "alert-success";
            switch (type)
            {
                case "Buyer":
                    Program.DataBase.Deserialize<Buyer>(path);
                    if (!DataAccessLayer.BuyerTableIsCorrect(Program.DataBase))
                    {
                        throw new NullReferenceException("Ошибка десериализации!");
                    }

                    break;
                case "Good":
                    Program.DataBase.Deserialize<Good>(path);
                    if (!DataAccessLayer.GoodTableIsCorrect(Program.DataBase))
                    {
                        throw new NullReferenceException("Ошибка десериализации!");
                    }

                    break;
                case "Shop":
                    Program.DataBase.Deserialize<Shop>(path);
                    if (!DataAccessLayer.ShopTableIsCorrect(Program.DataBase))
                    {
                        throw new NullReferenceException("Ошибка десериализации!");
                    }

                    break;
                case "Sale":
                    Program.DataBase.Deserialize<Sale>(path);
                    if (!DataAccessLayer.SaleTableIsCorrect(Program.DataBase))
                    {
                        throw new NullReferenceException("Ошибка десериализации!");
                    }

                    break;
                default:
                    throw new DataBaseException("Такого типа нет!");
            }
        }
        catch (NullReferenceException exception)
        {
            Program.DataBase = new DataBase();
            Message = exception.Message;
            MessageType = "alert-danger";
            _logger.Log(LogLevel.Error, "{ExceptionMessage}", exception.Message);
        }
        catch (DataBaseException exception)
        {
            Message = exception.Message;
            MessageType = "alert-danger";
            _logger.Log(LogLevel.Error, "{ExceptionMessage}", exception.Message);
        }
    }
}