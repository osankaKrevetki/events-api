# Events API

REST API для управления мероприятиями на ASP.NET Core Web API (.NET 8).
Данные хранятся в памяти приложения и сбрасываются при перезапуске.

## Требования

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) или новее

## Запуск

```bash
git clone <ссылка-на-репозиторий>
cd <папка-репозитория>
dotnet build
dotnet run --project src/EventsApi
```

После запуска API доступно по адресу `http://localhost:5140`,
Swagger UI — по адресу http://localhost:5140/swagger.

Для запуска по HTTPS: `dotnet run --project src/EventsApi --launch-profile https`
(Swagger: https://localhost:7263/swagger).

## Тесты

Юнит-тесты бизнес-логики `EventService` написаны на xUnit + FluentAssertions
и покрывают CRUD-операции, фильтрацию, пагинацию и неуспешные сценарии
(несуществующий id, некорректные даты и параметры пагинации).

```bash
dotnet test
```

## Структура проекта

```
src/EventsApi/
├── Controllers/   # EventsController — тонкие эндпоинты, только вызовы сервиса
├── Dtos/          # EventRequest (+ валидация), EventResponse, EventQuery,
│                  # PaginatedResult, маппинг
├── Exceptions/    # NotFoundException и ValidationException для сервиса
├── Middleware/    # ExceptionHandlingMiddleware — глобальная обработка ошибок
├── Models/        # Доменная модель Event
├── Services/      # IEventService и его in-memory реализация EventService
└── Program.cs     # Регистрация зависимостей (DI), Swagger, обработка ошибок
tests/EventsApi.Tests/  # Юнит-тесты EventService (xUnit)
```

`EventService` зарегистрирован как **Singleton**: список мероприятий хранится в самом сервисе
и должен жить всё время работы приложения. Доступ к списку синхронизирован, так как
singleton используется параллельными запросами.

## API

| Метод    | Путь           | Описание                         | Успех            | Ошибки     |
|----------|----------------|----------------------------------|------------------|------------|
| `GET`    | `/events`      | Список мероприятий с фильтрацией и пагинацией | `200 OK` | `400` |
| `GET`    | `/events/{id}` | Мероприятие по id                | `200 OK`         | `404`      |
| `POST`   | `/events`      | Создать мероприятие              | `201 Created`    | `400`      |
| `PUT`    | `/events/{id}` | Полностью обновить мероприятие   | `200 OK`         | `400`, `404` |
| `DELETE` | `/events/{id}` | Удалить мероприятие              | `204 No Content` | `404`      |

`id` — GUID, генерируется сервером при создании. `POST` возвращает созданное мероприятие
и заголовок `Location` со ссылкой на него.

### GET /events — фильтрация и пагинация

| Параметр   | Тип      | По умолчанию | Описание                                                    |
|------------|----------|--------------|-------------------------------------------------------------|
| `title`    | string   | —            | Поиск по названию (регистронезависимый, частичное совпадение) |
| `from`     | DateTime | —            | События, которые начинаются не раньше указанной даты        |
| `to`       | DateTime | —            | События, которые заканчиваются не позже указанной даты      |
| `page`     | int      | `1`          | Номер страницы, начиная с 1                                 |
| `pageSize` | int      | `10`         | Количество элементов на странице, от 1 до 100               |

Фильтры применяются только если переданы и объединяются по логическому И.
Фильтрация и пагинация реализованы на LINQ (`Where`, `OrderBy`, `Skip`, `Take`).

Пример запроса:

```
GET /events?title=dotnet&from=2026-02-01T00:00:00&to=2026-02-28T23:59:59&page=1&pageSize=10
```

Ответ — объект `PaginatedResult`:

```json
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "title": "dotnet party",
      "description": null,
      "startAt": "2026-03-05T12:00:00",
      "endAt": "2026-03-06T15:00:00"
    }
  ],
  "totalCount": 1,
  "page": 1,
  "pageSize": 10,
  "totalPages": 1
}
```

`items` — мероприятия текущей страницы (их количество — число элементов на странице),
`totalCount` — общее количество мероприятий, удовлетворяющих фильтрам.

### Тело запроса (POST, PUT)

```json
{
  "title": "Конференция .NET",
  "description": "Доклады о новинках ASP.NET Core",
  "startAt": "2026-11-01T10:00:00",
  "endAt": "2026-11-01T18:00:00"
}
```

| Поле          | Тип      | Обязательное | Правила                         |
|---------------|----------|--------------|---------------------------------|
| `title`       | string   | да           | не пустое, не длиннее 200 символов |
| `description` | string   | нет          |                                 |
| `startAt`     | DateTime | да           |                                 |
| `endAt`       | DateTime | да           | позже `startAt`                 |

### Ответ

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "title": "Конференция .NET",
  "description": "Доклады о новинках ASP.NET Core",
  "startAt": "2026-11-01T10:00:00",
  "endAt": "2026-11-01T18:00:00"
}
```

### Ошибки

Все ошибки возвращаются в едином формате [Problem Details (RFC 7807)](https://datatracker.ietf.org/doc/html/rfc7807)
с типом содержимого `application/problem+json`.

Необработанные исключения перехватывает глобальный `ExceptionHandlingMiddleware`
(этап 2 спринта 2): ошибки валидации → `400 Bad Request`, отсутствие ресурса → `404 Not Found`,
непредвиденные ошибки → `500 Internal Server Error`; ошибки логируются через `ILogger`.

Ошибка валидации полей модели (`400 Bad Request`):

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "EndAt": ["Дата окончания (EndAt) должна быть позже даты начала (StartAt)."]
  }
}
```

Некорректные параметры пагинации (`400 Bad Request`, из middleware):

```json
{
  "title": "Ошибка валидации",
  "status": 400,
  "detail": "Параметр page должен быть не меньше 1.",
  "instance": "/events",
  "traceId": "0HNQBII134TUM:00000001"
}
```

Мероприятие не найдено (`404 Not Found`, из middleware):

```json
{
  "title": "Ресурс не найден",
  "status": 404,
  "detail": "Мероприятие с id '3fa85f64-5717-4562-b3fc-2c963f66afa6' не найдено.",
  "instance": "/events/3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "traceId": "0HNQBII134TUL:00000001"
}
```

Непредвиденная ошибка (`500 Internal Server Error`, из middleware):

```json
{
  "title": "Внутренняя ошибка сервера",
  "status": 500,
  "detail": "Unexpected failure.",
  "instance": "/events",
  "traceId": "0HNQBII134TUS:00000001"
}
```
