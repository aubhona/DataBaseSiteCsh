using Microsoft.AspNetCore.Mvc.RazorPages;
using NewVariant.Exceptions;

namespace SiteForDB.Pages;

/// <summary>
/// Класс описывает модель страницы покупателей.
/// </summary>
public class BuyerModel : PageModel
{
    public static ILogger<BuyerModel>? Logger { get; private set; }

    /// <summary>
    /// Конструктор создаёт модель страницы покупателей, на основе логгера.
    /// </summary>
    /// <param name="logger"> Используемый логгер.</param>
    public BuyerModel(ILogger<BuyerModel>? logger)
    {
        Logger = logger;
    }

    /// <summary>
    /// Метод обрабатывает POST - запрос на добавление нового покупателя в базу данных на основе формы.
    /// </summary>
    /// <param name="name"> Имя добавляемого покупателя.</param>
    /// <param name="surname"> Фамилия добавляемого покупателя.</param>
    /// <param name="city"> Город добавляемого покупателя.</param>
    /// <param name="country"> Страна добавляемого покупателя.</param>
    public void OnPost(string name, string surname, string city, string country)
    {
        try
        {
            Program.DataBase.InsertInto(() => new NewVariant.Models.Buyer(name, surname, city, country));
        }
        // Если таблицы не существует.
        catch (DataBaseException exception)
        {
            Logger?.Log(LogLevel.Error, "{ExceptionMessage}", exception.Message);
            Program.DataBase.CreateTable<NewVariant.Models.Buyer>();
            Program.DataBase.InsertInto(() => new NewVariant.Models.Buyer(name, surname, city, country));
        }
    }
}