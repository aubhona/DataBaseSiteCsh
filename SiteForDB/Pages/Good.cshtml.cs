using DataBaseLib;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewVariant.Exceptions;

namespace SiteForDB.Pages;

/// <summary>
/// Класс описывает модель страницы товаров.
/// </summary>
public class GoodModel : PageModel
{
    /// <summary>
    /// Сообщение ошибки доьавления товара.
    /// </summary>
    public static string ErrorMessage { get; set; } = string.Empty;
    
    public static ILogger<GoodModel>? Logger { get; private set; }

    /// <summary>
    /// Конструктор создаёт модель страницы товаров, на основе логгера.
    /// </summary>
    /// <param name="logger"> Используемый логгер.</param>
    public GoodModel(ILogger<GoodModel>? logger)
    {
        Logger = logger;
    }
    
    /// <summary>
    /// Метод обрабатывает POST - запрос на добавление нового товара в базу данных на основе формы.
    /// </summary>
    /// <param name="name"> Название добавляемого товара.</param>
    /// <param name="shopId"> Идентификатор магазина добавляемого товара..</param>
    /// <param name="category"> Категория добавляемого товара.</param>
    /// <param name="price"> Цена добавляемого товара.</param>
    public void OnPost(string name, int shopId, string category, long price)
    {
        try
        {
            Program.DataBase.InsertInto(() => new NewVariant.Models.Good(name, shopId, category, price));
        }
        // Если таблицы не существует.
        catch (DataBaseException exception)
        {
            Logger?.Log(LogLevel.Error, "{ExceptionMessage}", exception.Message);
            Program.DataBase.CreateTable<NewVariant.Models.Good>();
            if (!DataAccessLayer.ShopIdIsExist(Program.DataBase, shopId))
            {
                ErrorMessage = $"Магазин с id = {shopId} не существует!";
                return;
            }
            Program.DataBase.InsertInto(() => new NewVariant.Models.Good(name, shopId, category, price));
        }
    }
}