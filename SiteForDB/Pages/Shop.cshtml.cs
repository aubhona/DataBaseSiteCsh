using Microsoft.AspNetCore.Mvc.RazorPages;
using NewVariant.Exceptions;

namespace SiteForDB.Pages;

/// <summary>
/// Класс описывает модель страницы магазинов.
/// </summary>
public class ShopModel : PageModel
{
    public static ILogger<ShopModel>? Logger { get; private set; }

    /// <summary>
    /// Конструктор создаёт модель страницы магазинов, на основе логгера.
    /// </summary>
    /// <param name="logger"> Используемый логгер.</param>
    public ShopModel(ILogger<ShopModel>? logger)
    {
        Logger = logger;
    }
    
    /// <summary>
    /// Метод обрабатывает POST - запрос на добавление нового магазина в базу данных на основе формы.
    /// </summary>
    /// <param name="name"> Название добавляемого магазина.</param>
    /// <param name="city"> Город добавляемого магазина.</param>
    /// <param name="country"> Страна добавляемого магазина.</param>
    public void OnPost(string name, string city, string country)
    {
        try
        {
            Program.DataBase.InsertInto(() => new NewVariant.Models.Shop(name, city, country));
        }
        catch (DataBaseException exception)
        {
            Logger?.Log(LogLevel.Error, "{ExceptionMessage}", exception.Message);
            Program.DataBase.CreateTable<NewVariant.Models.Shop>();
            Program.DataBase.InsertInto(() => new NewVariant.Models.Shop(name, city, country));
        }
    }
}