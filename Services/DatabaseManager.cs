// DatabaseManager.cs
using System;
using System.IO;
using Microsoft.Data.Sqlite; // Убедитесь, что пакет Microsoft.Data.Sqlite установлен через NuGet

namespace NOPRO
{
    public class DatabaseManager
    {
        // Явно указываем путь к файлу базы данных в папке приложения
        private readonly string _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nopro.db");
        private readonly object _lock = new object();

        public DatabaseManager()
        {
            InitializeDatabase();
        }

        /// <summary>
        /// Создаёт таблицы в базе данных, если они ещё не существуют.
        /// </summary>
        private void InitializeDatabase()
        {
            lock (_lock) // Защита от многопоточного доступа при инициализации
            {
                try
                {
                    using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
                    {
                        connection.Open();
                        string createTablesSql = @"
                            CREATE TABLE IF NOT EXISTS Prompts (
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                Name TEXT UNIQUE NOT NULL,
                                Template TEXT
                            );

                            CREATE TABLE IF NOT EXISTS Resources (
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                Key TEXT UNIQUE NOT NULL,
                                Value TEXT
                            );";

                        using (var command = new SqliteCommand(createTablesSql, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                    // Необязательно: Вывод пути к базе для проверки
                    // System.Diagnostics.Debug.WriteLine("База данных инициализирована: " + _dbPath);
                }
                catch (Exception ex)
                {
                    // Лучше использовать логирование (например, Serilog, NLog)
                    Console.WriteLine($"Ошибка инициализации базы данных: {ex.Message}");
                    System.Windows.MessageBox.Show($"Ошибка инициализации базы данных: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Получает шаблон промпта по его имени.
        /// </summary>
        /// <param name="name">Имя промпта.</param>
        /// <returns>Шаблон промпта или null, если не найден.</returns>
        public string GetPrompt(string name)
        {
            lock (_lock)
            {
                try
                {
                    using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
                    {
                        connection.Open();
                        string selectSql = "SELECT Template FROM Prompts WHERE Name = @name";
                        using (var command = new SqliteCommand(selectSql, connection))
                        {
                            command.Parameters.AddWithValue("@name", name);
                            var result = command.ExecuteScalar(); // ExecuteScalar возвращает первый столбец первой строки или null
                            return result?.ToString(); // Преобразуем в строку, если результат не null
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка получения промпта '{name}': {ex.Message}");
                    System.Windows.MessageBox.Show($"Ошибка получения промпта: {ex.Message}");
                    return null; // Возвращаем null в случае ошибки
                }
            }
        }

        /// <summary>
        /// Сохраняет или обновляет шаблон промпта по его имени.
        /// </summary>
        /// <param name="name">Имя промпта.</param>
        /// <param name="template">Шаблон промпта.</param>
        /// <returns>True, если успешно.</returns>
        public bool SavePrompt(string name, string template)
        {
            if (string.IsNullOrEmpty(name)) return false;

            lock (_lock)
            {
                try
                {
                    using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
                    {
                        connection.Open();
                        // Используем INSERT OR REPLACE для обновления или вставки
                        string insertSql = "INSERT OR REPLACE INTO Prompts (Name, Template) VALUES (@name, @template)";
                        using (var command = new SqliteCommand(insertSql, connection))
                        {
                            command.Parameters.AddWithValue("@name", name);
                            command.Parameters.AddWithValue("@template", template ?? string.Empty); // Обрабатываем null
                            command.ExecuteNonQuery();
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка сохранения промпта '{name}': {ex.Message}");
                    System.Windows.MessageBox.Show($"Ошибка сохранения промпта: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// Получает значение ресурса по ключу.
        /// </summary>
        /// <param name="key">Ключ ресурса.</param>
        /// <returns>Значение ресурса или null, если не найден.</returns>
        public string GetResource(string key)
        {
            lock (_lock)
            {
                try
                {
                    using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
                    {
                        connection.Open();
                        string selectSql = "SELECT Value FROM Resources WHERE Key = @key";
                        using (var command = new SqliteCommand(selectSql, connection))
                        {
                            command.Parameters.AddWithValue("@key", key);
                            var result = command.ExecuteScalar();
                            return result?.ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка получения ресурса '{key}': {ex.Message}");
                    System.Windows.MessageBox.Show($"Ошибка получения ресурса: {ex.Message}");
                    return null;
                }
            }
        }

        /// <summary>
        /// Сохраняет или обновляет значение ресурса по ключу.
        /// </summary>
        /// <param name="key">Ключ ресурса.</param>
        /// <param name="value">Значение ресурса.</param>
        /// <returns>True, если успешно.</returns>
        public bool SaveResource(string key, string value)
        {
            if (string.IsNullOrEmpty(key)) return false;

            lock (_lock)
            {
                try
                {
                    using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
                    {
                        connection.Open();
                        string insertSql = "INSERT OR REPLACE INTO Resources (Key, Value) VALUES (@key, @value)";
                        using (var command = new SqliteCommand(insertSql, connection))
                        {
                            command.Parameters.AddWithValue("@key", key);
                            command.Parameters.AddWithValue("@value", value ?? string.Empty);
                            command.ExecuteNonQuery();
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка сохранения ресурса '{key}': {ex.Message}");
                    System.Windows.MessageBox.Show($"Ошибка сохранения ресурса: {ex.Message}");
                    return false;
                }
            }
        }
    }
}