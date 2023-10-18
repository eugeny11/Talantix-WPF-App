CREATE DATABASE ShiftScheduler;

USE ShiftScheduler;


CREATE TABLE shifts (
    id INT AUTO_INCREMENT PRIMARY KEY,      -- Идентификатор записи
    City VARCHAR(255) NOT NULL,             -- Город
    Department VARCHAR(255) NOT NULL,       -- Отдел
    Employee VARCHAR(255) NOT NULL,         -- Сотрудник
    Shift VARCHAR(255) NOT NULL             -- Смена
);

INSERT INTO shifts (City, Department, Employee, Shift) 
VALUES 
    ('Москва', 'Отдел продаж', 'Иван Иванов', 'Первая (8:00 - 20:00)'),
    ('Санкт-Петербург', 'Отдел маркетинга', 'Петр Петров', 'Вторая (20:00 - 8:00)');
