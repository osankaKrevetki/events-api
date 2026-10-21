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

## Структура проекта

```
src/EventsApi/
├── Controllers/   # EventsController — тонкие эндпоинты, только вызовы сервиса
├── Dtos/          # EventRequest (+ валидация), EventResponse, маппинг
├── Models/        # Доменная модель Event
├── Services/      # IEventService и его in-memory реализация EventService
└── Program.cs     # Регистрация зависимостей (DI), Swagger, обработка ошибок
```

`EventService` зарегистрирован как **Singleton**: список мероприятий хранится в самом сервисе
и должен жить всё время работы приложения. Доступ к списку синхронизирован, так как
singleton используется параллельными запросами.

## API

| Метод    | Путь           | Описание                         | Успех            | Ошибки     |
|----------|----------------|----------------------------------|------------------|------------|
| `GET`    | `/events`      | Список всех мероприятий          | `200 OK`         | —          |
| `GET`    | `/events/{id}` | Мероприятие по id                | `200 OK`         | `404`      |
| `POST`   | `/events`      | Создать мероприятие              | `201 Created`    | `400`      |
| `PUT`    | `/events/{id}` | Полностью обновить мероприятие   | `200 OK`         | `400`, `404` |
| `DELETE` | `/events/{id}` | Удалить мероприятие              | `204 No Content` | `404`      |

`id` — GUID, генерируется сервером при создании. `POST` возвращает созданное мероприятие
и заголовок `Location` со ссылкой на него.

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

Все ошибки возвращаются в едином формате [Problem Details (RFC 7807)](https://datatracker.ietf.org/doc/html/rfc7807).

Ошибка валидации (`400 Bad Request`):

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

Мероприятие не найдено (`404 Not Found`):

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Мероприятие не найдено",
  "status": 404,
  "detail": "Мероприятие с id '3fa85f64-5717-4562-b3fc-2c963f66afa6' не найдено."
}
```
