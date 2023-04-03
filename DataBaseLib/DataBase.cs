using System.Runtime.Serialization;
using System.Text.Json;
using NewVariant.Exceptions;
using NewVariant.Interfaces;


namespace DataBaseLib;

public class DataBase : IDataBase
{
    // Таблицы базы данных.
    private readonly Dictionary<Type, IEnumerable<IEntity>> _tables;

    /// <summary>
    /// Конструктор создаёт пустую базу данных.
    /// </summary>
    public DataBase() => _tables = new Dictionary<Type, IEnumerable<IEntity>>();

    /// <summary>
    /// Метод создаёт новую таблицу типа Т.
    /// </summary>
    /// <typeparam name="T"> Тип создаваемой таблицы.</typeparam>
    /// <exception cref="DataBaseException"> Выбрасывается при существовании таблицы такого типа.</exception>
    public void CreateTable<T>() where T : IEntity
    {
        if (_tables.ContainsKey(typeof(T)))
        {
            throw new DataBaseException($"Таблица типа {typeof(T).Name} уже существует.");
        }

        _tables.Add(typeof(T), (new List<T>() as IEnumerable<IEntity>)!);
    }

    /// <summary>
    /// Метод добавляет новую строку в таблицу типа Т.
    /// </summary>
    /// <param name="getEntity"> Возвращает экземпляр, соответствующего типа.</param>
    /// <typeparam name="T"> Тип таблицы, в которую добавляют новую строку.</typeparam>
    /// <exception cref="DataBaseException"> Выбрасывается при отсутствии таблицы такого типа.</exception>
    public void InsertInto<T>(Func<T> getEntity) where T : IEntity
    {
        if (!_tables.ContainsKey(typeof(T)))
        {
            throw new DataBaseException($"Таблица типа {typeof(T).Name} не существует.");
        }

        _tables[typeof(T)] =
            (GetTable<T>().Append(getEntity.Invoke()) as IEnumerable<IEntity>)!;
    }

    /// <summary>
    /// Метод возвращает ссылку на таблицу типа Т.
    /// </summary>
    /// <typeparam name="T"> Тип искомой таблицы.</typeparam>
    /// <returns> Ссылка на таблицу типа Т.</returns>
    /// <exception cref="DataBaseException"> Выбрасывается при отсутствии таблицы такого типа.</exception>
    public IEnumerable<T> GetTable<T>() where T : IEntity
    {
        if (!_tables.ContainsKey(typeof(T)))
        {
            throw new DataBaseException($"Таблица типа {typeof(T).Name} не существует.");
        }

        return (_tables[typeof(T)] as IEnumerable<T>)!;
    }

    /// <summary>
    /// Метод сериализует таблицу типа Т в файл, расположенный по переданному пути.
    /// </summary>
    /// <param name="path"> Путь файла сериализации.</param>
    /// <typeparam name="T"> Тип сериализуемой таблицы.</typeparam>
    /// <exception cref="DataBaseException"> Выбрасывается при отсутствии таблицы такого типа.</exception>
    public void Serialize<T>(string path) where T : IEntity
    {
        if (!_tables.ContainsKey(typeof(T)))
        {
            throw new DataBaseException($"Таблица типа {typeof(T).Name} не существует.");
        }

        try
        {
            using var fileStream = new FileStream(path, FileMode.Create);
            JsonSerializer.Serialize(fileStream, GetTable<T>(), typeof(IEnumerable<T>));
        }
        catch (Exception exception)
        {
            throw new DataBaseException("Ошибка доступа к файлу!", exception);
        }
    }

    /// <summary>
    /// Метод десериализует и сохраняет таблицу типа Т из файла, расположенного по переданному пути.
    /// </summary>
    /// <param name="path"> Путь файла десериализации.</param>
    /// <typeparam name="T"> Тип десериализуемой таблицы.</typeparam>
    /// <exception cref="DataBaseException"> Выбрасывается при ошибке десериализации или доступа к файлу.</exception>
    public void Deserialize<T>(string path) where T : IEntity
    {
        IEnumerable<T> entities;
        try
        {
            using var fileStream = new FileStream(path, FileMode.Open);
            entities = JsonSerializer.Deserialize(fileStream, typeof(IEnumerable<T>)) as IEnumerable<T> ??
                       throw new DataBaseException("Ошибка десериализации!",
                           new SerializationException("В файле некорректные данные!"));
        }
        catch (DataBaseException dataBaseException)
        {
            throw new DataBaseException(dataBaseException.Message, dataBaseException.InnerException!);
        }
        catch (Exception exception)
        {
            throw new DataBaseException("Ошибка доступа к файлу!", exception);
        }

        if (!_tables.ContainsKey(typeof(T)))
        {
            _tables.Add(typeof(T), (entities as IEnumerable<IEntity>)!);
            return;
        }

        _tables[typeof(T)] = (entities as IEnumerable<IEntity>)!;
    }
}