Для запуска программы нужен запущенный локально PostgreSQL.
Для того, чтобы указать новую строку подключения к БД, нужно изменить строку подключения protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder).
Тестовые методы проверяют отправку значений на локальный сервер и проверяет, чтобы последним добавлялись новые данные (они должны иметь наивысшее значение ID).
