using DataBaseLib;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewVariant.Exceptions;

namespace SiteForDB.Pages;

/// <summary>
/// Класс описывает модель страницы продаж.
/// </summary>
public class SaleModel : PageModel
{
    /// <summary>
    /// Сообщение ошибки при добавлении новой продажи.
    /// </summary>
    public static string ErrorMessage { get; set; } = string.Empty;
    
    public static ILogger<SaleModel>? Logger { get; private set; }

    /// <summary>
    /// Конструктор создаёт модель страницы продаж, на основе логгера.
    /// </summary>
    /// <param name="logger"> Используемый логгер.</param>
    public SaleModel(ILogger<SaleModel>? logger)
    {
        Logger = logger;
    }
    
    /// <summary>
    /// Метод обрабатывает POST - запрос на добавление новой продажи в базу данных на основе формы.
    /// </summary>
    /// <param name="buyerId"> Идентификатор покупателя добавляемой продажи.</param>
    /// <param name="shopId"> Идентификатор магазина добавляемой продажи.</param>
    /// <param name="goodId"> Идентификатор товара добавляемой продажи.</param>
    /// <param name="goodCount"> Количество купленных товаров добавляемой продажи.</param>
    public void OnPost(int buyerId, int shopId, int goodId, long goodCount)
    {
        try
        {
            Program.DataBase.InsertInto(() => new NewVariant.Models.Sale(buyerId, shopId, goodId, goodCount));
        }
        catch (DataBaseException exception)
        {
            Logger?.Log(LogLevel.Error, "{ExceptionMessage}", exception.Message);
            Program.DataBase.CreateTable<NewVariant.Models.Sale>();

            var flag = false;
            
            if (!DataAccessLayer.ShopIdIsExist(Program.DataBase, shopId))
            {
                ErrorMessage += $"Магазин с id = {shopId} не существует! ";
                flag = true;
            }
            
            if (!DataAccessLayer.BuyerIdIsExist(Program.DataBase, buyerId))
            {
                ErrorMessage += $"Покупателя с id = {buyerId} не существует! ";
                flag = true;
            }
            
            if (!DataAccessLayer.GoodIdIsExist(Program.DataBase, goodId))
            {
                ErrorMessage += $"Товар с id = {goodCount} не существует! ";
                flag = true;
            }

            if (flag)
            {
                return;
            }
            
            Program.DataBase.InsertInto(() => new NewVariant.Models.Sale(buyerId, shopId, goodId, goodCount));
        }
    }
}