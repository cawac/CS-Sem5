# EFCore.7 — консольное приложение (EF Code First)

Практическое задание: Entity Framework Code First, репозитории, транзакции, консольное меню.

## Запуск

```bash
dotnet run --project Solution
```

При первом запуске создаётся БД и заполняется тестовыми данными (по 30 записей в таблицах). По умолчанию используется SQLite; через конфигурацию можно переключиться на MS SQL Server.

## Структура решения

| Проект       | Назначение |
|-------------|------------|
| **DomainModel** | Сущности: Watches, Manufacturer, WatchesType. Без зависимостей от EF. |
| **Data**        | DbContext (Fluent API), репозитории, DataInitializer, бизнес-сервис (транзакция). |
| **Solution**    | Консоль: конфигурация (appsettings.json), DI, меню CRUD и бизнес-операции. |

## База данных

Поддерживаются два провайдера (задаётся в `appsettings.json`):

| Провайдер   | Ключ в конфиге | Строка подключения |
|-------------|----------------|---------------------|
| **SQLite**  | `DatabaseProvider: "Sqlite"` | `ConnectionStrings:DefaultConnection` (по умолчанию `Data Source=efcore.db`) |
| **MS SQL Server** | `DatabaseProvider: "SqlServer"` | `ConnectionStrings:SqlServer` или `DefaultConnection` |

Пример для SQL Server (LocalDB):

```json
{
  "DatabaseProvider": "SqlServer",
  "ConnectionStrings": {
    "SqlServer": "Server=(localdb)\\mssqllocaldb;Database=EFCore7;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Таблицы: **Manufacturers**, **Watches**. Связь один-ко-многим (у продукта — ManufacturerId). Схема создаётся при старте через `EnsureCreatedAsync`; данные заполняет `DataInitializer` (идемпотентно).

## Меню

- 0 — инициализация БД (создание таблиц и заполнение при необходимости).
- 1 — CRUD по производителям (Manufacturer).
- 2 — CRUD по продуктам (Watches).
- 3 — бизнес-операция: новый производитель и новый продукт в одной транзакции (с откатом при ошибке).
- 4 — выборка: все продукты указанного производителя.
- 5 — выход.
